using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm.SourceGenerators
{
    internal static class MvvmDiagnostics
    {
        public static DiagnosticDescriptor UnknownTargetFramework { get; } =
           new("RFB_MVVM_000", "Unknown target framework",
               "Could not determine target framework for compatibility check",
               "RegexGeneration", DiagnosticSeverity.Warning, isEnabledByDefault: true);

        public static DiagnosticDescriptor UnsupportedLanguage { get; } =
           new("RFB_MVVM_001", "Unsupported Language",
               "This source generator only supports C#",
               "RegexGeneration", DiagnosticSeverity.Error, isEnabledByDefault: true);

        public static DiagnosticDescriptor LanguageVersionTooLow { get; } =
           new("RFB_MVVM_002", "Language version too low",
               "This source generator only supports C# {0} and newer",
               "RegexGeneration", DiagnosticSeverity.Warning, isEnabledByDefault: true);

        public static DiagnosticDescriptor ClassIsNotPartial { get; } =
            new DiagnosticDescriptor(
                    id: "RFB_MVVM_003",
                    title: "Class is not partial",
                    messageFormat: "Class must have 'partial' keyword to support code generation",
                    category: "SourceGenerator",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true);

        public static DiagnosticDescriptor RelayCommandArgumentsInvalid { get; } =
            new DiagnosticDescriptor(
                    id: "RFB_MVVM_004",
                    title: "Invalid Arguments",
                    messageFormat: "The method {0} cannot be used to generate a command property, as its signature isn't compatible with any of the existing relay command types",
                    category: "SourceGenerator",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true);

        public static DiagnosticDescriptor MethodReturnTypeDoesNotImplementIList { get; } =
            new DiagnosticDescriptor(
                    id: "RFB_MVVM_005",
                    title: "Invalid Arguments",
                    messageFormat: "The method {0} cannot be used to generate a {1} property. Return type must implement IList<T> Task<IList<T>>, or ValueTask<IList<T>>",
                    category: "SourceGenerator",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor InvalidNameDescriptor =
            new DiagnosticDescriptor(
                id: "RFB_MVVM_006",
                title: "Invalid name",
                messageFormat: "Unable to generate names from '{0}'",
                category: "SourceGenerator",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor UnableToDetermineCollectionType =
            new DiagnosticDescriptor(
                id: "RFB_MVVM_007",
                title: "Unable to determine collection type",
                messageFormat: "A fully qualfied name was unable to be generated for {0}",
                category: "SourceGenerator",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public static bool IsNotEmpty(this string text) => !string.IsNullOrWhiteSpace(text);

        /// <summary>
        /// Tries to clean a symbol's name by stripping known prefixes/suffixes
        /// and producing valid field and property identifiers.
        /// </summary>
        /// <param name="symbol">Symbol whose name to clean.</param>
        /// <param name="fieldName">Resulting field name (e.g. _myValue).</param>
        /// <param name="propertyName">Resulting property name (e.g. MyValue).</param>
        /// <param name="diagnostic">Diagnostic if cleaning fails.</param>
        /// <returns>True if cleaning succeeded; otherwise false.</returns>
        public static bool TryCleanName(ISymbol symbol, string suffixToAppend, out string fieldName, out string propertyName, out Diagnostic diagnostic)
        {
            fieldName = null;
            propertyName = null;
            diagnostic = null;

            if (symbol is null || string.IsNullOrWhiteSpace(symbol.Name) || symbol.Name.Contains(' '))
            {
                var loc = symbol?.Locations.FirstOrDefault() ?? Location.None;
                diagnostic = Diagnostic.Create(InvalidNameDescriptor, loc, symbol?.Name ?? string.Empty);
                return false;
            }

            // prefixes
            string cleaned = symbol.Name.Trim('_').Trim();
            if (cleaned.Length > 2 && cleaned.StartsWith("On", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(2).TrimStart('_');
            }
            if (cleaned.Length > 3 && cleaned.StartsWith("Get", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(3).TrimStart('_');
            }
            if (cleaned.Length > 7 && cleaned.StartsWith("Refresh", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(7).TrimStart('_');
            }

            // suffixes
            if (cleaned.Length > 5 && cleaned.EndsWith("Async", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(0, cleaned.Length - 5).TrimEnd('_');
            }
            if (cleaned.Length > 7 && cleaned.EndsWith("Command", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(0, cleaned.Length - 7).TrimEnd('_');
            }
            if (cleaned.Length > 7 && cleaned.EndsWith("Func", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(0, cleaned.Length - 4).TrimEnd('_');
            }

            if (string.IsNullOrWhiteSpace(cleaned) || !char.IsLetter(cleaned[0]))
            {
                var loc = symbol.Locations.FirstOrDefault() ?? Location.None;
                diagnostic = Diagnostic.Create(InvalidNameDescriptor, loc, symbol.Name);
                return false;
            }

            string subString = cleaned.Length > 1 ? cleaned.Substring(1) : string.Empty;

            // PascalCase property
            propertyName = $"{char.ToUpperInvariant(cleaned[0])}{subString}{suffixToAppend}";
            // _camelCase field
            fieldName = $"_{char.ToLowerInvariant(cleaned[0])}{subString}{suffixToAppend}";

            return true;
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
