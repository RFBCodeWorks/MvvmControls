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
        public const string QualifiedName_ComboBox = "RFBCodeWorks.Mvvm." + nameof(RFBCodeWorks.Mvvm.ComboBoxAttribute);
        public const string QualifiedName_ListBox = "RFBCodeWorks.Mvvm." + nameof(RFBCodeWorks.Mvvm.ListBoxAttribute);

        private const string GlobalQualifiedComboBox = "global::RFBCodeWorks.Mvvm.RefreshableComboBoxDefinition";
        private const string GlobalQualifiedListBox = "global::RFBCodeWorks.Mvvm.RefreshableListBoxDefinition";


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        public static bool NodeSelector(SyntaxNode node, CancellationToken token = default)
        {
            return node is MethodDeclarationSyntax method && method.AttributeLists.Count > 0;
        }

        public static DataOrDiagnostics<RefreshableSelectorData> GetInfoOrDiagnostic(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            return GetInfoOrDiagnostic(context.TargetNode, context.SemanticModel, context.TargetSymbol, context.Attributes.First(), token);
        }

        public static DataOrDiagnostics<RefreshableSelectorData> GetInfoOrDiagnostic(SyntaxNode TargetNode, SemanticModel semanticModel, ISymbol TargetSymbol, AttributeData attributeData, CancellationToken token)
        { 
            if (TargetSymbol is not IMethodSymbol symbol) return default;
            if (MvvmDiagnostics.IsNotPartialClass(TargetNode, token, out var diagnostic))
            {
                return new(diagnostic);
            }

            if (
                symbol.ReturnsVoid
                || (symbol.Parameters.Length > 0 && symbol.Parameters[0].Type.Name != nameof(CancellationToken))
                || MethodReturnTypeImplementsIList(symbol, token) is false
                )
            {
                return new(Diagnostic.Create(MvvmDiagnostics.MethodReturnTypeDoesNotImplementIList, TargetNode.GetLocation(), symbol.ToDisplayString()));
            }

            // get the type of collection
            if (!TryGetCollectionType(symbol, out string collectionType, out string itemType))
            {
                return new(Diagnostic.Create(MvvmDiagnostics.UnableToDetermineCollectionType, TargetNode.GetLocation(), symbol.ToDisplayString()));
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
                return new(Diagnostic.Create(MvvmDiagnostics.InvalidNameDescriptor, TargetNode.GetLocation(), symbol.ToDisplayString()));
            }

            return new(new RefreshableSelectorData(typeToGen, suffix, TargetNode as MethodDeclarationSyntax, symbol, semanticModel, attributeData, itemType, collectionType, sv));
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

        private static SymbolDisplayFormat CollectionTypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
            .WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.UseSpecialTypes)
            .WithLocalOptions(SymbolDisplayLocalOptions.IncludeType)
            .WithMemberOptions(SymbolDisplayMemberOptions.IncludeType);

        /// <summary>
        /// Evaluates the TargetSymbol TargetSymbol's return type and extracts:
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
                collectionType = arrayType.ToDisplayString(CollectionTypeFormat);
                itemType = arrayType.ElementType.ToDisplayString(CollectionTypeFormat);
                return true;
            }

            // Generic collection type (e.g., IList<T>, IReadOnlyList<T>)
            if (returnType is INamedTypeSymbol genType && genType.IsGenericType)
            {
                collectionType = genType.ToDisplayString(CollectionTypeFormat);
                itemType = genType.TypeArguments[0].ToDisplayString(CollectionTypeFormat);
                return true;
            }

            // Not a recognized collection
            collectionType = null;
            itemType = null;
            return false;
        }


        /// <summary>
        /// Determines whether the given TargetSymbol TargetSymbol has a valid return type:
        /// - any type implementing IList<T>
        /// - a Task<T> where T implements IList<X>
        /// </summary>
        private static bool MethodReturnTypeImplementsIList(IMethodSymbol methodSymbol, CancellationToken token)
        {
            //GeneratorExtensions.DebuggerBreak();
            var returnType = methodSymbol.ReturnType;

            // Unwrap Task<T> if necessary
            if (returnType is INamedTypeSymbol named
                && named.IsGenericType
                && named.Name == nameof(Task)
                && named.ContainingNamespace.ToDisplayString() == "System.Threading.Tasks")
            {
                returnType = named.TypeArguments[0];
            }

            if (returnType is IArrayTypeSymbol) 
                return true;

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

        const SpecialType ValidTypes = SpecialType.System_Array | SpecialType.System_Collections_Generic_IList_T;

        private static bool IsOrImplementsIList(INamedTypeSymbol typeSymbol, CancellationToken token)
        {

            if (typeSymbol.SpecialType.HasFlag(SpecialType.System_Array) || typeSymbol.SpecialType.HasFlag(SpecialType.System_Collections_Generic_IList_T))
            {
                return true;
            }
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
            if (typeSymbol.SpecialType.HasFlag(SpecialType.System_Collections_Generic_IList_T))
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
