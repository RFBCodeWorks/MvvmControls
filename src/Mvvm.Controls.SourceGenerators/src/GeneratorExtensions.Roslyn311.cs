#if !ROSLYN_4_0_OR_GREATER

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace RFBCodeWorks.Mvvm.SourceGenerators
{

    internal static partial class GeneratorExtensions
    {
        public static void ForAttributeWithMetaDataName<T>(
            this GeneratorInitializationContext context, 
            string attributeName, 
            Func<SyntaxNode, bool> selector, 
            Func<GeneratorAttributeSyntaxContext, CancellationToken, DataOrDiagnostics<T>> transformer
            )
            where T :struct
        {
            context.RegisterForSyntaxNotifications(() => new ContextReceiver<T>(attributeName, selector, transformer));
        }

        public static void ForAttributeWithMetaDataName<T>(this GeneratorInitializationContext context, Func<T> createReceiver)
            where T: ContextReceiverBase, ISyntaxContextReceiver
        {
            if (createReceiver is SyntaxContextReceiverCreator r)
                context.RegisterForSyntaxNotifications(r);
            else
                context.RegisterForSyntaxNotifications(() => createReceiver());
        }
    }
    
    /// helper used for type compatibility of generic methods
    internal abstract class ContextReceiverBase { }
    
    /// <summary>
    /// This ContextReceiver is used to mimick the Roslyn4 ForAttributeWithMetaDataName 
    /// </summary>
    /// <remarks> Only evaluates <see cref="MemberDeclarationSyntax"/> nodes. </remarks>
    /// <typeparam name="T"></typeparam>
    /// <remarks>implements <see cref="ISyntaxContextReceiver"/></remarks>
    internal class ContextReceiver<T> : ContextReceiverBase, ISyntaxContextReceiver where T : struct
    {
        public ContextReceiver(
            string attributeName, 
            Func<SyntaxNode, bool> selector, 
            Func<GeneratorAttributeSyntaxContext, CancellationToken, DataOrDiagnostics<T>> transformer
            )
        {
            if (string.IsNullOrWhiteSpace(attributeName)) throw new ArgumentException("attribute name is not fully qualified", nameof(attributeName));
            _qualifiedName = attributeName;
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
        }
        private readonly string _qualifiedName;
        private readonly Func<SyntaxNode, bool> _selector;
        private readonly Func<GeneratorAttributeSyntaxContext, CancellationToken, DataOrDiagnostics<T>> _transformer;
        private readonly List<GeneratorAttributeSyntaxContext> _candidates = new(1);

        public bool AnyCandidates => _candidates is not null && _candidates.Count > 0;

        /// <summary>
        /// When enabled, transforms any FieldDeclarationSyntax into the <see cref="VariableDeclaratorSyntax"/> nodes before adding to the candidate collection
        /// </summary>
        protected bool TransformFieldDeclarations { get; set; }

        /// <summary>
        /// Reports all diagnostics and Yield Returns any valid candidates
        /// </summary>
        public IEnumerable<T> ReportAndEnumerate(Action<Diagnostic> reportDiagnostic, CancellationToken token)
        {
            if (!AnyCandidates)
            {
                yield break;
            }
            else
            {
                foreach (var c in _candidates)
                {
                    token.ThrowIfCancellationRequested();
                    var data = _transformer(c, token);
                    if (data.IsErrored)
                    {
                        foreach (var diag in data.Diagnostics)
                        {
                            token.ThrowIfCancellationRequested();
                            reportDiagnostic(diag);
                        }
                    }
                    if (data.IsValid)
                    {
                        yield return data.Data;
                    }
                }
            }
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is null || context.SemanticModel is null || _selector(context.Node) == false) return;
            if (context.Node is not MemberDeclarationSyntax member) return;
            if (_selector(member) == false) return;
            if (member.AttributeLists.Count == 0) return;

            SyntaxNode node;
            ISymbol symbol;
            ImmutableArray<AttributeData>.Builder attributeBuilder = null;

            if (member is FieldDeclarationSyntax fsyntax)
            {
                node = fsyntax.Declaration.Variables[0];
                symbol = context.SemanticModel.GetDeclaredSymbol(node);

                // special case to iterate across all field variables
                if (fsyntax.Declaration.Variables.Count > 1)
                {
                    // attributes are shared across all declarations
                    foreach (var attribute in symbol.GetAttributes())
                    {
                        if (attribute.AttributeClass.ToDisplayString() == _qualifiedName)
                        {
                            (attributeBuilder ??= ImmutableArray.CreateBuilder<AttributeData>()).Add(attribute);
                        }
                    }
                    if (attributeBuilder is null || attributeBuilder.Count == 0) return;
                    ImmutableArray<AttributeData> attrArray = attributeBuilder.ToImmutableArray();
                    
                    // add each declaration as a candidate
                    foreach(var value in fsyntax.Declaration.Variables)
                    {
                        symbol = context.SemanticModel.GetDeclaredSymbol(value);
                        _candidates.Add(new(value, symbol, context.SemanticModel, attrArray));
                    }
                    return;
                }
            }
            else if (context.SemanticModel.GetDeclaredSymbol(context.Node) is ISymbol memberSymbol)
            {
                node = context.Node;
                symbol = memberSymbol;
                if (member.Modifiers.Any(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword) && _candidates.Any(d => d.TargetSymbol.Equals(symbol, SymbolEqualityComparer.Default))) return;
            }
            else
            {
                return;
            }

            // Get attributes
            foreach (var attribute in symbol.GetAttributes())
            {
                if (attribute.AttributeClass.ToDisplayString() == _qualifiedName)
                {
                    (attributeBuilder ??= ImmutableArray.CreateBuilder<AttributeData>()).Add(attribute);
                }
            }
            if (attributeBuilder is null || attributeBuilder.Count == 0) return;

            // add the item to the collection of candidates. Transform will not be applied until a cancellation token is available.
            _candidates.Add(new(node, symbol, context.SemanticModel, attributeBuilder.ToImmutable()));
        }
    }

    // Taken from the Roslyn4 pkg
    internal readonly struct GeneratorAttributeSyntaxContext
    {
        /// <summary>
        /// The syntax node the attribute is attached to.  For example, with <c>[CLSCompliant] class C { }</c> this would
        /// the class declaration node.
        /// </summary>
        public SyntaxNode TargetNode { get; }

        /// <summary>
        /// The symbol that the attribute is attached to.  For example, with <c>[CLSCompliant] class C { }</c> this would be
        /// the <see cref="INamedTypeSymbol"/> for <c>"C"</c>.
        /// </summary>
        public ISymbol TargetSymbol { get; }

        /// <summary>
        /// Semantic model for the file that <see cref="TargetNode"/> is contained within.
        /// </summary>
        public SemanticModel SemanticModel { get; }

        /// <summary>
        /// <see cref="AttributeData"/>s for any matching attributes on <see cref="TargetSymbol"/>.  Always non-empty.  All
        /// these attributes will have an <see cref="AttributeData.AttributeClass"/> whose fully qualified name metadata
        /// name matches the name requested in <see cref="SyntaxValueProvider.ForAttributeWithMetadataName{T}"/>.
        /// <para>
        /// To get the entire list of attributes, use <see cref="ISymbol.GetAttributes"/> on <see cref="TargetSymbol"/>.
        /// </para>
        /// </summary>
        public ImmutableArray<AttributeData> Attributes { get; }

        internal GeneratorAttributeSyntaxContext(
            SyntaxNode targetNode,
            ISymbol targetSymbol,
            SemanticModel semanticModel,
            ImmutableArray<AttributeData> attributes)
        {
            TargetNode = targetNode;
            TargetSymbol = targetSymbol;
            SemanticModel = semanticModel;
            Attributes = attributes;
        }
    }
}
#endif
