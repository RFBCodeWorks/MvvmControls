using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    internal static class RefreshableSelectorParser
    {
        public const string QualifiedName_ComboBox = "RFBCodeWorks.Mvvm.ComboBoxAttribute";
        public const string QualifiedName_ListBox = "RFBCodeWorks.Mvvm.ListBoxAttribute";

        private const string GlobalQualifiedComboBox = "global::" + QualifiedName_ComboBox;
        private const string GlobalQualifiedListBox = "global::" + QualifiedName_ListBox;


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        public static bool NodeSelector(SyntaxNode node, CancellationToken token = default)
        {
            return node is MethodDeclarationSyntax method && method.AttributeLists.Count > 0;
        }

#if ROSLYN_4_0_OR_GREATER
        public static IncrementalValuesProvider<GroupedCandidates<RefreshableSelectorData>> ForMethodsWithComboBoxAttribute(this IncrementalGeneratorInitializationContext context)
        {
            return GetData(context, QualifiedName_ComboBox);
        }
        public static IncrementalValuesProvider<GroupedCandidates<RefreshableSelectorData>> ForMethodsWithListBoxAttribute(this IncrementalGeneratorInitializationContext context)
        {
            return GetData(context, QualifiedName_ListBox);
        }

        private static IncrementalValuesProvider<GroupedCandidates<RefreshableSelectorData>> GetData(IncrementalGeneratorInitializationContext context, string qualifiedName)
        {
            var candidatesWithDiagnostics = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: QualifiedName_ComboBox,
                predicate: RefreshableSelectorParser.NodeSelector,
                transform: static (context, token) => RefreshableSelectorParser.GetInfoOrDiagnostic(context.TargetNode, context.SemanticModel, context.TargetSymbol, context.Attributes.First(), token)
                );

            context.ReportDiagnostics(candidatesWithDiagnostics.Where(static c => c.IsErrored).Select((c, _) => c.Diagnostics));

            return candidatesWithDiagnostics
                .Where(static c => c.IsValid)
                .Select(static (c, _) => c.Data)
                .GroupBy(static c => c.TargetSymbol.ContainingType, SymbolEqualityComparer.Default) // group by containing class
                .Select(static (c, _) => new GroupedCandidates<RefreshableSelectorData>(c.Key as INamedTypeSymbol, c.Values))
               ;
        }
#endif

        public static DataOrDiagnostics<RefreshableSelectorData> GetInfoOrDiagnostic(SyntaxNode node, SemanticModel semanticModel, ISymbol symbol, AttributeData attributeData, CancellationToken token)
        {
            if (symbol is not IMethodSymbol method) return default;
            if (MvvmDiagnostics.IsNotPartialClass(node, token, out var diagnostic))
            {
                return new(diagnostic);
            }

            if (
                method.ReturnsVoid
                || (method.Parameters.Length > 0 && method.Parameters[0].Type.Name != nameof(CancellationToken))
                || MethodReturnTypeImplementsIList(method, token) is false
                )
            {
                return new(Diagnostic.Create(MvvmDiagnostics.MethodReturnTypeDoesNotImplementIList, node.GetLocation(), symbol.ToDisplayString()));
            }

            // get the type of collection
            if (!TryGetCollectionType(method, out string collectionType, out string itemType))
            {
                return new(Diagnostic.Create(MvvmDiagnostics.UnableToDetermineCollectionType, node.GetLocation(), symbol.ToDisplayString()));
            }

            // get the selectedValue type
            var sv = GetSelectedValueType(attributeData);

            (string typeToGen, string suffix) = attributeData.AttributeClass.Name switch
            {
                "ListBoxAttribute" => (GlobalQualifiedListBox, "ListBox"),
                "ComboBoxAttribute" => (GlobalQualifiedComboBox, "ComboBox"),
                _ => ("", "")
            };

            if (typeToGen == "")
            {
                return new(Diagnostic.Create(MvvmDiagnostics.InvalidNameDescriptor, node.GetLocation(), symbol.ToDisplayString()));
            }

            return new(new RefreshableSelectorData(typeToGen, suffix, node as MethodDeclarationSyntax, symbol as IMethodSymbol, semanticModel, attributeData, itemType, collectionType, sv));
        }

        /// <summary>
        /// Returns the fully qualified name of the ListBoxAttribute.SelectedValueType or "object" if none was specified.
        /// </summary>
        private static string GetSelectedValueType(AttributeData attributeData)
        {
            foreach (var namedArg in attributeData.NamedArguments)
            {
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

        /// <summary>
        /// Evaluates the method symbol's return type and extracts:
        /// - collectionType: fully qualified collection type name (unwrapped Task if present)
        /// - itemType:   fully qualified element/item type name
        /// - valueType:  fully qualified original return type name
        /// </summary>
        private static bool TryGetCollectionType(IMethodSymbol symbol, out string collectionType, out string itemType)
        {
            // Original return type string
            var returnType = symbol.ReturnType;

            // Unwrap Task<T> if necessary
            if (returnType is INamedTypeSymbol named
                && named.IsGenericType
                && named.Name == nameof(Task)
                && named.ContainingNamespace.ToDisplayString() == "System.Threading.Tasks")
            {
                returnType = named.TypeArguments[0];
            }

            // Array type
            if (returnType is IArrayTypeSymbol arrayType)
            {
                collectionType = arrayType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                itemType = arrayType.ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                return true;
            }

            // Generic collection type (e.g., IList<T>, IReadOnlyList<T>)
            if (returnType is INamedTypeSymbol genType && genType.IsGenericType)
            {
                collectionType = genType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                itemType = genType.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                return true;
            }

            // Not a recognized collection
            collectionType = null;
            itemType = null;
            return false;
        }

        /// <summary>
        /// Determines whether the given method symbol has a valid return type:
        /// - any type implementing IList<T>
        /// - a Task<T> where T implements IList<X>
        /// </summary>
        private static bool MethodReturnTypeImplementsIList(IMethodSymbol methodSymbol, CancellationToken token)
        {
            var returnType = methodSymbol.ReturnType;
            if (returnType is not INamedTypeSymbol namedType)
                return false;

            // Direct IList<T> or implementing IList<T>
            if (IsOrImplementsIList(namedType, token))
                return true;

            // Task<T> where T implements IList<X>
            if (namedType.IsGenericType
                && (namedType.Name.StartsWith(nameof(Task))//|| namedType.Name == nameof(ValueTask))
                && namedType.ContainingNamespace.ToDisplayString() == "System.Threading.Tasks"))
            {
                var innerType = namedType.TypeArguments[0];
                if (innerType is INamedTypeSymbol innerNamed && IsOrImplementsIList(innerNamed, token))
                    return true;
            }

            return false;
        }

        private static bool IsOrImplementsIList(INamedTypeSymbol typeSymbol, CancellationToken token)
        {
            if (IsIList(typeSymbol))
            {
                return true;
            }

            // Check any implemented IList<T> interface
            foreach (var iface in typeSymbol.AllInterfaces)
            {
                token.ThrowIfCancellationRequested();
                if (IsIList(iface))
                    return true;
            }
            return false;
        }

        private static bool IsIList(INamedTypeSymbol typeSymbol)
        {
            return typeSymbol.IsGenericType
                    && typeSymbol.TypeParameters.Length == 1
                    && typeSymbol.Name.StartsWith("IList")
                    && typeSymbol.ContainingNamespace.ToDisplayString() == "System.Collections.Generic";
            
        }
    }
}
