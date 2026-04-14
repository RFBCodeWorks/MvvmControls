using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;


namespace RFBCodeWorks.Mvvm.Tests
{
    /// <summary>
    /// Creates a new Application running on the STA Thread for testing WPF with the default Application.Current.Dispatcher
    /// </summary>
    /// <remarks>
    /// <br/><see href="https://stackoverflow.com/questions/1106881/using-the-wpf-dispatcher-in-unit-tests"/>
    /// <br/><see href="https://stackoverflow.com/questions/9336165/correct-method-for-using-the-wpf-dispatcher-in-unit-tests"/>
    /// </remarks>
    [STATestClass]
    public static class ApplicationInitializer
    {
        private static Thread? _uiThread;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            var waitForApplicationRun = new TaskCompletionSource<bool>();

            _uiThread = new Thread(() =>
            {
                var application = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };

                application.Startup += (s, e) => { waitForApplicationRun.SetResult(true); };
                try
                {
                    application.Run();
                }
                catch (Exception e)
                {
                    Console.WriteLine( "\n--------------------\nMock Application Encountered an Error:\nError : {0}\n\nStack Trace : \n{1}", e.Message, e.StackTrace);
                    throw;
                }
            });

            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.IsBackground = true;
            _uiThread.Start();
            waitForApplicationRun.Task.Wait(context.CancellationToken);
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
        }

        /// <summary>
        /// Pumps the dispatcher to process ApplicationIdle priority operations.
        /// This allows the DispatcherTimer at ApplicationIdle priority to tick.
        /// </summary>
        public static void PumpDispatcher()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        private static object? ExitFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }
}
