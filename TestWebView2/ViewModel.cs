using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.WPF.WebView2Integration.Tester
{
    // https://stackoverflow.com/questions/64740327/detect-if-webview2-is-installed-on-clients-machine-vb-net

    class ViewModel : RFBCodeWorks.Mvvm.ViewModelBase
    {
        public ViewModel()
        {
            NavHandler = new NavigationHandler()
            {
                NavigationStartingHandler = (o, e) => SetProperty(ref NavStartedField, true, nameof(NavStarted)),
                NavigationCompletedHandler = (o, e) => SetProperty(ref NavCompletedField, true, nameof(NavCompleted)),
                ContentLoadingHandler = (o, e) => SetProperty(ref ContentLoadingField, true, nameof(ContentLoaded)),
                HistoryChangedHandler = (o, e) => SetProperty(ref historyChangedField, true, nameof(HistoryChanged)),
                SourceChangedHandler = (o, e) => SetProperty(ref sourceChangedField, true, nameof(SourceChanged)),
                CoreWebView2InitializationCompletedHandler = (o, e) => SetProperty(ref initializationCompleteField, true, nameof(IsInitialized)),
                WebMessageReceivedHandler = (o, e) => SetProperty(ref WebMessageReceivedField, true, nameof(WebMessageReceived)),
            };
        }
        
        private static WebView2InstallInfo InstallInfo { get; } = WebView2InstallInfo.GetInfo();

        public string InstalledVersion { get; } = string.Format("Version Installed : {0}", InstallInfo.Version);

        public string InstallType { get; } = string.Format("Install Type : {0}", InstallInfo.InstallType);

        public string IsEdgeClientInstalled { get; } = string.Format("Discovered in Registry : {0}", InstallInfo.IsRegistryKeyDetected);

        public Uri PageToLoad { get; } = new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, "TestPage.html"));


        public NavigationHandler NavHandler { get; }

        public bool NavStarted => NavStartedField;
        public bool NavCompleted => NavCompletedField;
        public bool ContentLoaded => ContentLoadingField;
        public bool HistoryChanged => historyChangedField;
        public bool SourceChanged => sourceChangedField;
        public bool IsInitialized => initializationCompleteField;
        public bool WebMessageReceived => WebMessageReceivedField;

        bool NavStartedField;
        bool NavCompletedField;
        bool ContentLoadingField;
        bool historyChangedField;
        bool sourceChangedField;
        bool initializationCompleteField;
        bool WebMessageReceivedField;


    }
}