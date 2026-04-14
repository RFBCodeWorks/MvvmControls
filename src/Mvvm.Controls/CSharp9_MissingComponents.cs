#if NETFRAMEWORK
#pragma warning disable IDE0130 // Namespace does not match folder structure

using System.ComponentModel;


namespace ClassLibrary
{

    internal record Class(string Str)
    {
        internal int Int { get; init; }
    }
}

namespace System.Runtime.CompilerServices
{
    
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// This class should not be used by developers in source code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    internal sealed class MemberNotNullWhenAttribute(bool returnValue, params string[] members) : Attribute
    {
        /// <summary>Gets the return value condition.</summary>
        public bool ReturnValue { get; } = returnValue;

        /// <summary>Gets field or property member names.</summary>
        public string[] Members { get; } = members;
    }
}

#pragma warning restore IDE0130 // Namespace does not match folder structure
#endif