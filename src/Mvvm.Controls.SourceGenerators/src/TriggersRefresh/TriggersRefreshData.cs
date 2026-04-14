using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;
using System;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    internal readonly struct TriggersRefreshData : IEquatable<TriggersRefreshData>
    {
        public const string QualifiedAttributeName = "RFBCodeWorks.Mvvm." + nameof(TriggersRefreshAttribute);
        public TriggersRefreshData(in GeneratorAttributeSyntaxContext? context, in ImmutableArray<string> selectorTargets) 
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
