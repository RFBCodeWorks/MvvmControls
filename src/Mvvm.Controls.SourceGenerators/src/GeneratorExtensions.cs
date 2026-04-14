using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable

namespace RFBCodeWorks.Mvvm.SourceGenerators
{
    internal static partial class GeneratorExtensions
    {
#if DEBUG
        private static bool _wasDebuggerLaunched;

        [DebuggerHidden]
        public static void DebuggerLaunch()
        {
            if (Debugger.IsAttached is false)
            {
                Debugger.Launch();
                _wasDebuggerLaunched = true;
            }
        }

        [DebuggerHidden]
        public static void DebuggerBreak()
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            else if (_wasDebuggerLaunched == false)
            {
                DebuggerLaunch();
            }

        }
#else
        public static void DebuggerLaunch() { }
        public static void DebuggerBreak() { }
#endif

        /// <summary>
        /// Returns opposite of <see cref="string.IsNullOrEmpty(string)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty(this string text) => !string.IsNullOrEmpty(text);

        /// <summary>
        /// Returns opposite of <see cref="string.IsNullOrWhiteSpace(string)"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmptyOrWhiteSpace(this string text) => !string.IsNullOrWhiteSpace(text);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SanitizeForXmlComment(this string text, string? nameSpaceToStrip = null)
        {
            if (!string.IsNullOrWhiteSpace(nameSpaceToStrip) && text.StartsWith(nameSpaceToStrip, StringComparison.OrdinalIgnoreCase))
            {
                text = text.Substring(nameSpaceToStrip!.Length).Trim('.');
            }
            return text.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "'").Trim();
        }

        /// <summary>
        /// Strips out the &lt;member&gt; nodes that are returned from <see cref="ISymbol.GetDocumentationCommentXml(CancellationToken)"/>. 
        /// </summary>
        /// <returns></returns>
        public static string SanitizeDocumentationCommentXml(this string documentationCommentXml)
        {
            if (string.IsNullOrEmpty(documentationCommentXml))
                return string.Empty;
            var startIndex = documentationCommentXml.IndexOf("<member name", StringComparison.OrdinalIgnoreCase);
            if (startIndex < 0) return documentationCommentXml;

            startIndex = documentationCommentXml.IndexOf("\">", startIndex, StringComparison.OrdinalIgnoreCase); // gets end of first <member name= "">
            

            var endIndex = documentationCommentXml.IndexOf("</member>", documentationCommentXml.Length - 20 , StringComparison.OrdinalIgnoreCase);
            if (startIndex < 0 || endIndex < 0) return documentationCommentXml;
            startIndex += 2; // move past the ">" at the end of the opening <member> tag
            var summaryContent = documentationCommentXml.Substring(startIndex, endIndex - startIndex);
            return summaryContent;
        }

        /// <summary>
        /// Iterates the attribute's constructor arguments, populating the builder if any are found.
        /// </summary>
        /// <param name="attr">The attribute whose <see cref="AttributeData.ConstructorArguments"/> should be evaluated.</param>
        /// <param name="builder">a reference to a builder. A builder will be created if the supplied reference is null.</param>
        /// <param name="token"></param>
        public static void GetStringConstructorArguments(this AttributeData attr, ref ImmutableArray<string>.Builder? builder, CancellationToken token = default)
        {
            foreach (var arg in attr.ConstructorArguments)
            {
                if (arg.Kind == TypedConstantKind.Array)
                {
                    foreach (var item in arg.Values)
                    {
                        token.ThrowIfCancellationRequested();
                        if (item.Value is string name && !string.IsNullOrWhiteSpace(name))
                            (builder ??= ImmutableArray.CreateBuilder<string>(arg.Values.Length)).Add(name);
                    }
                }
                else
                {
                    if (arg.Value is string name && !string.IsNullOrWhiteSpace(name))
                        (builder ??= ImmutableArray.CreateBuilder<string>(attr.ConstructorArguments.Length)).Add(name);
                }
            }
        }

    }
}