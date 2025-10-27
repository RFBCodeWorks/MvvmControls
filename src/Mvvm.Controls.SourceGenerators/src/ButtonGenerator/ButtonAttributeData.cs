using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RFBCodeWorks.Mvvm.SourceGenerators.ButtonGenerator
{
    internal readonly struct ButtonAttributeData : IEquatable<ButtonAttributeData>
    {
        public const string QualifiedName = "RFBCodeWorks.Mvvm." + nameof(RFBCodeWorks.Mvvm.ButtonAttribute);

        public MethodDeclarationSyntax TargetNode { get; }
        public IMethodSymbol TargetSymbol { get; }
        public SemanticModel SemanticModel { get; }
        public AttributeData AttributeData { get; }

        // Attribute-specific members:
        public string DisplayText { get; }
        public string ToolTip { get; }
        public string CanExecute { get; }
        public bool AllowConcurrentExecutions { get; }
        public bool FlowExceptionsToTaskScheduler { get; }
        public bool IncludeCancelCommand { get; }

        public ButtonAttributeData(
            MethodDeclarationSyntax syntaxNode,
            IMethodSymbol symbol,
            SemanticModel semanticModel,
            AttributeData attributeData)
        {
            TargetNode = syntaxNode ?? throw new ArgumentNullException(nameof(syntaxNode));
            TargetSymbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            AttributeData = attributeData ?? throw new ArgumentNullException(nameof(attributeData));

            foreach (var kvp in attributeData.NamedArguments)
            {
                switch (kvp.Key)
                {
                    case nameof(ButtonAttribute.DisplayText):
                        DisplayText = kvp.Value.ToCSharpString();
                        break;

                    case nameof(ButtonAttribute.Tooltip):
                        ToolTip = kvp.Value.ToCSharpString();
                        break;

                    case nameof(ButtonAttribute.CanExecute):
                        CanExecute = kvp.Value.Value as string ?? string.Empty;
                        break;

                    case nameof(AllowConcurrentExecutions):
                        AllowConcurrentExecutions = (bool)kvp.Value.Value;
                        break;

                    case nameof(FlowExceptionsToTaskScheduler):
                        FlowExceptionsToTaskScheduler = (bool)kvp.Value.Value;
                        break;

                    case nameof(IncludeCancelCommand):
                        IncludeCancelCommand = (bool)kvp.Value.Value;
                        break;

                }
            }
        }

        public bool Equals(ButtonAttributeData other) =>
            TargetNode == other.TargetNode
            && TargetSymbol.Equals(other.TargetSymbol, SymbolEqualityComparer.Default)
            && SemanticModel.Equals(other.SemanticModel)
            && AttributeData.Equals(other.AttributeData)
            && DisplayText == other.DisplayText;

        public override bool Equals(object obj) =>
            obj is ButtonAttributeData other && Equals(other);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(TargetNode);
            hash.Add(TargetSymbol);
            hash.Add(SemanticModel);
            hash.Add(AttributeData);
            hash.Add(DisplayText);
            return hash.ToHashCode();
        }

        public static bool operator ==(ButtonAttributeData left, ButtonAttributeData right) => left.Equals(right);
        public static bool operator !=(ButtonAttributeData left, ButtonAttributeData right) => !left.Equals(right);
    }
}

