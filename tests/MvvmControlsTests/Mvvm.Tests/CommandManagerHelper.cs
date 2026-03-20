using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace RFBCodeWorks.Mvvm.Tests
{
    ///<summary>This is a helper to allow the CommandManager to function during unit testing</summary>
    // https://stackoverflow.com/questions/12033798/why-doesnt-relaycommand-raisecanexecutechanged-work-in-a-unit-test
    [STATestClass]
    public class CommandManagerHelper// DispatcherTestHelper
    {

        [STATestMethod]
        public async Task InvalidateRequerySuggestedTest()
        {
            await CommandManagerHelper.InvalidateRequerySuggested("CommandManager Test");
        }

        /// <inheritdoc cref="CommandManager.InvalidateRequerySuggested"/>
        /// <param name="pretext">Text to WriteLine to the console prior to raising the event</param>
        public static async Task InvalidateRequerySuggested(string pretext)
        {
            if (!string.IsNullOrWhiteSpace(pretext)) Console.WriteLine(pretext);
            
            bool wasRaised = false;
            void handler(object? o, EventArgs e) => wasRaised = true;
            CommandManager.RequerySuggested += handler;
            try
            {
                // Trigger the CommandManager
                CommandManager.InvalidateRequerySuggested();
                var frame = new DispatcherFrame();
                var dispatched = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, ExitFrame, frame);
                Dispatcher.PushFrame(frame);

                await dispatched;
                Assert.IsTrue(wasRaised, "\nCommandManager.RequerySuggested was not raised!");
            }
            finally
            {
                CommandManager.RequerySuggested -= handler;
            }
        }

        private static object? ExitFrame(object state)
        {
            var frame = (DispatcherFrame)state;

            // Stops processing of work items, causing PushFrame to return.
            frame.Continue = false;
            return null;
        }
    }
}
