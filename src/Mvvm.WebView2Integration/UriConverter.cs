using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RFBCodeWorks.Mvvm.WebView2Integration
{
    /// <summary>
    /// Convert between a string and a URI
    /// </summary>
    [ValueConversion(typeof(string), typeof(Uri))]
    public sealed class UriConverter : IValueConverter
    {
        private static UriConverter _shared;
        
        /// <summary>Thread-Safe singleton</summary>
        public static UriConverter Shared => _shared ??= new UriConverter();

        /// <summary>Convert a Uri to a string</summary>
        /// <param name="value">the Uri to convert to a string representation</param>
        /// <param name="targetType"><see cref="string"/></param>
        /// <param name="parameter">unused</param>
        /// <param name="culture">unused</param>
        /// <returns>Either the <see cref="Uri.LocalPath"/> or <see cref="Uri.AbsoluteUri"/></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not Uri uri ? string.Empty : uri.IsFile ? uri.LocalPath : uri.AbsoluteUri;
        }

        /// <summary>Convert a string to a Uri</summary>
        /// <param name="value">the string to convert</param>
        /// <param name="targetType"><see cref="Uri"/></param>
        /// <param name="parameter">unused</param>
        /// <param name="culture">unused</param>
        /// <returns><see cref="Uri"/>?</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && TryConvert(s, out var result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Attempt to convert the <paramref name="input"/> to a valid URI. This accounts for lack of a 'http'\'ftp' prefix on the string, and can handle local files.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Uri Convert(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input cannot be null or whitespace.", nameof(input));
            }

            input = input.Trim();

            // Check if input is a valid web URL
            if (Uri.IsWellFormedUriString(input, UriKind.Absolute) &&
                (input.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                 input.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                 input.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase)))
            {
                return new Uri(input, UriKind.Absolute);
            }

            // If the input looks like a web URL but lacks a scheme, assume HTTP by default
            if (!input.StartsWith("file://", StringComparison.OrdinalIgnoreCase) && !(input.Length >=2 && input[1] == ':') && input.Contains(".") && !input.Contains(" "))
            {
                return new Uri($"http://{input}", UriKind.Absolute);
            }

            // Handle local file paths (absolute or relative)
            if (Uri.TryCreate(input, UriKind.RelativeOrAbsolute, out Uri uriResult) && uriResult.IsFile)
            {
                return uriResult;
            }

            // For relative file paths, combine with the current directory
            string combinedPath = System.IO.Path.GetFullPath(input);
            return new Uri(combinedPath);
        }

        /// <inheritdoc cref="Convert(string)"/>
        public static bool TryConvert(string input, out Uri uriResult)
        {
            try
            {
                uriResult = Convert(input);
                return true;
            }
            catch
            {
                uriResult = null;
                return false;
            }
        }
    }
}
