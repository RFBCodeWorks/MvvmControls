using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    internal class TriggersRefreshParser
    {
        public const string QualifiedName = "RFBCodeWorks.Mvvm." + nameof(RFBCodeWorks.Mvvm.TriggersRefreshAttribute);
        public const string ObservablePropertyAttribute = nameof(ObservablePropertyAttribute);
        public const string ObservablePropertyAttributeFullyQualified = "CommunityToolkit.Mvvm.ComponentModel" + ObservablePropertyAttribute;

        /// <summary> Roslyn 311 selector -> Gather FieldDeclarationSyntax nodes which will then be trasnformed </summary>
        public static bool Selector(SyntaxNode node)
        {
#if DEBUG
            if (node is FieldDeclarationSyntax)
            {
                //GeneratorExtensions.DebuggerBreak();
                return true;
            }
            else 
            {
                return false;
            }
#else
            return node is FieldDeclarationSyntax;
#endif
        }

        /// <summary> Roslyn 4 selector -> Gather VariableDeclaratorSyntax nodes directly </summary>
        public static bool Selector(SyntaxNode node, CancellationToken token) => node is VariableDeclaratorSyntax;

        public static TriggersRefreshData GetAllSelectorTargets(ISymbol symbol, CancellationToken token)
        {
            return new TriggersRefreshData(null, ParseAttributes(symbol.GetAttributes()
                .Where(attr =>
                attr.AttributeClass.Name == nameof(RFBCodeWorks.Mvvm.TriggersRefreshAttribute)
                && attr.AttributeClass.ContainingNamespace.Name == "Mvvm"
                && attr.AttributeClass.ContainingNamespace.ContainingNamespace.Name == "RFBCodeWorks"
                ), token));
        }

        public static DataOrDiagnostics<TriggersRefreshData> GetDataOrDiagnostics(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            if (context.TargetNode.GetClassDeclarationSyntax(token) is not TypeDeclarationSyntax parent) return default;
            if (context.TargetNode.IsNotPartialClass(token, out var partialDiag))
            {
                return new(partialDiag);
            }
            if (context.TargetSymbol.GetAttributes().Any(a => a.AttributeClass.Name == ObservablePropertyAttribute) is false)
            {
                return new(Diagnostic.Create(MvvmDiagnostics.InvalidTriggersRefreshUsage, context.TargetNode.GetLocation(), context.TargetSymbol.Name));
            }
            return new(new TriggersRefreshData(context, ParseAttributes(context.Attributes, token)));
        }

        private static ImmutableArray<string> ParseAttributes(IEnumerable<AttributeData> attributes, CancellationToken token = default)
        {
            ImmutableArray<string>.Builder builder = null;
            foreach (var attr in attributes)
            {
                token.ThrowIfCancellationRequested();
                if (attr.ConstructorArguments.Length == 1 && attr.ConstructorArguments[0].Kind == TypedConstantKind.Array)
                {
                    foreach (var item in attr.ConstructorArguments[0].Values)
                    {
                        token.ThrowIfCancellationRequested();
                        if (item.Value is string name && !string.IsNullOrWhiteSpace(name))
                            (builder ??= ImmutableArray.CreateBuilder<string>()).Add(name);
                    }
                }
            }
            return builder is null ? default : builder.ToImmutable();
        }
    }
}
