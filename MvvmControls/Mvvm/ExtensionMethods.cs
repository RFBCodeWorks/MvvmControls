using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm
{
    static class ExtensionMethods
    {
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
