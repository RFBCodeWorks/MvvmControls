using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RFBCodeWorks.Mvvm.SourceGenerators
{
    internal static partial class SymbolFormats
    {
        /// <inheritdoc cref="SymbolDisplayFormat.FullyQualifiedFormat"/>
        public static SymbolDisplayFormat FullyQualifiedFormat => SymbolDisplayFormat.FullyQualifiedFormat;

        /// <summary>
        /// format used to get the fully qualified name for an enum such as <see cref="System.Text.RegularExpressions.RegexOptions.Compiled"/>
        /// </summary>
        public static readonly SymbolDisplayFormat EnumFullyQualified = new SymbolDisplayFormat(
            SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            memberOptions: SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeContainingType
            );

        /// <summary>
        /// Namespaces and Containing Types ( global is omitted )
        /// </summary>
        /// <remarks>
        /// <see cref="SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces"/>
        /// </remarks>
        public static readonly SymbolDisplayFormat NameAndContainingTypes = new SymbolDisplayFormat(SymbolDisplayGlobalNamespaceStyle.Omitted, SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        public static readonly SymbolDisplayFormat NameOnly = new SymbolDisplayFormat();

        /// <summary>
        /// For use with <see cref="INamespaceSymbol"/>
        /// </summary>
        /// <remarks>
        /// Outputs <see langword="namespace System.Text.Json"/>
        /// </remarks>
        public static readonly SymbolDisplayFormat NamespaceFormat = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeModifiers | SymbolDisplayMemberOptions.IncludeContainingType,
            localOptions: SymbolDisplayLocalOptions.None,
            kindOptions: SymbolDisplayKindOptions.IncludeTypeKeyword | SymbolDisplayKindOptions.IncludeNamespaceKeyword,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
            );

        /// <summary>
        /// Fully Qualified Name with "<see langword="global::"/>" omitted
        /// <br/> For use with <see cref="INamedTypeSymbol"/>
        /// </summary>
        /// <remarks>
        /// Namespace.Class.Member
        /// </remarks>
        private static readonly SymbolDisplayFormat FileNameFormat = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
            );
        private static readonly System.Text.RegularExpressions.Regex _FileNameRegex = new System.Text.RegularExpressions.Regex(@"[^\w\.]+");
        /// <summary>
        /// Transforms the fully qualified format to a file name format
        /// </summary>
        /// <returns></returns>
        public static string GetFileName(this ISymbol symbol, bool includeFileExtension = true)
        {
            string raw = symbol.ToDisplayString(FileNameFormat);
            string sanitized = _FileNameRegex.Replace(raw, "_");
            return includeFileExtension ? $"{sanitized}.g.cs" : sanitized;
        }

        /// <summary>
        /// returns 'method(args)' without type qualifiers when used on a constructor
        /// </summary>
        /// <returns>
        /// method(arg1, arg2)
        /// </returns>
        public static readonly SymbolDisplayFormat MethodInvokePrivate = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            parameterOptions: SymbolDisplayParameterOptions.IncludeName | SymbolDisplayParameterOptions.IncludeParamsRefOut,
            memberOptions: SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeRef
            );

        /// <summary>
        /// returns 'Namspace.Class.Method(args)' with type qualifiers on method name
        /// </summary>
        /// <returns>
        /// Namspace.Class.Method(arg1, arg2)
        /// </returns>
        public static readonly SymbolDisplayFormat MethodInvokeFullyQualified = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            parameterOptions: SymbolDisplayParameterOptions.IncludeName | SymbolDisplayParameterOptions.IncludeParamsRefOut,
            memberOptions: SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeRef
            );

        /// <summary>
        /// return the method name only (with explicit interface qualifiers if required)
        /// </summary>
        /// <remarks>Does not return information on parameters or return type</remarks>
        public static readonly SymbolDisplayFormat MethodName = new SymbolDisplayFormat(
            SymbolDisplayGlobalNamespaceStyle.Omitted,
            SymbolDisplayTypeQualificationStyle.NameOnly,
            SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeTypeConstraints,
            SymbolDisplayMemberOptions.IncludeExplicitInterface,
            localOptions: SymbolDisplayLocalOptions.IncludeType | SymbolDisplayLocalOptions.IncludeConstantValue | SymbolDisplayLocalOptions.IncludeRef,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
            );

        /// <summary>
        /// Format used on <see cref="IParameterSymbol"/> to output 'type name' or 'type name = defaultValue'
        /// </summary>
        public static readonly SymbolDisplayFormat MethodParameter = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            parameterOptions: SymbolDisplayParameterOptions.IncludeName | SymbolDisplayParameterOptions.IncludeType | SymbolDisplayParameterOptions.IncludeDefaultValue | SymbolDisplayParameterOptions.IncludeParamsRefOut,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
            );

        /// <summary>
        /// Format used on <see cref="IParameterSymbol"/> to output 'type name'
        /// </summary>
        public static readonly SymbolDisplayFormat VariableFormat = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            parameterOptions: SymbolDisplayParameterOptions.IncludeName | SymbolDisplayParameterOptions.IncludeType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
            );

        public static SymbolDisplayFormat PartialClassEntry { get; } = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeParameters,
            localOptions: SymbolDisplayLocalOptions.None,
            kindOptions: SymbolDisplayKindOptions.IncludeTypeKeyword,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
            );

        /// <summary>
        /// (<see langword="type"/>) <see langword="Name"/>
        /// </summary>
        public static readonly SymbolDisplayFormat PropertyNameAndExplicitInterface = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            memberOptions: SymbolDisplayMemberOptions.IncludeExplicitInterface | SymbolDisplayMemberOptions.IncludeParameters
            );
    }
}
