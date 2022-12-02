using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RFBCodeWorks.WPF.Converters
{

    public static class StringConverters
    {

        public static BulletPointPrefixConverter GetBulletPointPrefixConverter { get; } = new();
        public static StringFormatConverter StringFormatter { get; } = new();

        /// <summary>
        /// Adds a bullet point to the specified string
        /// </summary>
        [ValueConversion(typeof(string), typeof(string))]
        public class BulletPointPrefixConverter : BaseConverter
        {
            const string BP = "☻";

            public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var v = value as string;
                if (v.IsNullOrEmpty()) return BP;
                return BP + " " + v.Trim();
            }

            public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var v = (value as string)?.Trim();
                if (v.IsNullOrEmpty()) return string.Empty;
                if (v.StartsWith(BP) && v.Length > 1) return v.Substring(1);
                return v;
            }
        }

        /// <summary>
        /// Formats a string
        /// </summary>
        [ValueConversion(typeof(string), typeof(string))]
        public class StringFormatConverter : BaseConverter
        {
            /// <summary>
            /// Convert a value to a formatted string
            /// </summary>
            /// <param name="value">The object/string value to format</param>
            /// <param name="parameter">The string format to use</param>
            /// <returns>The formatted string</returns>
            /// <inheritdoc/>
            /// <param name="culture"/><param name="targetType"/>
            public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return string.Format(parameter as string, value);
            }

            /// <summary> ConvertBack simply returns the input string value </summary>
            /// <returns>The <paramref name="value"/></returns>
            public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value;
            }
        }
    }
}
