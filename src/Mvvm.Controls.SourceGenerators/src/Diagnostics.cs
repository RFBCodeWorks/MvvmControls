using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm.SourceGenerators
{
    internal static partial class Diagnostics
    {
        internal static Diagnostic CreateExceptionDiagnostic(Exception e, Location? location)
        {
            return Diagnostic.Create(
                id: "RFB_MVVM_000",
                title: $"Generator Exception : {e.Message}",
                message: e.StackTrace,
                category: "RFBCodeWorks.Mvvm.SourceGenerators",
                severity: DiagnosticSeverity.Error,
                defaultSeverity: DiagnosticSeverity.Warning,
                isSuppressed: false,
                isEnabledByDefault: true,
                warningLevel: 4,
                location: location
            );
        }

        /// <remarks>
        /// Message Parameters:
        /// <br/> None
        /// </remarks>
        public static DiagnosticDescriptor UnknownTargetFramework { get; } =
           new("RFB_MVVM_001", "Unknown target framework",
               "Could not determine target framework for compatibility check",
               "RegexGeneration", DiagnosticSeverity.Warning, isEnabledByDefault: true);

        /// <remarks>
        /// Message Parameters:
        /// <br/> None
        /// </remarks>
        public static DiagnosticDescriptor UnsupportedLanguage { get; } =
           new("RFB_MVVM_002", "Unsupported Language",
               "This source generator only supports C#",
               "RegexGeneration", DiagnosticSeverity.Error, isEnabledByDefault: true);

        /// <remarks>
        /// Message Parameters:
        /// <br/> - Minimum Language Version
        /// </remarks>
        public static DiagnosticDescriptor LanguageVersionTooLow { get; } =
           new("RFB_MVVM_003", "Language version too low",
               "This source generator only supports C# {0} and newer",
               "RegexGeneration", DiagnosticSeverity.Warning, isEnabledByDefault: true);
        
        /// <remarks>
        /// Message Parameters:
        /// <br/> None
        /// </remarks>
        public static DiagnosticDescriptor ClassIsNotPartial { get; } =
            new DiagnosticDescriptor(
                    id: "RFB_MVVM_004",
                    title: "Class is not partial",
                    messageFormat: "Class must have 'partial' keyword to support code generation",
                    category: "SourceGenerator",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true);

        /// <summary>
        /// "The method {0} cannot be used to generate a command property, as its signature isn't compatible with any of the existing relay command types"
        /// </summary>
        /// <remarks>
        /// Message Parameters:
        /// <br/> - Method Name
        /// </remarks>
        public static DiagnosticDescriptor RelayCommandArgumentsInvalid { get; } =
            new DiagnosticDescriptor(
                    id: "RFB_MVVM_005",
                    title: "Invalid Arguments",
                    messageFormat: "The method {0} cannot be used to generate a command property, as its signature isn't compatible with any of the existing relay command types",
                    category: "SourceGenerator",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true);

        /// <summary>
        /// "The method {0} cannot be used to generate a {1} property. Return type must implement IList{T}, Task{IList{T}}, or ValueTask{IList{T}}"
        /// </summary>
        /// <remarks>
        /// Message Parameters:
        /// <br/> - Method Signature
        /// <br/> - Property Type being Generated
        /// </remarks>
        public static DiagnosticDescriptor MethodReturnTypeDoesNotImplementIList { get; } =
            new DiagnosticDescriptor(
                    id: "RFB_MVVM_006",
                    title: "Invalid Arguments",
                    messageFormat: "The method {0} cannot be used to generate a {1} property. Return type must implement IList<T>, Task<IList<T>>, or ValueTask<IList<T>>",
                    category: "SourceGenerator",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true);

        /// <summary>
        /// "A fully qualfied name was unable to be generated for '{0}'"
        /// </summary>
        /// <remarks>
        /// Message Parameters:
        /// <br/> - Symbol Text
        /// </remarks>
        public static readonly DiagnosticDescriptor InvalidNameDescriptor =
            new DiagnosticDescriptor(
                id: "RFB_MVVM_007",
                title: "Invalid name",
                messageFormat: "A fully qualfied name was unable to be generated for '{0}'",
                category: "SourceGenerator",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        /// <summary>
        /// "Unable to determine return collection type for '{0}'"
        /// </summary>
        /// <remarks>
        /// Message Parameters:
        /// <br/> - Method Signature
        /// </remarks>
        public static readonly DiagnosticDescriptor UnableToDetermineCollectionType =
            new DiagnosticDescriptor(
                id: "RFB_MVVM_008",
                title: "Unable to determine collection type",
                messageFormat: "Unable to determine return collection type for '{0}'",
                category: "SourceGenerator",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        /// <summary>
        ///  "Field '{0}' is marked with [TriggersRefresh] but is not an [ObservableProperty]. You must manually refresh the specified selectors."
        /// </summary>
        /// <remarks>
        /// Message Parameters:
        /// <br/> None
        /// </remarks>
        public static readonly DiagnosticDescriptor InvalidTriggersRefreshUsage = new DiagnosticDescriptor(
            id: "RFB_MVVM_009",
            title: "TriggersRefreshAttribute requires ObservableProperty",
            messageFormat: "Field '{0}' is marked with [TriggersRefresh] but is not an [ObservableProperty]. You must manually refresh the specified selectors.",
            category: "RFBCodeWorks.Mvvm.SourceGenerators",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "When applied to a field, the TriggersRefreshAttribute requires the field to be marked with [ObservableProperty] so that a partial OnValueChanged method is generated. Without it, the selectors will not be automatically refreshed."
        );

        /// <summary>
        /// "Field '{0}' is not marked as private"
        /// </summary>
        /// <remarks>
        /// Message Parameters:
        /// <br/> - Field Name
        /// </remarks>
        public static readonly DiagnosticDescriptor ObservableFieldIsNotPrivate = new DiagnosticDescriptor(
            id: "RFB_MVVM_010",
            title: "ObservableProperty field is not private",
            messageFormat: "Field '{0}' is not marked as private",
            category: "RFBCodeWorks.Mvvm.SourceGenerators",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        /// <summary>
        ///  "Unable to generate {0} : Method '{1}' has too many parameters"
        /// </summary>
        /// <remarks>
        /// Message Parameters:
        /// <br/> - What is being generated
        /// <br/> - Method Name
        /// </remarks>
        public static DiagnosticDescriptor TooManyParameters = new DiagnosticDescriptor(
             id: "RFB_MVVM_011",
             title: "Invalid Parameter List for Refreshable Generator",
             messageFormat: "Unable to generate {0} : Method '{1}' has too many parameters",
             category: "RFBCodeWorks.Mvvm.SourceGenerators",
             defaultSeverity: DiagnosticSeverity.Error,
             isEnabledByDefault: true
        );

        /// <summary>
        /// "Unable to generate {0} : Method {1} has an invalid parameter of type : {2}"
        /// </summary>
        /// <remarks>
        /// Message Parameters:
        /// <br/> - What is being generated
        /// <br/> - Method Name
        /// <br/> - Parameter Type FullName
        /// </remarks>
        public static DiagnosticDescriptor ParameterIsNotCancellationToken = new DiagnosticDescriptor(
             id: "RFB_MVVM_012",
             title: "Async Refreshable parameter must be CancellationToken or none",
             messageFormat: "Unable to generate {0} : Method {1} has an invalid parameter of type : {2}",
             category: "RFBCodeWorks.Mvvm.SourceGenerators",
             defaultSeverity: DiagnosticSeverity.Error,
             isEnabledByDefault: true
        );

        public static DiagnosticDescriptor ClassDoesNotImplementInterface = new DiagnosticDescriptor(
             id: "RFB_MVVM_013",
             title: "Interface Required",
             messageFormat: "The class '{0}' must implement interface '{1}' to support this generator",
             category: "RFBCodeWorks.Mvvm.SourceGenerators",
             defaultSeverity: DiagnosticSeverity.Error,
             isEnabledByDefault: true
        );


        /// <summary>
        /// Tries to clean a symbol's name by stripping known prefixes/suffixes
        /// and producing valid field and property identifiers.
        /// </summary>
        /// <param name="symbol">Symbol whose name to clean.</param>
        /// <param name="fieldName">Resulting field name (e.g. _myValue).</param>
        /// <param name="propertyName">Resulting property name (e.g. MyValue).</param>
        /// <param name="diagnostic">Diagnostic if cleaning fails.</param>
        /// <returns>Returns null if successful, otherwise returns a diagnostic.</returns>
        public static Diagnostic? TryGetPropertyName(IMethodSymbol symbol, string suffixToAppend, out ReadOnlySpan<char> fieldName, out ReadOnlySpan<char> propertyName)
        {
            fieldName = null;
            propertyName = null;

            if (symbol is null || string.IsNullOrWhiteSpace(symbol.Name) || symbol.Name.Contains(' '))
            {
                var loc = symbol?.Locations.FirstOrDefault() ?? Location.None;
                return Diagnostic.Create(InvalidNameDescriptor, loc, symbol?.Name ?? string.Empty);
            }

            // prefixes
            ReadOnlySpan<char> cleaned = symbol.Name.AsSpan().Trim('_').Trim();
            if (cleaned.Length > 2 && cleaned.StartsWith("On".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Slice(2).TrimStart('_');
            }

            if (suffixToAppend != "Button")
            {
                if (cleaned.Length > 3 && cleaned.StartsWith("Get".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    cleaned = cleaned.Slice(3).TrimStart('_');
                }
                if (cleaned.Length > 7 && cleaned.StartsWith("Refresh".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    cleaned = cleaned.Slice(7).TrimStart('_');
                }
            }

            // suffixes
            if (cleaned.Length > 5 && cleaned.EndsWith("Async".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Slice(0, cleaned.Length - 5).TrimEnd('_');
            }
            if (cleaned.Length > 7 && cleaned.EndsWith("Command".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Slice(0, cleaned.Length - 7).TrimEnd('_');
            }
            if (cleaned.Length > 7 && cleaned.EndsWith("Func".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Slice(0, cleaned.Length - 4).TrimEnd('_');
            }
            cleaned = cleaned.Trim();

            if (cleaned.IsEmpty || !char.IsLetter(cleaned[0]))
            {
                var loc = symbol.Locations.FirstOrDefault() ?? Location.None;
                return Diagnostic.Create(InvalidNameDescriptor, loc, symbol.Name);
            }

            //string subString = cleaned.Length > 1 ? cleaned.Substring(1) : string.Empty;

            // PascalCase property
            //propertyName = $"{char.ToUpperInvariant(cleaned[0])}{subString}{suffixToAppend}";
            Span<char> pn = new char[cleaned.Length + suffixToAppend.Length];
            cleaned.CopyTo(pn);
            pn[0] = char.ToUpperInvariant(cleaned[0]);
            suffixToAppend.AsSpan().CopyTo(pn.Slice(cleaned.Length));
            propertyName = pn;

            // _camelCase field
            //fieldName = $"_{char.ToLowerInvariant(cleaned[0])}{subString}{suffixToAppend}";
            Span<char> fn = new char[cleaned.Length + suffixToAppend.Length + 1];
            fn[0] = '_';
            cleaned.CopyTo(fn.Slice(1));
            fn[1] = char.ToLowerInvariant(cleaned[0]);
            suffixToAppend.AsSpan().CopyTo(fn.Slice(cleaned.Length + 1));
            fieldName = fn;
            
            return null;
        }

        /// <summary>
        /// Attempts to get the property name that would be generated by CommunityToolkit.Mvvm's [ObservableProperty] for the given field.
        /// </summary>
        /// <param name="fieldSymbol">The backing field symbol.</param>
        /// <param name="propertyName">The inferred property name.</param>
        /// <returns>True if the field is valid for [ObservableProperty]; otherwise false.</returns>
        /// <returns>Returns null if successful, otherwise returns a diagnostic.</returns>
        public static Diagnostic? TryGetPropertyName(IFieldSymbol fieldSymbol, string suffixToAppend, out string propertyName)
        {
            propertyName = null;

            if (fieldSymbol is null || fieldSymbol.DeclaredAccessibility != Accessibility.Private)
                return Diagnostic.Create(ObservableFieldIsNotPrivate, fieldSymbol.Locations.FirstOrDefault() ?? Location.None, fieldSymbol.Name);

            if (fieldSymbol.Name.Length > 0)
            {
                string name = fieldSymbol.Name.TrimStart('_').TrimEnd('_');
                if (name.Length > 0)
                {
                    char u = char.ToUpperInvariant(name[0]);
                    if (char.IsLetter(u) && name[0] != u)
                    {
                        propertyName = name.Length == 1 ? $"{u}{suffixToAppend}": $"{u}{name.Substring(1)}{suffixToAppend}";
                        return null;
                    }
                }
            }
            return Diagnostic.Create(InvalidNameDescriptor, fieldSymbol.Locations.FirstOrDefault() ?? Location.None, fieldSymbol.Name); 
        }

        internal static bool IsLanguageVersionTooLow(LanguageVersion compilation, LanguageVersion required, out Diagnostic? diagnostic, Func<Location> getLocation)
        {
            if (compilation < required)
            {
                diagnostic = Diagnostic.Create(LanguageVersionTooLow, getLocation(), required);
                return true;
            }
            diagnostic = null;
            return false;
        }

        internal static bool IsNotPartialClass(this SyntaxNode node, CancellationToken token, out Diagnostic? diagnostic)
        {
            var result = node.IsPartialClass(token);
            diagnostic = result ? null : Diagnostic.Create(ClassIsNotPartial, node.GetLocation());
            return !result;
        }

        internal static bool DoesNotImplementInterface(this ITypeSymbol typeSymbol, string interfaceFullName, CancellationToken token, out Diagnostic? diagnostic)
        {
            foreach (var iface in typeSymbol.AllInterfaces)
            {
                token.ThrowIfCancellationRequested();
                if (iface.ToDisplayString() == interfaceFullName)
                {
                    diagnostic = null;
                    return false;
                }
            }
            diagnostic = Diagnostic.Create(ClassDoesNotImplementInterface, typeSymbol.Locations.FirstOrDefault() ?? Location.None, typeSymbol.Name, interfaceFullName);
            return true;
        }

        internal static bool IsPartialClass(this SyntaxNode node, CancellationToken token)
        {
            node = GetClassDeclarationSyntax(node, token);
            bool nodeFound = node is not null;
            while (node is ClassDeclarationSyntax c)
            {
                if (c.Modifiers.Any(SyntaxKind.PartialKeyword) == false)
                {
                    return false;
                }
                node = node.Parent;
                token.ThrowIfCancellationRequested();
            }
            return nodeFound;
        }

        internal static ClassDeclarationSyntax? GetClassDeclarationSyntax(this SyntaxNode node, CancellationToken token)
        {
            while (node is not null && node is not NamespaceDeclarationSyntax)
            {
                token.ThrowIfCancellationRequested();
                if (node is ClassDeclarationSyntax c) return c;
                node = node.Parent;
            }
            return null;
        }
    }
}
