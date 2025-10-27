using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace RFBCodeWorks.Mvvm.Tests.Helpers
{
    ///<summary>This is a helper to allow the CommandManager to function during unit testing</summary>
    // https://stackoverflow.com/questions/12033798/why-doesnt-relaycommand-raisecanexecutechanged-work-in-a-unit-test
    public static class CommandManagerHelper// DispatcherTestHelper
    {
        private static DispatcherOperationCallback exitFrameCallback = ExitFrame;
        private static EventHandler Handler = Raise;
        private static bool IsSubscribed = false;
        private static bool wasRaised = false;

        /// <inheritdoc cref="CommandManager.InvalidateRequerySuggested"/>
        /// <param name="pretext">Text to WriteLine to the console prior to raising the event</param>
        public static void InvalidateRequerySuggested(string pretext)
        {
            if (!string.IsNullOrWhiteSpace(pretext)) Console.WriteLine(pretext);
            if (!IsSubscribed)
            {
                CommandManager.RequerySuggested += Handler;
                IsSubscribed = true;
            }
            wasRaised = false;
            CommandManager.InvalidateRequerySuggested();
            ProcessWorkItems();
            Assert.IsTrue(wasRaised, "\nCommandManager.RequerySuggested was not raised!");
        }

        private static void Raise(object s, EventArgs e)
        {
            //Console.WriteLine("- CommandManager.RequerySuggested Event was raised successfully.");
            wasRaised = true;
        }

        /// <summary>
        /// Synchronously processes all work items in the current dispatcher queue.
        /// </summary>
        /// <param name="minimumPriority">
        /// The minimum priority. 
        /// All work items of equal or higher priority will be processed.
        /// </param>
        private static void ProcessWorkItems(DispatcherPriority minimumPriority = DispatcherPriority.Background)
        {
            var frame = new DispatcherFrame();

            // Queue a work item.
            Dispatcher.CurrentDispatcher.BeginInvoke(
                minimumPriority, exitFrameCallback, frame);

            // Force the work item to run.
            // All queued work items of equal or higher priority will be run first. 
            Dispatcher.PushFrame(frame);
        }

        private static object ExitFrame(object state)
        {
            var frame = (DispatcherFrame)state;

            // Stops processing of work items, causing PushFrame to return.
            frame.Continue = false;
            return null;
        }
    }
}
