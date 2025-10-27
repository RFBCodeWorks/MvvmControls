using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm.WebView2Integration
{
    /// <summary>
    /// Basic implementation of <see cref="IWebView2NavigationHandler"/> that provides other common functionality, such as Forward/Back commands, and storage of the associated <see cref="Microsoft.Web.WebView2.Core.CoreWebView2"/> object.
    /// </summary>
    public partial class NavigationHandler : ObservableObject, IWebView2NavigationHandler, IWebView2NavigationHandlerExpanded
    {

        private bool _isStopped;
        private CoreWebView2 _core;

        /// <summary> 
        /// A URI that can be bound to the 'Source' property in xaml with. If bound, when this is updated it will cause the WebView2 to navigate to the new page.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TextboxUri))]
        private Uri _uri;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(StopCommand))]
        [NotifyCanExecuteChangedFor(nameof(ResumeCommand))]
        private bool _isNavigating;

        /// <summary>
        /// A property that the friendly path from the <see cref="Uri"/>
        /// <br/> - Uri.AbsoluteUri
        /// <br/> - String.Empty
        /// </summary>
        public string TextboxUri => Uri is null ? string.Empty : Uri.IsFile ? Uri.LocalPath : Uri.AbsoluteUri;

        /// <summary> The associated CoreWebView2 object, as determined by the last time <see cref="OnCoreWebView2InitializationCompleted"/> was executed. </summary>
        /// <remarks> This value may be null </remarks>
        public CoreWebView2 Core => _core;

        /// <summary>True if <see cref="Core"/>  is not null, otherwise false.</summary>
        public bool IsInitialized => _core is not null;

        #region < Public Event Handler Properties >

        /// <inheritdoc cref="IWebView2NavigationHandlerExpanded.OnBasicAuthenticationRequested"/>
        public EventHandler<CoreWebView2BasicAuthenticationRequestedEventArgs> BasicAuthenticationRequestedHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandlerExpanded.OnContentLoading"/>
        public EventHandler<CoreWebView2ContentLoadingEventArgs> ContentLoadingHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandlerExpanded.OnCoreWebView2InitializationCompleted"/>
        public EventHandler<CoreWebView2InitializationCompletedEventArgs> CoreWebView2InitializationCompletedHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandlerExpanded.OnHistoryChanged"/>
        public EventHandler<object> HistoryChangedHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandler.OnNavigationCompleted"/>
        public EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompletedHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandler.OnNavigationStarting"/>
        public EventHandler<CoreWebView2NavigationStartingEventArgs> NavigationStartingHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandlerExpanded.OnSourceChanged"/>
        public EventHandler<CoreWebView2SourceChangedEventArgs> SourceChangedHandler { get; set; }

        /// <inheritdoc cref="IWebView2NavigationHandlerExpanded.OnWebMessageReceived"/>
        public EventHandler<CoreWebView2WebMessageReceivedEventArgs> WebMessageReceivedHandler { get; set; }

        /// <inheritdoc cref="CoreWebView2.BasicAuthenticationRequested" />
        public void OnBasicAuthenticationRequested(object sender, CoreWebView2BasicAuthenticationRequestedEventArgs e) => BasicAuthenticationRequestedHandler?.Invoke(sender, e);

        /// <inheritdoc cref="CoreWebView2.ContentLoading" />
        public void OnContentLoading(object sender, CoreWebView2ContentLoadingEventArgs e) => ContentLoadingHandler?.Invoke(sender, e);

        /// <inheritdoc cref="CoreWebView2.HistoryChanged" />
        public void OnHistoryChanged(object sender, object e) => HistoryChangedHandler?.Invoke(sender, e);

        /// <inheritdoc cref="CoreWebView2.WebMessageReceived" /> 
        public void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e) => WebMessageReceivedHandler?.Invoke(sender, e);

        #endregion

        /// <summary> 
        /// When executed, checks the sender object and retrieves the  <see cref="Microsoft.Web.WebView2.Core.CoreWebView2"/> object from the sender, and sets the <see cref="Core"/> property.
        /// <br/>This is typically executed once per instance of WebView2, after (during) the first navigation.
        /// </summary>
        public void OnCoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (sender is CoreWebView2 c1)
            {
                _core = c1;
            }
            else if (sender is Microsoft.Web.WebView2.Wpf.WebView2 c2)
            {
                _core = c2.CoreWebView2;
            }
            else if (sender is Microsoft.Web.WebView2.WinForms.WebView2 c3)
            {
                _core = c3.CoreWebView2;
            }
            OnPropertyChanged(nameof(IsInitialized));
            OnPropertyChanged(nameof(Core));
            reloadCommand?.NotifyCanExecuteChanged();
            CoreWebView2InitializationCompletedHandler?.Invoke(sender, e);
        }

        /// <inheritdoc cref="CoreWebView2.NavigationCompleted" />
        public void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            IsNavigating = false;
            NavigationCompletedHandler?.Invoke(sender, e);
            this.goBackCommand?.NotifyCanExecuteChanged();
            this.goForwardCommand?.NotifyCanExecuteChanged();
        }

        /// <inheritdoc cref="CoreWebView2.NavigationStarting" />
        public void OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            IsNavigating = true;
            NavigationStartingHandler?.Invoke(sender, e);
        }

        /// <inheritdoc cref="CoreWebView2.SourceChanged" />
        public void OnSourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            if (IsInitialized)
            {
                Uri = new Uri(Core.Source);
            }
            SourceChangedHandler?.Invoke(sender, e);
        }

        /// <summary>
        /// Attempts to navigate to the specified <paramref name="uri"/>.
        /// </summary>
        /// <remarks>
        /// When <see cref="IsInitialized"/> is <see langword="true"/>, calls <see cref="CoreWebView2.Navigate(string)"/>. <br/>The <see cref="Uri"/> will be updated after navigation has completed via <see cref="OnSourceChanged"/>.
        /// <para/>When <see cref="IsInitialized"/> is <see langword="false"/>, updates the <see cref="Uri"/> property directly.
        /// </remarks>
        /// <param name="uri"></param>
        [RelayCommand]
        public void Navigate(string uri = "about:blank")
        {
            var target = true switch
            {
                true when string.IsNullOrWhiteSpace(uri) => new Uri("about:blank"),
                true when UriConverter.TryConvert(uri, out var result) => result,
                _ => null
            };

            if (IsInitialized && target is not null)
            {
                Core.Navigate(target.ToString());
            }
            else if (target is not null)
            {
                Uri = target;
            }
        }

        /// <summary>
        /// This method will check for Core being initialized, and return once complete.
        /// <br/>If <see cref="OnCoreWebView2InitializationCompleted"/> is never triggered, this task will never complete.
        /// </summary>
        public async Task EnsureCoreWebView2Initialized()
        {
            if (Core is not null) return;
            if (Uri is null) Navigate("about:blank");
            var tcs = new TaskCompletionSource<bool>();
            void eval(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(Core) && Core is not null)
                    tcs.TrySetResult(true);
            }
            this.PropertyChanged += eval;
            await tcs.Task;
            this.PropertyChanged -= eval;
        }


        /// <inheritdoc cref="CoreWebView2.Stop"/>
        [RelayCommand(CanExecute = nameof(IsNavigating))]
        public void Stop()
        {
            if (IsInitialized)
            {
                try
                {
                    Core?.Stop();
                    _isStopped = true;
                    stopCommand?.NotifyCanExecuteChanged();
                    resumeCommand?.NotifyCanExecuteChanged();
                }
                finally { }
            }
        }

        /// <inheritdoc cref="CoreWebView2.Stop"/>
        [RelayCommand(CanExecute = nameof(CanResume))]
        public void Resume()
        {
            if (IsInitialized)
            {
                try
                {
                    Core.Resume();
                    _isStopped = false;
                    stopCommand?.NotifyCanExecuteChanged();
                    resumeCommand?.NotifyCanExecuteChanged();
                }
                finally { }
            }
        }

        /// <inheritdoc cref="CoreWebView2.Reload"/>
        [RelayCommand(CanExecute = nameof(IsInitialized))]
        public void Reload() => Core?.Reload();

        /// <inheritdoc cref="CoreWebView2.GoBack"/>
        [RelayCommand(CanExecute = nameof(CanGoBack))]
        public void GoBack() => Core?.GoBack();

        /// <inheritdoc cref="CoreWebView2.GoForward"/>
        [RelayCommand(CanExecute = nameof(CanGoForward))]
        public void GoForward() => Core?.GoForward();

        private bool CanResume() => _isStopped;
        private bool CanGoBack() => Core is not null && Core.CanGoBack;
        private bool CanGoForward() => Core is not null && Core.CanGoForward;
    }
}
