using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using System;

namespace RFBCodeWorks.Mvvm.WebView2Integration
{
    /// <summary>
    /// Helper to determine which type of WebView2 installation is installed
    /// </summary>
    /// <remarks>
    /// See : <see href="https://stackoverflow.com/questions/64740327/detect-if-webview2-is-installed-on-clients-machine-vb-net"/>
    /// </remarks>
    public enum InstallType
    {
        /// <summary/>
        WebView2,
        /// <summary/>
        EdgeChromiumBeta,
        /// <summary/>
        EdgeChromiumCanary,
        /// <summary/>
        EdgeChromiumDev,
        /// <summary/>
        NotInstalled
    }

    /// <summary>
    /// Contains information about the current WebView2 installation, without spinning up a new WebView2 instance.
    /// </summary>
    /// <remarks>
    /// See : <see href="https://stackoverflow.com/questions/64740327/detect-if-webview2-is-installed-on-clients-machine-vb-net"/>
    /// </remarks>
    public class WebView2InstallInfo
    {
        /// <summary>
        /// Check if WebView2 runtime is installed.
        /// </summary>
        /// <returns></returns>
        public static bool CheckIfInstalled()
        {
            return GetInfo().IsInstalled;
        }

        /// <summary>
        /// Get the information about the current installation
        /// </summary>
        /// <returns></returns>
        public static WebView2InstallInfo GetInfo() => new WebView2InstallInfo(GetWebView2Version());

        /// <inheritdoc cref="CoreWebView2Environment.SetLoaderDllFolderPath"/>
        public static void SetLoaderDllFolderPath(string folderPath) => CoreWebView2Environment.SetLoaderDllFolderPath(folderPath);

        private static string GetWebView2Version()
        {
            try
            {
                return CoreWebView2Environment.GetAvailableBrowserVersionString();
            }
            catch (Exception) { return ""; }
        }

        private WebView2InstallInfo(string version)
        {
            Version = version;
        }

        /// <summary>
        /// The string reported by <see cref="CoreWebView2Environment.GetAvailableBrowserVersionString(string)"/>
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Check if the <see cref="InstallType"/> is not <see cref="InstallType.NotInstalled"/>
        /// </summary>
        public bool IsInstalled => InstallType != InstallType.NotInstalled;

        /// <summary>
        /// Gets the <see cref="RFBCodeWorks.Mvvm.WebView2Integration.InstallType"/>
        /// </summary>
        public InstallType InstallType => Version switch
        {
            var version when version.Contains("dev") => InstallType.EdgeChromiumDev,
            var version when version.Contains("beta") => InstallType.EdgeChromiumBeta,
            var version when version.Contains("canary") => InstallType.EdgeChromiumCanary,
            var version when !string.IsNullOrEmpty(version) => InstallType.WebView2,
            _ => InstallType.NotInstalled
        };

        /// <summary>
        /// Check the Windows Registry to detect if the system has WebView2 registered
        /// </summary>
        public bool IsRegistryKeyDetected => regKeyDetected ??= (DetectRegKey(RegistryKey) || DetectRegKey(RegistryKeyCurrentUser));
        private bool? regKeyDetected;

        /// <summary>The first registry key that will be checked</summary>
        public const string RegistryKey = @"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients";
        
        /// <summary>The alternate registry key that will be checked</summary>
        public const string RegistryKeyCurrentUser = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}";

        /// <summary>
        /// Check if the registry key for Edge is detected
        /// </summary>
        /// <returns>True if the key exists and has any subkeys, otherwise false</returns>
        public static bool DetectRegKey(string key)
        {
            using RegistryKey edgeKey = Registry.LocalMachine.OpenSubKey(key);
            if (edgeKey != null)
            {
                return edgeKey.SubKeyCount > 0;
            }
            return false;
        }
    }
}
