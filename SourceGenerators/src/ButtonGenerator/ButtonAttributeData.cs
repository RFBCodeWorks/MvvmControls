using System;
using System.Linq;
using Microsoft.CodeAnalysis;
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
        public string DisplayName { get; }
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

            // Extract DisplayName named argument, if present
            var displayNameArg = attributeData.NamedArguments.FirstOrDefault(kvp => kvp.Key == nameof(DisplayName));
            DisplayName = displayNameArg.Key != null ? displayNameArg.Value.Value as string : null;
            
            var canEx= attributeData.NamedArguments.FirstOrDefault(kvp => kvp.Key == nameof(CanExecute));
            CanExecute = canEx.Key != null ? canEx.Value.Value as string : string.Empty;

            var b = attributeData.NamedArguments.FirstOrDefault(kvp => kvp.Key == nameof(AllowConcurrentExecutions));
            AllowConcurrentExecutions = b.Key != null ? (bool)b.Value.Value : false;

            b = attributeData.NamedArguments.FirstOrDefault(kvp => kvp.Key == nameof(FlowExceptionsToTaskScheduler));
            FlowExceptionsToTaskScheduler = b.Key != null ? (bool)b.Value.Value : false;

            b = attributeData.NamedArguments.FirstOrDefault(kvp => kvp.Key == nameof(IncludeCancelCommand));
            IncludeCancelCommand = b.Key != null ? (bool)b.Value.Value : false;
        }

        public bool Equals(ButtonAttributeData other) =>
            TargetNode == other.TargetNode
            && TargetSymbol.Equals(other.TargetSymbol, SymbolEqualityComparer.Default)
            && SemanticModel.Equals(other.SemanticModel)
            && AttributeData.Equals(other.AttributeData)
            && DisplayName == other.DisplayName;

        public override bool Equals(object obj) =>
            obj is ButtonAttributeData other && Equals(other);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(TargetNode);
            hash.Add(TargetSymbol);
            hash.Add(SemanticModel);
            hash.Add(AttributeData);
            hash.Add(DisplayName);
            return hash.ToHashCode();
        }

        public static bool operator ==(ButtonAttributeData left, ButtonAttributeData right) => left.Equals(right);
        public static bool operator !=(ButtonAttributeData left, ButtonAttributeData right) => !left.Equals(right);
    }
}

