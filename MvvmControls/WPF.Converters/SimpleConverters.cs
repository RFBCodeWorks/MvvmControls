using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RFBCodeWorks.WPF.Converters
{
    /// <summary>
    /// Always returns the string representation of the input value
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public sealed class ToStringConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// Always returns the string representation of the input value
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public sealed class StringToInt : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }
    }

}
