using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MvvmControls
{
    //https://stackoverflow.com/a/26032796/12135042


    /// <summary>
    /// Interface that can be used to send a signal from the View to the ViewModel that the window is closing
    /// </summary>
    public interface IWindowClosing
    {
        /// <summary>
        /// Executes when window is closing
        /// </summary>
        /// <returns>TRUE if the window should be closed, FALSE to cancel closing the window</returns>
        bool OnClosing();

        /// <summary>
        /// Occurs when the window has closed
        /// </summary>
        void OnClosed();

    }

    /// <summary>
    /// ViewModel Interface that can be used to perform an action when the View is loading
    /// </summary>
    public interface IWindowLoading
    {
        /// <summary>
        /// Executes after a window has Loaded, but before content is rendered
        /// </summary>
        void OnLoaded();

        /// <summary>
        /// Executes after the content of a window has rendered for the first time
        /// </summary>
        void OnContentRendered();
    }

    /// <summary>
    /// Interface that can be used to send a signal from the View to the ViewModel that the window has Loaded
    /// </summary>
    public interface IWindowActivated
    {
        /// <summary>
        /// Action to take when the window is activated by the user
        /// </summary>
        void OnWindowActivated();

        /// <summary>
        /// Action to take when the window is deactivated by the user
        /// </summary>
        void OnWindowDeactivated();

    }

    /// <summary>
    /// Static class that allows subscribing to a <see cref="System.Windows.Window"/>'s events safely for MVVM.
    /// </summary>
    /// <remarks>
    /// ViewModels should implement the following interfaces if they want to interact with the window's events:
    /// <br/> - <see cref="IWindowClosing"/>
    /// <br/> - <see cref="IWindowLoading"/>
    /// <br/> - <see cref="IWindowActivated"/>
    /// </remarks>
    public static class WindowEventHandlers
    {
        /// <summary>
        /// Subscribe to a window's events.
        /// </summary>
        /// <inheritdoc cref="WindowEventHandlers" path="/remarks"/>
        public static void Subscribe(System.Windows.Window window)
        {
            window.Loaded += OnWindowLoaded;
            window.Closing += OnWindowClosing;
            window.Closed += OnWindowClosed;
            window.Activated += OnWindowActivated;
            window.Deactivated += OnWindowDeactivated;
            window.ContentRendered += OnWindowContentRendered;
        }

        /// <summary>
        /// Unsubscribe from the window's events
        /// </summary>
        public static void Unsubscribe(System.Windows.Window window)
        {
            window.Loaded -= OnWindowLoaded;
            window.Closing -= OnWindowClosing;
            window.ContentRendered -= OnWindowContentRendered;
            window.Activated += OnWindowActivated;
            window.Deactivated += OnWindowDeactivated;
        }

        /// <summary>
        /// Event Handler for the <see cref="System.Windows.Window.Deactivated"/> event. 
        /// This will check if the DataContext implements <see cref="IWindowActivated"/>. 
        /// and call OnWindowDeactivated if able.
        /// </summary>
        /// <param name="sender">the window that raised the event</param>
        /// <param name="e"/>
        public static void OnWindowDeactivated(object sender, EventArgs e)
        {
            if (sender is System.Windows.Window window)
            {
                var context = window.DataContext as IWindowActivated;
                if (context != null)
                {
                    context.OnWindowDeactivated();
                }
            }
        }

        /// <summary>
        /// Event Handler for the <see cref="System.Windows.Window.Deactivated"/> event. 
        /// This will check if the DataContext implements <see cref="IWindowActivated"/>. 
        /// and call OnWindowActivated if able.
        /// </summary>
        /// <param name="sender">the window that raised the event</param>
        /// <param name="e"/>
        public static void OnWindowActivated(object sender, EventArgs e)
        {
            if (sender is System.Windows.Window window)
            {
                var context = window.DataContext as IWindowActivated;
                if (context != null)
                {
                    context.OnWindowActivated();
                }
            }
        }


        /// <summary>
        /// Event Handler for the <see cref="System.Windows.FrameworkElement.Loaded"/> event. This will check if the DataContext implements <see cref="IWindowLoading"/>. 
        /// and call OnLoaded if able.
        /// </summary>
        /// <param name="sender">the window that raised the event</param>
        /// <param name="e"/>
        public static void OnWindowLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is System.Windows.Window window)
            {
                var context = window.DataContext as IWindowLoading;
                if (context != null)
                {
                    context.OnLoaded();
                }
            }
        }

        /// <summary>
        /// Event Handler for the <see cref="System.Windows.Window.ContentRendered"/> event. This will check if the DataContext implements <see cref="IWindowLoading"/>, 
        /// and call OnContentRendered if able.
        /// </summary>
        /// <param name="sender">the window that raised the event</param>
        /// <param name="e"/>
        public static void OnWindowContentRendered(object sender, EventArgs e)
        {
            if (sender is System.Windows.Window window)
            {
                var context = window.DataContext as IWindowLoading;
                if (context != null)
                {
                    context.OnContentRendered();
                }
            }
        }

        /// <summary>
        /// Event Handler for the <see cref="System.Windows.Window.Closing"/> event. This will check if the DataContext implements <see cref="IWindowClosing"/>. 
        /// If the DataContext does implement IClosing, then IClosing.OnClosing() will be called to determine if the window can close or if closing should be cancelled.
        /// </summary>
        /// <param name="sender">the window that raised the event</param>
        /// <param name="e">the cancellation event args</param>
        public static void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is System.Windows.Window window)
            {
                var context = window.DataContext as IWindowClosing;
                if (context != null)
                {
                    e.Cancel = !context.OnClosing();
                }
            }
        }

        /// <summary>
        /// Event Handler for the <see cref="System.Windows.Window.Closing"/> event. This will check if the DataContext implements <see cref="IWindowClosing"/>. 
        /// If the DataContext does implement IClosing, then IClosing.OnClosing() will be called to determine if the window can close or if closing should be cancelled.
        /// </summary>
        /// <param name="sender">the window that raised the event</param>
        /// <param name="e"/>
        public static void OnWindowClosed(object sender, EventArgs e)
        {
            if (sender is System.Windows.Window window)
            {
                var context = window.DataContext as IWindowClosing;
                if (context != null)
                {
                    context.OnClosed();
                }
            }
        }
    }
}
