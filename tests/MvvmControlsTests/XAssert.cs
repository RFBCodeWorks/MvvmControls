
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Diagnostics.CodeAnalysis
{
#if NETFRAMEWORK
    [AttributeUsage(validOn: AttributeTargets.Parameter)]
    internal class NotNullAttribute : Attribute {}
#endif
}


namespace RFBCodeWorks.Mvvm.Tests
{
    /// <summary>
    /// Wrapper class for throwing customized messages with XUnit
    /// </summary>
    internal static class XAssert
    {
        public static void AssertIsNotNull([NotNull] this object? obj) => Assert.IsNotNull(obj);
        public static void AssertIsNull(this object? obj) => Assert.IsNull(obj);
        public static void AssertIsOfType<T>(this object? obj) => Assert.IsInstanceOfType(obj, typeof(T));

        public static Task ThrowAsync<T>(this Func<Task> func, string? message = "") 
            where T : Exception
            => ThrowsAsync<T>(func, message);

        public static Task ThrowsAsync<T>(this Func<Task> func, string? message = "")
            where T : Exception
        {
            return Assert.ThrowsAsync<T>(func, message);
        }
    }
}
