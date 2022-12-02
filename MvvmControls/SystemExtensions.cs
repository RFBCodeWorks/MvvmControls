using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RFBCodeWorks
{
    internal static class SystemExtensions
    {
        /// <summary>
        /// Generic function to check if an object is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>TRUE if the object is null, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static bool IsNull<T>(this T obj) => obj is null;

        /// <summary>
        /// Generic function to check if an object is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>TRUE if the object is not null, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull<T>(this T obj) => !obj.IsNull();

        ///<summary> Checks IsNullOrWhiteSpace() then returns the inverse. (Shortcut for !String.IsNullOrEmpty(str) </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty(this string str) => !String.IsNullOrWhiteSpace(str);

        ///<inheritdoc cref="String.IsNullOrWhiteSpace(string)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string str) => String.IsNullOrWhiteSpace(str);

        /// <summary>
        /// Check to see if the item exists in the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item">Item to search for</param>
        /// <returns>Returns the index of the item within the collection. If no match is found, return -1.</returns>
        public static int IndexOf<T>(this IEnumerable<T> array, T item)
        {
            if (array is null) return -1;
            int i = 0;
            foreach (T o in array)
            {
                if (o.Equals(item))
                    return i;
                i++;
            }
            return -1;
        }

        public static int ConvertToInt<T>(this T value)
            where T : class
        {
            if (value is null || value is DBNull) return 0;
            if (typeof(T) == typeof(string))
            {
                if (value?.ToString()?.IsNullOrEmpty() ?? true)
                    return 0;
                else
                    return (int)Math.Round(Convert.ToDouble(value.ToString()));
            }
            //Integer/Long
            else if (
                typeof(T) == typeof(Int16) | typeof(T) == typeof(Int32) | typeof(T) == typeof(Int64) |
                typeof(T) == typeof(UInt16) | typeof(T) == typeof(UInt32) | typeof(T) == typeof(UInt64) |
                typeof(T) == typeof(long) | typeof(T) == typeof(ulong) | typeof(T) == typeof(int)
                )
            {
                return Convert.ToInt32(value);
            }
            else
                return (int)Math.Round(Convert.ToDouble(value.ToString()));
        }
    }
}
