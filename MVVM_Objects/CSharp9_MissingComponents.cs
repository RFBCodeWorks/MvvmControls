#if !NET5_0_OR_GREATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.ComponentModel;

namespace ClassLibrary
{

    public record Class(string Str)
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
    public static class IsExternalInit
    {
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#endif