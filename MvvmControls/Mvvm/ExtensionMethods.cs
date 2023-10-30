using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm
{
    static class ExtensionMethods
    {
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

        public static async void FireAndForgetErrorHandling(this Task task, Action<Exception> errorHandler, bool swallowTaskCancelled = true)
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
