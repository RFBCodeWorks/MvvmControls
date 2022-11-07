using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.MvvmControls.Behaviors
{
    /// <summary>
    /// Attached Properties for Windows that allow MVVM to react to a window Loading/Activating/Deactivating/Closing
    /// </summary>
    public static class WindowBehaviors
    {

        #region < IWindowActivated >

        /// <summary>
        /// Assigns an <see cref="IWindowActivated"/> handler to a <see cref="Window"/>
        /// </summary>
        public static readonly DependencyProperty IWindowActivatedHandlerProperty =
            DependencyProperty.RegisterAttached("IWindowActivatedHandler",
                typeof(IWindowActivated),
                typeof(WindowBehaviors),
                new PropertyMetadata(null, IWindowActivatedHandlerPropertyChanged)
                );

        /// <summary>
        /// Gets the assigned <see cref="IWindowActivated"/> from a <see cref="Window"/>
        /// </summary>
        public static IWindowActivated GetIWindowActivatedHandler(DependencyObject obj) => (IWindowActivated)obj.GetValue(IWindowActivatedHandlerProperty);


        /// <summary>
        /// Assigns an <see cref="IWindowActivated"/> to a <see cref="Window"/>
        /// </summary>
        public static void SetIWindowActivatedHandler(DependencyObject obj, IWindowActivated value) => obj.SetValue(IWindowActivatedHandlerProperty, value);

        private static void IWindowActivatedHandlerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window w = d as Window;
            if (w is null) return;
            if (e.NewValue != null)
            {
                w.Activated += W_Activated;
                w.Deactivated += W_Deactivated;
            }
            else
            {
                w.Activated -= W_Activated;
                w.Deactivated -= W_Deactivated;
            }
        }

        private static void W_Deactivated(object sender, EventArgs e)
        {
            GetIWindowActivatedHandler(sender as DependencyObject)?.OnWindowDeactivated();
        }

        private static void W_Activated(object sender, EventArgs e)
        {
            GetIWindowActivatedHandler(sender as DependencyObject)?.OnWindowActivated();
        }

        #endregion


        #region < IWindowClosing >

        /// <summary>
        /// Assigns an <see cref="IWindowClosing"/> handler to a <see cref="Window"/>
        /// </summary>
        public static readonly DependencyProperty IWindowClosingHandlerProperty =
            DependencyProperty.RegisterAttached("IWindowClosingHandler",
                typeof(IWindowClosing),
                typeof(WindowBehaviors),
                new PropertyMetadata(null, IWindowClosingHandlerPropertyChanged)
                );

        /// <summary>
        /// Gets the assigned <see cref="IWindowLoading"/> from a <see cref="Window"/>
        /// </summary>
        public static IWindowClosing GetIWindowClosingHandler(DependencyObject obj) => (IWindowClosing)obj.GetValue(IWindowClosingHandlerProperty);

        /// <summary>
        /// Assigns an <see cref="IWindowClosing"/> to a <see cref="Window"/>
        /// </summary>
        public static void SetIWindowClosingHandler(DependencyObject obj, IWindowClosing value) => obj.SetValue(IWindowClosingHandlerProperty, value);

        private static void IWindowClosingHandlerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window w = d as Window;
            if (w is null) return;
            if (e.NewValue != null)
            {
                w.Closing += W_Closing;
                w.Closed += W_Closed;
            }
            else
            {
                w.Closing -= W_Closing;
                w.Closed -= W_Closed;
            }
        }

        private static void W_Closed(object sender, EventArgs e)
        {
            GetIWindowClosingHandler(sender as DependencyObject)?.OnClosed();
        }

        private static void W_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !GetIWindowClosingHandler(sender as DependencyObject)?.OnClosing() ?? true; 
        }

        #endregion


        #region < IWindowLoading >

        /// <summary>
        /// Assigns an <see cref="IWindowLoading"/> handler to a <see cref="Window"/>
        /// </summary>
        public static readonly DependencyProperty IWindowLoadingHandlerProperty =
            DependencyProperty.RegisterAttached("IWindowLoadingHandler",
                typeof(IWindowLoading),
                typeof(WindowBehaviors),
                new PropertyMetadata(null, IWindowLoadingHandlerPropertyChanged)
                );
        
        /// <summary>
        /// Gets the assigned <see cref="IWindowLoading"/> from a <see cref="Window"/>
        /// </summary>
        public static IWindowLoading GetIWindowLoadingHandler(DependencyObject obj) => (IWindowLoading)obj.GetValue(IWindowLoadingHandlerProperty);

        /// <summary>
        /// Assigns an <see cref="IWindowActivated"/> to a <see cref="Window"/>
        /// </summary>
        public static void SetIWindowLoadingHandler(DependencyObject obj, IWindowLoading value) => obj.SetValue(IWindowLoadingHandlerProperty, value);


        private static void IWindowLoadingHandlerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window w = d as Window;
            if (w is null) return;
            if (e.NewValue != null)
            {
                w.Loaded += W_Loaded;
                w.ContentRendered += W_ContentRendered;
            }
            else
            {
                w.Loaded -= W_Loaded;
                w.ContentRendered -= W_ContentRendered;
            }
        }

        private static void W_ContentRendered(object sender, EventArgs e)
        {
            GetIWindowLoadingHandler(sender as DependencyObject)?.OnContentRendered();
        }

        private static void W_Loaded(object sender, RoutedEventArgs e)
        {
            GetIWindowLoadingHandler(sender as DependencyObject)?.OnLoaded();
        }

        #endregion
    }
}
