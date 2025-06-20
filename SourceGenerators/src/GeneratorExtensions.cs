using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace RFBCodeWorks.Mvvm.SourceGenerators
{
    internal static class GeneratorExtensions
    {
#if ROSLYN_4_0_OR_GREATER

#else
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
#endif
    }

#if !ROSLYN_4_0_OR_GREATER
    
    /// helper used for type compatibility of generic methods
    internal abstract class ContextReceiverBase { }
    
    /// <summary>
    /// This ContextReceiver is used to mimick the Roslyn4 ForAttributeWithMetaDataName 
    /// </summary>
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
            if (context.Node is null || context.SemanticModel is null) return;
            if (context.Node is not MemberDeclarationSyntax member) return; // types/fields/properties/methods
            if (member.AttributeLists.Count == 0) return;
            if (_selector(member) == false) return;
            if (context.SemanticModel.GetDeclaredSymbol(member) is not ISymbol symbol) return;

            // avoid duplicates
            if (member.Modifiers.Any(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword) && _candidates.Any(d => d.TargetSymbol.Equals(symbol, SymbolEqualityComparer.Default))) return; 

            ImmutableArray<AttributeData>.Builder attributeBuilder = null;

            foreach (var attribute in symbol.GetAttributes())
            {
                if (attribute.AttributeClass.ToDisplayString() == _qualifiedName)
                {
                    (attributeBuilder ??= ImmutableArray.CreateBuilder<AttributeData>()).Add(attribute);
                }
            }
            if (attributeBuilder is null || attributeBuilder.Count == 0) return;

            // add the item to the collection of candidates. Transform will not be applied until a cancellation token is available.
            _candidates.Add(new(member, symbol, context.SemanticModel, attributeBuilder.ToImmutable()));
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
#endif
}
