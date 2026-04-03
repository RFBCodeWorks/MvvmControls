using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

#nullable enable

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Represents an injectable service that allows viewmodels to indicate to the user that the application is busy by setting the cursor to the <see cref="BusyCursor"/>
    /// </summary>
    /// <remarks>
    /// Default Implementations:
    /// <br/> - <see cref="UIServices.GetMockCursorService"/> : an object that can be used for unit testing or if the STAThread for WPF is unavailable.
    /// <br/> - <see cref="UIServices.GetDispatcherCursorService"/> : Uses <see cref="Application.Current"/>.Dispatcher to update the <see cref="Mouse.OverrideCursor"/>
    /// <para/>Prefer this over <see cref="UIServices.SetBusyState()"/>. 
    /// </remarks>
    public interface ICursorService
    {
        /// <summary>
        /// Sets the type of cursor to use when <see cref="SetBusy"/> is called
        /// </summary>
        Cursor BusyCursor { get; set; }

        /// <summary>
        /// True when the application is busy (set by <see cref="SetBusy"/>)
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Tries to sets the cursor to this <paramref name="cursor"/> type if the service is not busy.
        /// </summary>
        /// <param name="cursor"></param>
        void OverrideCursor(Cursor cursor);

        /// <summary>
        /// Sets the <see cref="IsBusy"/> state to true
        /// </summary>
        void SetBusy();

        /// <summary>
        /// Invokes <see cref="SetBusy"/> asynchronously
        /// </summary>
        Task SetBusyAsync(CancellationToken token);

        /// <summary>
        /// Resets the <see cref="IsBusy"/> flag
        /// </summary>
        public void Reset();
    }

    /// <summary>
    /// Contains helper methods for UI, so far just one for showing a waitcursor
    /// <para/> Note : These methods are only accessible from the UI Thread!
    /// <br/> Inject <see cref="ICursorService"/> from ViewModels and unit testing.
    /// </summary>
    public static partial class UIServices
    {
        /// <summary>
        /// Returns a new <see cref="UIServices"/> object athat can be use for unit testing. Does not rely on <see cref="System.Windows.Application.Current"/>.Dispatcher
        /// </summary>
        /// <returns></returns>
        public static MockCursorService GetMockCursorService() => new MockCursorService();

        /// <summary>
        /// A mock <see cref="ICursorService"/> that can be used for unit tests
        /// </summary>
        public class MockCursorService : ICursorService
        {
            private Cursor cursor = Cursors.Arrow;
            private Cursor busyCursor = Cursors.Wait;
            private Cursor? previousCursor;

            public event EventHandler? CursorChanged;

            /// <summary>
            /// The current cursor
            /// </summary>
            public Cursor Cursor
            {
                get => cursor;
                set
                {
                    if (cursor != (value ??= Cursors.Arrow))
                    {
                        cursor = value;
                        CursorChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }

            public Cursor BusyCursor { get => busyCursor; set => busyCursor = value ?? Cursors.Wait; } 

            public bool IsBusy { get; set; }

            public void SetBusy()
            {
                if (Cursor != BusyCursor)
                    previousCursor = Cursor;

                Cursor = BusyCursor;
                IsBusy = true;
            }

            public Task SetBusyAsync(CancellationToken token)
            {
                token.ThrowIfCancellationRequested();
                SetBusy();
                return Task.CompletedTask;
            }

            public void OverrideCursor(Cursor cursor) 
            {
                previousCursor = cursor;
                if (!IsBusy) 
                    Cursor = cursor; 
            }

            /// <summary>
            /// Reset to the default state
            /// </summary>
            public void Reset()
            {
                IsBusy = false;
                Cursor = previousCursor ?? Cursors.Arrow;
            }
        }        
    }
}
