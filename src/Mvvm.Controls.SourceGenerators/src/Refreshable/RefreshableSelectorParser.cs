using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    internal static class RefreshableSelectorParser
    {
        public const string QualifiedName_ComboBox = "RFBCodeWorks.Mvvm." + nameof(RFBCodeWorks.Mvvm.ComboBoxAttribute);
        public const string QualifiedName_ListBox = "RFBCodeWorks.Mvvm." + nameof(RFBCodeWorks.Mvvm.ListBoxAttribute);
        public const string QualifiedName_Selector = "RFBCodeWorks.Mvvm." + nameof(RFBCodeWorks.Mvvm.SelectorAttribute);

        public static readonly HashSet<string> QualifiedAttributes = [QualifiedName_Selector, QualifiedName_ListBox, QualifiedName_ComboBox];

        private const string GlobalQualifiedComboBox = "global::RFBCodeWorks.Mvvm.RefreshableComboBoxDefinition";
        private const string GlobalQualifiedListBox = "global::RFBCodeWorks.Mvvm.RefreshableListBoxDefinition";
        private const string GlobalQualifiedSelector = "global::RFBCodeWorks.Mvvm.RefreshableSelector";

        private static readonly SymbolDisplayFormat CollectionTypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
            .WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.UseSpecialTypes)
            .WithLocalOptions(SymbolDisplayLocalOptions.IncludeType)
            .WithMemberOptions(SymbolDisplayMemberOptions.IncludeType);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        public static bool NodeSelector(SyntaxNode node, CancellationToken token = default)
        {
            return node is MethodDeclarationSyntax method && method.AttributeLists.Count > 0;
        }

        /// <inheritdoc cref="GetInfoOrDiagnostic(SyntaxNode, SemanticModel, ISymbol, AttributeData, CancellationToken)"/>
        public static DataOrDiagnostics<RefreshableSelectorData> GetInfoOrDiagnostic(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            return GetInfoOrDiagnostic(context.TargetNode, context.SemanticModel, context.TargetSymbol, context.Attributes.First(), token);
        }

        private static string GetDiagTypeToGen(this string typeToGen) => typeToGen.Substring(typeToGen.LastIndexOf('.') + 1);

        /// <summary>
        /// Generates any diagnostic Data for a <see cref="RefreshableSelectorData"/> struct
        /// </summary>
        public static DataOrDiagnostics<RefreshableSelectorData> GetInfoOrDiagnostic(SyntaxNode targetNode, SemanticModel semanticModel, ISymbol TargetSymbol, AttributeData attributeData, CancellationToken token)
        { 
            if (TargetSymbol is not IMethodSymbol symbol) return default;
            if (Diagnostics.IsNotPartialClass(targetNode, token, out var diagnostic))
            {
                return new(diagnostic);
            }

            (string typeToGenerate, string suffix) = attributeData.AttributeClass.Name switch
            {
                nameof(SelectorAttribute) => (GlobalQualifiedSelector, "Selector"),
                nameof(ListBoxAttribute) => (GlobalQualifiedListBox, "ListBox"),
                nameof(ComboBoxAttribute) => (GlobalQualifiedComboBox, "ComboBox"),
                _ => ("", "")
            };

            if (typeToGenerate == "")
            {
                return new(Diagnostic.Create(Diagnostics.InvalidNameDescriptor, targetNode.GetLocation(), symbol.ToDisplayString()));
            }

            bool isCancellable;
            bool isAsync = false;
            var returnType = symbol.ReturnType;

            if (symbol.ReturnsVoid)
            {
                return new(Diagnostic.Create(Diagnostics.MethodReturnTypeDoesNotImplementIList, targetNode.GetLocation(), symbol.ToDisplayString(), typeToGenerate.GetDiagTypeToGen()));
            }

            // Unwrap Task<T> if necessary
            if (returnType is INamedTypeSymbol taskUnwrapper
              && taskUnwrapper.IsGenericType
              && (taskUnwrapper.Name == nameof(Task) || taskUnwrapper.Name == nameof(ValueTask))
              && taskUnwrapper.ContainingNamespace.ToDisplayString(SymbolFormats.NameAndContainingTypes) == "System.Threading.Tasks")
            {
                isAsync = true;
                returnType = taskUnwrapper.TypeArguments[0];
            }

            // evaluate parameters
            switch (symbol.Parameters.Length)
            {
                case 0:
                    isCancellable = false;
                    break;

                case 1 when isAsync:
                    var param = symbol.Parameters[0].Type.ToDisplayString(SymbolFormats.NameAndContainingTypes);
                    if (param == typeof(CancellationToken).FullName)
                    {
                        isCancellable = true;
                    }
                    else
                    {
                        return new(Diagnostic.Create(Diagnostics.ParameterIsNotCancellationToken, targetNode.GetLocation(), typeToGenerate, symbol.Name, param));
                    }
                    break;

                default:
                    return new (Diagnostic.Create(Diagnostics.TooManyParameters, targetNode.GetLocation(), typeToGenerate.GetDiagTypeToGen(), symbol.ToDisplayString(SymbolFormats.MethodInvokeFullyQualified)));
            }

            // get the selectedValue type
            var selectedValueType = GetSelectedValueType(attributeData, token);

            /*
             *  Evaluate return type
             */

            // Array type
            if (returnType is IArrayTypeSymbol arrayType)
            {
                return new(new RefreshableSelectorData(
                    typeToGenerate,
                    suffix,
                    targetNode as MethodDeclarationSyntax,
                    symbol, semanticModel, attributeData,
                    arrayType.ElementType.ToDisplayString(CollectionTypeFormat),
                    arrayType.ToDisplayString(CollectionTypeFormat),
                    selectedValueType,
                    isAsync, isCancellable
                    ));
            }

            // validate that the return type implements IList<T>
            if (returnType is not INamedTypeSymbol namedReturnType || IsOrImplementsIList(namedReturnType, token, out ITypeSymbol elementType) is false)
            {
                return new(Diagnostic.Create(Diagnostics.MethodReturnTypeDoesNotImplementIList, targetNode.GetLocation(), symbol.ToDisplayString(), typeToGenerate.GetDiagTypeToGen()));
            }

            // The return type implements IList, and we now know the element type
            return new(new RefreshableSelectorData(
                    typeToGenerate,
                    suffix,
                    targetNode as MethodDeclarationSyntax,
                    symbol, semanticModel, attributeData,
                    elementType.ToDisplayString(CollectionTypeFormat),
                    namedReturnType.ToDisplayString(CollectionTypeFormat),
                    selectedValueType,
                    isAsync, isCancellable
                    ));
        }

        /// <summary>
        /// Returns the fully qualified name of the ListBoxAttribute.SelectedValueType or "object" if none was specified.
        /// </summary>
        private static string GetSelectedValueType(AttributeData attributeData, CancellationToken token)
        {
            foreach (var namedArg in attributeData.NamedArguments)
            {
                token.ThrowIfCancellationRequested();
                if (namedArg.Key == "SelectedValueType")
                {
                    if (namedArg.Value.Value is ITypeSymbol typeSymbol)
                    {
                        return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    }
                }
            }
            return "object";
        }

        private static bool IsOrImplementsIList(INamedTypeSymbol typeSymbol, CancellationToken token, out ITypeSymbol elementType)
        {
            if (IsIList(typeSymbol))
            {
                elementType = typeSymbol.TypeArguments[0];
                return true;
            }

            // Check any implemented IList<T> interface
            foreach (var iface in typeSymbol.AllInterfaces)
            {
                token.ThrowIfCancellationRequested();
                if (IsIList(iface))
                {
                    elementType = iface.TypeArguments[0];
                    return true;
                }
            }
            elementType = null;
            return false;
        }

        private static bool IsIList(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.SpecialType.HasFlag(SpecialType.System_Collections_Generic_IList_T) || typeSymbol.SpecialType.HasFlag(SpecialType.System_Collections_Generic_IReadOnlyList_T))
            {
                return true;
            }
            return typeSymbol.IsGenericType
                    && typeSymbol.TypeParameters.Length == 1
                    && typeSymbol.Name.StartsWith("IList")
                    && typeSymbol.ContainingNamespace.ToDisplayString() == "System.Collections.Generic";
            
        }
    }
}
