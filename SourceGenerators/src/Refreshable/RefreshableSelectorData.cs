using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    internal readonly struct RefreshableSelectorData : IEquatable<RefreshableSelectorData>
    {
        public SyntaxNode TargetNode { get; }
        public ISymbol TargetSymbol { get; }
        public SemanticModel SemanticModel { get; }
        public AttributeData AttributeData { get; }

        // Attribute-specific members:
        public readonly string PropertySuffix;
        public readonly string TypeToGenerate;

        public readonly bool RefreshOnInitialize;
        public readonly string CanRefresh;

        public readonly string CombinedType;
        public readonly string ItemType;
        public readonly string SelectedValueType;
        public readonly string ListType;


        public RefreshableSelectorData(
            string typeToGenerate,
            string propertySuffix,
            SyntaxNode syntaxNode,
            ISymbol symbol,
            SemanticModel semanticModel,
            AttributeData attributeData,
            string itemType, 
            string listType, 
            string selectedValueType
            )
        {
            PropertySuffix = propertySuffix;
            TypeToGenerate = typeToGenerate;
            TargetNode = syntaxNode ?? throw new ArgumentNullException(nameof(syntaxNode));
            TargetSymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            AttributeData = attributeData ?? throw new ArgumentNullException(nameof(attributeData));

            RefreshOnInitialize = true; // default
            var refreshArg = attributeData.NamedArguments.FirstOrDefault(kvp => kvp.Key == nameof(RefreshOnInitialize));
            if (refreshArg.Key != null && refreshArg.Value.Value is bool refreshBool)
                RefreshOnInitialize = refreshBool;

            var canRefreshArg = attributeData.NamedArguments.FirstOrDefault(kvp => kvp.Key == nameof(CanRefresh));
            CanRefresh = canRefreshArg.Key != null ? canRefreshArg.Value.Value as string : null;

            ItemType = itemType;
            ListType = listType;
            SelectedValueType = string.IsNullOrWhiteSpace(selectedValueType) ? "global::System.Object" : selectedValueType;
            
            CombinedType = $"<{ItemType}, {ListType}, {SelectedValueType}>";
        }

        public bool Equals(RefreshableSelectorData other) =>
            TargetNode == other.TargetNode
            && TargetSymbol.Equals(other.TargetSymbol, SymbolEqualityComparer.Default)
            && SemanticModel.Equals(other.SemanticModel)
            && AttributeData.Equals(other.AttributeData)
            && RefreshOnInitialize == other.RefreshOnInitialize
            && CanRefresh == other.CanRefresh;

        public override bool Equals(object obj) =>
            obj is RefreshableSelectorData other && Equals(other);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(TargetNode);
            hash.Add(TargetSymbol);
            hash.Add(SemanticModel);
            hash.Add(AttributeData);
            hash.Add(RefreshOnInitialize);
            hash.Add(CanRefresh);
            return hash.ToHashCode();
        }

        public static bool operator ==(RefreshableSelectorData left, RefreshableSelectorData right) => left.Equals(right);
        public static bool operator !=(RefreshableSelectorData left, RefreshableSelectorData right) => !left.Equals(right);
        
    }
}

