using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    internal readonly struct TriggersRefreshData : IEquatable<TriggersRefreshData>
    {
        public const string QualifiedName = "RFBCodeWorks.Mvvm." + nameof(RFBCodeWorks.Mvvm.TriggersRefreshAttribute);

        private TriggersRefreshData(in GeneratorAttributeSyntaxContext? context, in ImmutableArray<string> selectorTargets) 
        { 
            TargetsToRefresh = selectorTargets; 
            _context = context;
        }

        private readonly GeneratorAttributeSyntaxContext? _context;
        public SyntaxNode TargetNode => _context?.TargetNode;
        public ISymbol TargetSymbol => _context?.TargetSymbol;
        public SemanticModel SemanticModel => _context?.SemanticModel;

        public ImmutableArray<string> TargetsToRefresh { get; }

        public bool HasContext => _context is not null;

        public bool Any => !TargetsToRefresh.IsDefaultOrEmpty && TargetsToRefresh.Length > 0;

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
            if (context.TargetSymbol.GetAttributes().Any(a => a.AttributeClass.Name == "ObservableProperty") is false)
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
                            (builder ?? ImmutableArray.CreateBuilder<string>()).Add(name);
                    }
                }
            }
            return builder is null ? default : builder.ToImmutable();
        }

        public bool Equals(TriggersRefreshData other)
        {
            if (this.HasContext && other.HasContext)
            {
                return SymbolEqualityComparer.Default.Equals(this.TargetSymbol, other.TargetSymbol);
            }
            return this.TargetsToRefresh.SequenceEqual(other.TargetsToRefresh);
        }
    }
}
