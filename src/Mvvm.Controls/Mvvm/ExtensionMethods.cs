using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace RFBCodeWorks.Mvvm
{

    internal static class ExtensionMethods
    {
        public static void ThrowIfNull<T>(this T value, string paramName)
        {
            if (value is null) ThrowArgNull(paramName);
        }
        private static void ThrowArgNull(string? paramName) => throw new ArgumentNullException(paramName);


        /// <summary>
        /// Extension Method that selects all of type <typeparamref name="T2"/> from the collection
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<T2> SelectWhere<T2>(this IEnumerable collection)
        {
            foreach(object x in collection)
            {
                if (x is T2 y)
                    yield return y;
            }
            yield break;
        }

        public static async void FireAndForgetErrorHandling(this Task task, Action<Exception>? errorHandler, bool swallowTaskCancelled = true)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException) when (swallowTaskCancelled) { }
            catch (System.Threading.Tasks.TaskCanceledException) when (swallowTaskCancelled) { }
            catch (Exception e)
            {
                errorHandler?.Invoke(e);
            }
        }
    }
}
