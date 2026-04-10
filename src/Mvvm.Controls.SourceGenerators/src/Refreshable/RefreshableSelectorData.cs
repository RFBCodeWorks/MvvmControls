using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    internal readonly struct RefreshableSelectorData : IEquatable<RefreshableSelectorData>
    {
        public SyntaxNode TargetNode { get; }
        public IMethodSymbol TargetSymbol { get; }
        public SemanticModel SemanticModel { get; }
        public AttributeData AttributeData { get; }
        public ITypeSymbol ElementType { get;  }
        public bool SupportsMultiSelect { get; }

        // Attribute-specific members:
        public readonly string PropertySuffix;
        public readonly string TypeToGenerate;

        public readonly bool RefreshOnInitialize;
        public readonly string CanRefresh;

        public readonly string CombinedType;
        public readonly string ElementTypeString;
        public readonly string SelectedValueType;
        public readonly string CollectionType;

        // command type
        public readonly bool IsAsync, IsCancellable;

        // misc. properties
        public readonly string ToolTip;
        public readonly string DisplayMemberPath;
        public readonly string SelectedValuePath;
        public readonly string PropertyName;

        public RefreshableSelectorData(
            string typeToGenerate,
            string propertySuffix,
            SyntaxNode syntaxNode,
            IMethodSymbol symbol,
            SemanticModel semanticModel,
            AttributeData attributeData,
            ITypeSymbol elementType,
            string collectionType, 
            string selectedValueType,
            bool isAsync, bool isCancellable
            )
        {
            PropertySuffix = propertySuffix;
            TypeToGenerate = typeToGenerate;
            TargetNode = syntaxNode ?? throw new ArgumentNullException(nameof(syntaxNode));
            TargetSymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            AttributeData = attributeData ?? throw new ArgumentNullException(nameof(attributeData));
            ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));

            // process the submitted parameters
            ElementTypeString = elementType.ToDisplayString(RefreshableSelectorParser.CollectionTypeFormat);
            CollectionType = collectionType;
            SelectedValueType = string.IsNullOrWhiteSpace(selectedValueType) ? "global::System.Object" : selectedValueType;
            CombinedType = $"<{ElementTypeString}, {CollectionType}, {SelectedValueType}>";
            IsAsync = isAsync;
            IsCancellable = isCancellable;

            // set named argument defaults
            RefreshOnInitialize = true;

            // Named Arguments
            foreach (var kvp in attributeData.NamedArguments)
            {
                switch (kvp.Key)
                {
                    case nameof(SelectorAttribute.CanRefresh):
                        CanRefresh = kvp.Value.Value as string;
                        break;

                    case nameof(SelectorAttribute.RefreshOnInitialize):
                        if (kvp.Value.Value is bool refreshBool)
                        {
                            RefreshOnInitialize = refreshBool;
                        }
                        break;

                    case nameof(SelectorAttribute.ToolTip):
                        ToolTip = kvp.Value.ToCSharpString();
                        break;

                    case nameof(SelectorAttribute.DisplayMemberPath):
                        DisplayMemberPath = kvp.Value.ToCSharpString();
                        break;

                    case nameof(SelectorAttribute.SelectedValuePath):
                        SelectedValuePath = kvp.Value.ToCSharpString();
                        break;
                    
                    case nameof(SelectorAttribute.PropertyName):
                        PropertyName = (kvp.Value.Value as string).Trim('_', ' ').Trim();
                        break;

                    case nameof(ListBoxAttribute.SupportsMultiSelect):
                        SupportsMultiSelect = (kvp.Value.Value is bool multiSelectBool) && multiSelectBool;
                        break;
                }
            }
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

