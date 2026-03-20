#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE0130 // Namespace does not match folder structure

using System.ComponentModel;

#if NETFRAMEWORK
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(validOn: AttributeTargets.Parameter)]
    internal class NotNullAttribute : Attribute { }
}
#endif

#if !NET5_0_OR_GREATER
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

#endif

#pragma warning restore IDE0130 // Namespace does not match folder structure
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore IDE0079 // Remove unnecessary suppression