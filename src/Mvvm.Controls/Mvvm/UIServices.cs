using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

#nullable enable

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Contains helper methods for UI, so far just one for showing a waitcursor
    /// </summary>
    public static partial class UIServices
    {
        /// <summary>
        /// Returns the singleton <see cref="ICursorService"/> that uses <see cref="System.Windows.Application.Current"/>.Dispatcher to interact with <see cref="Mouse.OverrideCursor"/>
        /// </summary>
        /// <returns></returns>
        public static ICursorService GetApplicationDispatcherCursorService() => ApplicationDispatchCursorService.GetSerivce();
        
        /// <inheritdoc cref="ICursorService.BusyCursor"/>
        [Obsolete("Prefer usage of RFBCodeWorks.Mvvm.ICursorService within ViewModels. Calling this from WPF Code-Behind is fine.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Cursor BusyCursor 
        { 
            get => GetApplicationDispatcherCursorService().BusyCursor; 
            set => GetApplicationDispatcherCursorService().BusyCursor = value; 
        }
        
        /// <inheritdoc cref="ICursorService.OverrideCursor(Cursor)"/>
        [Obsolete($"This method requires an STAThread. Prefer usage of {nameof(RFBCodeWorks)}.{nameof(RFBCodeWorks.Mvvm)}.{nameof(RFBCodeWorks.Mvvm.ICursorService)}", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetCursor(Cursor cursor)
            => GetApplicationDispatcherCursorService().OverrideCursor(cursor);

        /// <inheritdoc cref="ICursorService.Reset"/>
        [Obsolete($"This method requires an STAThread. Prefer usage of {nameof(RFBCodeWorks)}.{nameof(RFBCodeWorks.Mvvm)}.{nameof(RFBCodeWorks.Mvvm.ICursorService)}", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ResetCursor()
        => GetApplicationDispatcherCursorService().Reset();

        /// <inheritdoc cref="ICursorService.SetBusy"/>
        [Obsolete($"This method requires an STAThread. Prefer usage of {nameof(RFBCodeWorks)}.{nameof(RFBCodeWorks.Mvvm)}.{nameof(RFBCodeWorks.Mvvm.ICursorService)}", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetBusyState()
            => GetApplicationDispatcherCursorService().SetBusy();

        /// <inheritdoc cref="ICursorService.SetBusy"/>
        [Obsolete($"This method requires an STAThread. Prefer usage of {nameof(RFBCodeWorks)}.{nameof(RFBCodeWorks.Mvvm)}.{nameof(RFBCodeWorks.Mvvm.ICursorService)}", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void TrySetBusyState()
            => GetApplicationDispatcherCursorService().SetBusy();


        private class ApplicationDispatchCursorService : ICursorService
        {
            private ApplicationDispatchCursorService()
            {
                timer = new DispatcherTimer(TimeSpan.FromMilliseconds(1), DispatcherPriority.ApplicationIdle, SetBusyStateFalse, Application.Current.Dispatcher)
                { IsEnabled = false, Tag = nameof(ICursorService) };
            }

            public static ICursorService GetSerivce ()
            {
                if (applicationService is null) 
                {
                    if (Application.Current is null)
                    {
                        throw new InvalidOperationException($"Application.Current is null - Unable to construct {typeof(ApplicationDispatchCursorService).FullName}");
                    }
                    if (Application.Current.Dispatcher is null)
                    {
                        throw new InvalidOperationException($"Application.Current.Dispatcher is null - Unable to construct {typeof(ApplicationDispatchCursorService).FullName}");
                    }
                    if (Application.Current.Dispatcher.Thread.GetApartmentState() != ApartmentState.STA)
                    {
                        throw new InvalidOperationException($"Application.Current.Dispatcher is not {nameof(ApartmentState)}.{nameof(ApartmentState.STA)} - Unable to construct {typeof(ApplicationDispatchCursorService).FullName}");
                    }
                    applicationService = new();
                }
                return applicationService;
            }

            private static ApplicationDispatchCursorService? applicationService;

            private readonly DispatcherTimer timer;
            private Cursor busyCursor = Cursors.Wait;
            private Cursor? previousCursor;

            public Cursor BusyCursor { get => busyCursor; set => busyCursor = value ?? Cursors.Wait; }
            public bool IsBusy { get; private set; }

            public void SetBusy()
            {
                if (Application.Current.Dispatcher.CheckAccess())
                    SetBusyCursor();
                else
                    Application.Current.Dispatcher.Invoke(SetBusyCursor, CancellationToken.None);
            }

            private void SetBusyCursor(CancellationToken token = default)
            {
                if (!IsBusy && !token.IsCancellationRequested)
                {
                    IsBusy = true;
                    if (previousCursor != BusyCursor)
                        previousCursor = Mouse.OverrideCursor;

                    Mouse.OverrideCursor = BusyCursor;
                    timer.IsEnabled = true;
                }
            }

            public Task SetBusyAsync(CancellationToken token)
            {
                token.ThrowIfCancellationRequested();
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    SetBusyCursor(token);
                    return Task.CompletedTask;
                }
                else
                    return Application.Current.Dispatcher.BeginInvoke(SetBusyCursor, token).Task;
            }

            public void OverrideCursor(Cursor cursor)
            {
                previousCursor = cursor;
                if (!IsBusy)
                {
                    try { Mouse.OverrideCursor = cursor; } catch { }
                }
            }

            public void Reset()
            {
                if (Application.Current.Dispatcher.CheckAccess())
                    SetBusyStateFalse(null, EventArgs.Empty);
                else
                    Application.Current.Dispatcher.BeginInvoke(SetBusyStateFalseDelegate);
            }

            private void SetBusyStateFalseDelegate() => SetBusyStateFalse(null, EventArgs.Empty);
            private void SetBusyStateFalse(object? sender, EventArgs e)
            {
                IsBusy = false;
                timer.IsEnabled = false;
                if (Mouse.OverrideCursor == BusyCursor)
                    Mouse.OverrideCursor = previousCursor;
            }
        }

    }
}
