
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks
{
    /// <summary>
    /// Wrapper class for throwing customized messages with XUnit
    /// </summary>
    internal static class XAssert
    {
        public static void AssertIsNotNull([NotNull] this object? obj) => Assert.IsNotNull(obj);
        public static void AssertIsNull(this object? obj) => Assert.IsNull(obj);
        public static T AssertIsOfType<T>(this object? obj) => Assert.IsInstanceOfType<T>(obj);

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
