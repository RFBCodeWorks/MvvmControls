using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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
        public static string SanitizeForXmlComment(this string text)
        {
            return text.Replace("<", "&lt;").Replace(">", "&lt;").Replace("\"", "'").Trim();
        }

    }
}