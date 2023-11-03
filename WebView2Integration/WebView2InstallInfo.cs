using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace RFBCodeWorks.WPF.WebView2Integration
{
    public enum InstallType
    {
        WebView2, EdgeChromiumBeta, EdgeChromiumCanary, EdgeChromiumDev, NotInstalled
    }

    /// <summary>
    /// Contains information about the current WebView2 installation
    /// </summary>
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
        /// The string reported by <see cref="CoreWebView2Environment.GetAvailableBrowserVersionString"/>
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Check if the <see cref="InstallType"/> is not <see cref="InstallType.NotInstalled"/>
        /// </summary>
        public bool IsInstalled => InstallType != InstallType.NotInstalled;

        /// <summary>
        /// Gets the <see cref="RFBCodeWorks.WPF.WebView2Integration.InstallType"/>
        /// </summary>
        public InstallType InstallType => Version switch
        {
            var version when version.Contains("dev") => InstallType.EdgeChromiumDev,
            var version when version.Contains("beta") => InstallType.EdgeChromiumBeta,
            var version when version.Contains("canary") => InstallType.EdgeChromiumCanary,
            var version when !string.IsNullOrEmpty(version) => InstallType.WebView2,
            _ => InstallType.NotInstalled
        };

        public bool IsRegistryKeyDetected => regKeyDetected ??= DetectRegKey();
        private bool? regKeyDetected;


        /// <summary>
        /// Check if the registry key for Edge is detected
        /// </summary>
        /// <returns></returns>
        private static bool DetectRegKey()
        {
            string regKey = @"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients";
            string regKey2 = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}";
            return CheckKey(regKey) || CheckKey(regKey2);

            bool CheckKey(string key)
            {
                using (RegistryKey edgeKey = Registry.LocalMachine.OpenSubKey(regKey))
                {
                    if (edgeKey != null)
                    {
                        string[] productKeys = edgeKey.GetSubKeyNames();
                        if (productKeys.Any())
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}
