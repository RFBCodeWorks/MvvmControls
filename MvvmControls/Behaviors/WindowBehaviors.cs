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

        #region < IWindowFocusHandler >

        /// <summary>
        /// Assigns an <see cref="IWindowActivatedHandler"/> handler to a <see cref="Window"/>
        /// </summary>
        public static readonly DependencyProperty IWindowFocusHandlerProperty =
            DependencyProperty.RegisterAttached(nameof(IWindowFocusHandler),
                typeof(IWindowFocusHandler),
                typeof(WindowBehaviors),
                new PropertyMetadata(null, IWindowFocusHandlerPropertyChanged)
                );

        /// <summary>
        /// Gets the assigned <see cref="IWindowFocusHandler"/> from a <see cref="UIElement"/>
        /// </summary>
        public static IWindowFocusHandler GetIWindowFocusHandler(DependencyObject obj) => (IWindowFocusHandler)obj.GetValue(IWindowFocusHandlerProperty);

        /// <summary>
        /// Assigns an <see cref="IWindowFocusHandler"/> to a <see cref="UIElement"/>
        /// </summary>
        public static void SetIWindowFocusHandler(DependencyObject obj, IWindowFocusHandler value)
        {
            if (obj is not null and not UIElement) throw new ArgumentException($"{nameof(IWindowFocusHandler)} property can only be bound to a {nameof(UIElement)}");
            obj.SetValue(IWindowFocusHandlerProperty, value);
        }

        private static void IWindowFocusHandlerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var w = d as UIElement;
            if (w is null) return;
            if (e.NewValue != null)
            {
                w.GotFocus += W_GotFocus;
                w.LostFocus += W_LostFocus;
            }
            else
            {
                w.GotFocus -= W_GotFocus;
                w.LostFocus -= W_LostFocus;
            }
        }

        private static void W_LostFocus(object sender, RoutedEventArgs e)
        {
            GetIWindowFocusHandler(sender as DependencyObject)?.OnUIElementLostFocus(sender, e);
        }

        private static void W_GotFocus(object sender, RoutedEventArgs e)
        {
            GetIWindowFocusHandler(sender as DependencyObject)?.OnUIElementGotFocus(sender, e);
        }

        #endregion

        #region < IWindowLoading >

        /// <summary>
        /// Assigns an <see cref="IWindowLoadingHandler"/> handler to a <see cref="Window"/>
        /// </summary>
        public static readonly DependencyProperty IWindowLoadingHandlerProperty =
            DependencyProperty.RegisterAttached(nameof(IWindowLoadingHandler),
                typeof(IWindowLoadingHandler),
                typeof(WindowBehaviors),
                new PropertyMetadata(null, IWindowLoadingHandlerPropertyChanged)
                );

        /// <summary>
        /// Gets the assigned <see cref="IWindowLoadingHandler"/> from a <see cref="Window"/>
        /// </summary>
        public static IWindowLoadingHandler GetIWindowLoadingHandler(DependencyObject obj) => (IWindowLoadingHandler)obj.GetValue(IWindowLoadingHandlerProperty);

        /// <summary>
        /// Assigns an <see cref="IWindowActivatedHandler"/> to a <see cref="Window"/>
        /// </summary>
        public static void SetIWindowLoadingHandler(DependencyObject obj, IWindowLoadingHandler value)
        {
            if (obj is not null and not Window) throw new ArgumentException($"{nameof(IWindowLoadingHandler)} property can only be bound to a {nameof(Window)}");
            obj.SetValue(IWindowLoadingHandlerProperty, value);
        }


        private static void IWindowLoadingHandlerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var w = d as Window;
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
            GetIWindowLoadingHandler(sender as DependencyObject)?.OnWindowContentRendered(sender, e);
        }

        private static void W_Loaded(object sender, RoutedEventArgs e)
        {
            GetIWindowLoadingHandler(sender as DependencyObject)?.OnWindowLoaded(sender, e);
        }

        #endregion

        #region < IWindowActivated >

        /// <summary>
        /// Assigns an <see cref="IWindowActivatedHandler"/> handler to a <see cref="Window"/>
        /// </summary>
        public static readonly DependencyProperty IWindowActivatedHandlerProperty =
            DependencyProperty.RegisterAttached(nameof(IWindowActivatedHandler),
                typeof(IWindowActivatedHandler),
                typeof(WindowBehaviors),
                new PropertyMetadata(null, IWindowActivatedHandlerPropertyChanged)
                );

        /// <summary>
        /// Gets the assigned <see cref="IWindowActivatedHandler"/> from a <see cref="Window"/>
        /// </summary>
        public static IWindowActivatedHandler GetIWindowActivatedHandler(DependencyObject obj) => (IWindowActivatedHandler)obj.GetValue(IWindowActivatedHandlerProperty);


        /// <summary>
        /// Assigns an <see cref="IWindowActivatedHandler"/> to a <see cref="Window"/>
        /// </summary>
        public static void SetIWindowActivatedHandler(DependencyObject obj, IWindowActivatedHandler value)
        {
            if (obj is not null and not Window) throw new ArgumentException($"{nameof(IWindowActivatedHandler)} property can only be bound to a {nameof(Window)}");
            obj.SetValue(IWindowActivatedHandlerProperty, value);
        }

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
            GetIWindowActivatedHandler(sender as DependencyObject)?.OnWindowDeactivated(sender, e);
        }

        private static void W_Activated(object sender, EventArgs e)
        {
            GetIWindowActivatedHandler(sender as DependencyObject)?.OnWindowActivated(sender, e);
        }

        #endregion

        #region < IWindowClosing >

        /// <summary>
        /// Assigns an <see cref="IWindowClosingHandler"/> handler to a <see cref="Window"/>
        /// </summary>
        public static readonly DependencyProperty IWindowClosingHandlerProperty =
            DependencyProperty.RegisterAttached(nameof(IWindowClosingHandler),
                typeof(IWindowClosingHandler),
                typeof(WindowBehaviors),
                new PropertyMetadata(null, IWindowClosingHandlerPropertyChanged)
                );

        /// <summary>
        /// Gets the assigned <see cref="IWindowLoadingHandler"/> from a <see cref="Window"/>
        /// </summary>
        public static IWindowClosingHandler GetIWindowClosingHandler(DependencyObject obj) => (IWindowClosingHandler)obj.GetValue(IWindowClosingHandlerProperty);

        /// <summary>
        /// Assigns an <see cref="IWindowClosingHandler"/> to a <see cref="Window"/>
        /// </summary>
        public static void SetIWindowClosingHandler(DependencyObject obj, IWindowClosingHandler value)
        {
            if (obj is not null and not Window) throw new ArgumentException($"{nameof(IWindowClosingHandler)} property can only be bound to a {nameof(Window)}");
            obj.SetValue(IWindowClosingHandlerProperty, value);
        }

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
            GetIWindowClosingHandler(sender as DependencyObject)?.OnWindowClosed(sender, e);
        }

        private static void W_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GetIWindowClosingHandler(sender as DependencyObject)?.OnWindowClosing(sender, e);
        }

        #endregion

    }
}
