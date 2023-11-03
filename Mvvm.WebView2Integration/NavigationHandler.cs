using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.WebView2Integration
{
    /// <summary>
    /// Class that implements <see cref="IWebView2NavigationHandler"/> via its delegate properties.
    /// </summary>
    public class NavigationHandler : IWebView2NavigationHandlerExpanded
    {

        /// <inheritdoc cref="IWebView2NavigationHandlerExpanded.OnBasicAuthenticationRequested"/>
        public EventHandler<CoreWebView2BasicAuthenticationRequestedEventArgs> BasicAuthenticationRequestedHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandler.OnContentLoading"/>
        public EventHandler<CoreWebView2ContentLoadingEventArgs> ContentLoadingHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandler.OnCoreWebView2InitializationCompleted"/>
        public EventHandler<CoreWebView2InitializationCompletedEventArgs> CoreWebView2InitializationCompletedHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandler.OnHistoryChanged"/>
        public EventHandler<object> HistoryChangedHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandler.OnNavigationCompleted"/>
        public EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompletedHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandler.OnNavigationStarting"/>
        public EventHandler<CoreWebView2NavigationStartingEventArgs> NavigationStartingHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandler.OnSourceChanged"/>
        public EventHandler<CoreWebView2SourceChangedEventArgs> SourceChangedHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandler.OnWebMessageReceived"/>
        public EventHandler<CoreWebView2WebMessageReceivedEventArgs> WebMessageReceivedHandler { get; set; }


        public void OnBasicAuthenticationRequested(object sender, CoreWebView2BasicAuthenticationRequestedEventArgs e) => BasicAuthenticationRequestedHandler?.Invoke(sender, e);
        public void OnContentLoading(object sender, CoreWebView2ContentLoadingEventArgs e) => ContentLoadingHandler?.Invoke(sender, e);
        public void OnCoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e) => CoreWebView2InitializationCompletedHandler?.Invoke(sender, e);
        public void OnHistoryChanged(object sender, object e) => HistoryChangedHandler?.Invoke(sender, e);
        public void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e) => NavigationCompletedHandler?.Invoke(sender, e);
        public void OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e) => NavigationStartingHandler?.Invoke(sender, e);
        public void OnSourceChanged(object sender, CoreWebView2SourceChangedEventArgs e) => SourceChangedHandler?.Invoke(sender, e);
        public void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e) => WebMessageReceivedHandler?.Invoke(sender, e);
    }
}
