using System;
using Microsoft.CodeAnalysis;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    internal readonly struct IViewModelData : IEquatable<IViewModelData>
    {
        public SyntaxNode TargetNode { get; }
        public ISymbol TargetSymbol { get; }
        public SemanticModel SemanticModel { get; }

        public IViewModelData(
            SyntaxNode syntaxNode,
            ISymbol symbol,
            SemanticModel semanticModel
            )
        {
            TargetNode = syntaxNode ?? throw new ArgumentNullException(nameof(syntaxNode));
            TargetSymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
        }

        public bool Equals(IViewModelData other) =>
            TargetNode == other.TargetNode
            && TargetSymbol.Equals(other.TargetSymbol, SymbolEqualityComparer.Default)
            && SemanticModel.Equals(other.SemanticModel)
            ;

        public override bool Equals(object obj) =>
            obj is IViewModelData other && Equals(other);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(TargetNode);
            hash.Add(TargetSymbol);
            hash.Add(SemanticModel);
            return hash.ToHashCode();
        }

        public static bool operator ==(IViewModelData left, IViewModelData right) => left.Equals(right);
        public static bool operator !=(IViewModelData left, IViewModelData right) => !left.Equals(right);
        
    }
}

