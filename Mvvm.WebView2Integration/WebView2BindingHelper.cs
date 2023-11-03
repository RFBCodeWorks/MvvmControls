using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.Mvvm.WebView2Integration
{
    public class WebView2BindingHelper : FrameworkElement
    {
        static WebView2BindingHelper()
        {
            IsHitTestVisibleProperty.OverrideMetadata(typeof(WebView2BindingHelper), new FrameworkPropertyMetadata(false));
            VisibilityProperty.OverrideMetadata(typeof(WebView2BindingHelper), new FrameworkPropertyMetadata(Visibility.Collapsed));
            HeightProperty.OverrideMetadata(typeof(WebView2BindingHelper), new FrameworkPropertyMetadata((double)0));
            MaxHeightProperty.OverrideMetadata(typeof(WebView2BindingHelper), new FrameworkPropertyMetadata((double)0));
            WidthProperty.OverrideMetadata(typeof(WebView2BindingHelper), new FrameworkPropertyMetadata((double)0));
            MaxWidthProperty.OverrideMetadata(typeof(WebView2BindingHelper), new FrameworkPropertyMetadata((double)0));
        }

        private bool IsNavigationHandlerExpanded;

        public WebView2 WebView
        {
            get { return (WebView2)GetValue(WebViewProperty); }
            set { SetValue(WebViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WebView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WebViewProperty =
            DependencyProperty.Register("WebView", typeof(WebView2), typeof(WebView2BindingHelper), new PropertyMetadata(null, WebViewChanged));

        public IWebView2NavigationHandler NavigationHandler
        {
            get { return (IWebView2NavigationHandler)GetValue(NavigationHandlerProperty); }
            set { SetValue(NavigationHandlerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationHandler.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationHandlerProperty =
            DependencyProperty.Register(nameof(NavigationHandler), typeof(IWebView2NavigationHandler), typeof(WebView2BindingHelper), new PropertyMetadata(null, NavigationHandlerChanged));

        private static void NavigationHandlerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebView2BindingHelper wv)
            {
                wv.IsNavigationHandlerExpanded = e.NewValue is IWebView2NavigationHandlerExpanded;
            }
        }

        private static void WebViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not WebView2BindingHelper bh) return;
            if (e.OldValue is WebView2 old)
            {
                //Basic
                old.NavigationStarting -= bh.OnNavigationStarting;
                old.NavigationCompleted -= bh.OnNavigationCompleted;
                //Expanded
                old.ContentLoading -= bh.OnContentLoading;
                old.CoreWebView2InitializationCompleted -= bh.OnCoreWebView2InitializationCompleted;
                old.SourceChanged -= bh.OnSourceChanged;
                old.WebMessageReceived -= bh.OnWebMessageReceived;
                if (old.CoreWebView2 != null)
                {
                    old.CoreWebView2.HistoryChanged -= bh.CoreWebView2_HistoryChanged;
                    old.CoreWebView2.BasicAuthenticationRequested -= bh.CoreWebView2_BasicAuthenticationRequested;
                }
            }
            if (e.NewValue is WebView2 wv)
            {
                //Basic
                wv.NavigationStarting += bh.OnNavigationStarting;
                wv.NavigationCompleted += bh.OnNavigationCompleted;
                //Expanded
                wv.ContentLoading += bh.OnContentLoading;
                wv.CoreWebView2InitializationCompleted += bh.OnCoreWebView2InitializationCompleted;
                wv.SourceChanged += bh.OnSourceChanged;
                wv.WebMessageReceived += bh.OnWebMessageReceived;
            }
        }


        private void CoreWebView2_BasicAuthenticationRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2BasicAuthenticationRequestedEventArgs e)
        {
            if (IsNavigationHandlerExpanded && NavigationHandler is IWebView2NavigationHandlerExpanded nav)
                nav.OnBasicAuthenticationRequested(sender, e);
        }

        private void CoreWebView2_HistoryChanged(object sender, object e)
        {
            if (IsNavigationHandlerExpanded && NavigationHandler is IWebView2NavigationHandlerExpanded nav)
                nav.OnHistoryChanged(sender, e);
        }

        private void OnCoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (sender is WebView2 wv)
            {
                wv.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
                wv.CoreWebView2.BasicAuthenticationRequested += CoreWebView2_BasicAuthenticationRequested;
            }
            if (IsNavigationHandlerExpanded && NavigationHandler is IWebView2NavigationHandlerExpanded nav)
                nav.OnCoreWebView2InitializationCompleted(sender, e);
        }

        private void OnWebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (IsNavigationHandlerExpanded && NavigationHandler is IWebView2NavigationHandlerExpanded nav)
                nav.OnWebMessageReceived(sender, e);
        }

        private void OnSourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            if (IsNavigationHandlerExpanded && NavigationHandler is IWebView2NavigationHandlerExpanded nav)
                nav.OnSourceChanged(sender, e);
        }

        private void OnContentLoading(object sender, Microsoft.Web.WebView2.Core.CoreWebView2ContentLoadingEventArgs e)
        {
            if (IsNavigationHandlerExpanded && NavigationHandler is IWebView2NavigationHandlerExpanded nav)
                nav.OnContentLoading(sender, e);
        }

        private void OnNavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            NavigationHandler?.OnNavigationStarting(sender, e);
        }

        private void OnNavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            NavigationHandler?.OnNavigationCompleted(sender, e);
        }
    }
}
