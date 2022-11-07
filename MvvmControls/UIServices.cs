using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Contains helper methods for UI, so far just one for showing a waitcursor
    /// </summary>
    public static class UIServices
    {
        /// <summary>
        /// Set the cursor to use when <see cref="SetBusyState()"/> is called
        /// </summary>
        /// <remarks>Default is <see cref="Cursors.Wait"/></remarks>
        public static Cursor BusyCursor { get; set; } = Cursors.Wait;
        
        /// <summary>
        ///   A value indicating whether the UI is currently busy
        /// </summary>
        private static bool IsBusy;

        /// <summary>
        /// Sets the cursor to some cursor type if the BusyCursor is not active
        /// </summary>
        public static void SetCursor(Cursor cursor)
        {
            if (!IsBusy)
                Mouse.OverrideCursor = cursor;
        }

        /// <summary>
        /// Resets the cursor to the default functionality
        /// </summary>
        public static void ResetCursor()
        {
            if (!IsBusy)
                Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Sets cursor to the <see cref="BusyCursor"/> to indicate application is performing a long-running action
        /// </summary>
        /// <remarks>Automatically resets cursor on Application.Idle</remarks>
        public static void SetBusyState()
        {
            SetBusyState(true);
        }

        /// <summary>
        /// Attempt to call <see cref="SetBusyState()"/>. Will only set the busy state of the cursor if called from a UI thread.
        /// </summary>
        public static void TrySetBusyState()
        {
            try { System.Windows.Application.Current.Dispatcher.Invoke(SetBusyState); } finally { }
        }

        /// <summary>
        /// Sets the busystate to busy or not busy.
        /// </summary>
        /// <param name="busy">if set to <c>true</c> the application is now busy.</param>
        private static void SetBusyState(bool busy)
        {
            if (busy != IsBusy)
            {
                IsBusy = busy;
                Mouse.OverrideCursor = busy ? BusyCursor : null;

                if (IsBusy)
                {
                    new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle, dispatcherTimer_Tick, System.Windows.Application.Current.Dispatcher);
                }
            }
        }

        /// <summary>
        /// Handles the Tick event of the dispatcherTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var dispatcherTimer = sender as DispatcherTimer;
            if (dispatcherTimer != null)
            {
                SetBusyState(false);
                dispatcherTimer.Stop();
            }
        }
    }
}
