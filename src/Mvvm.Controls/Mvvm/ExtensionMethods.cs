using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace RFBCodeWorks.Mvvm
{

    internal static class ExtensionMethods
    {
        public static void ThrowIfNull<T>([NotNull] this T value, string paramName)
        {
            if (value is null) ThrowArgNull(paramName);
        }

        public static void ThrowInvalidOperationIfNull<T>([NotNull] this T value, string message)
        {
            if (value is null) ThrowInvalidOperation(message);
        }

        [DoesNotReturn] private static void ThrowArgNull(string? paramName) => throw new ArgumentNullException(paramName);
        [DoesNotReturn] private static void ThrowInvalidOperation(string message) => throw new InvalidOperationException(message);


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
