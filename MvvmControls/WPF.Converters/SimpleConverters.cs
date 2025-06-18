using System;
using System.Globalization;
using System.Windows.Data;

namespace RFBCodeWorks.WPF.Converters
{
    /// <summary>
    /// Always returns the string representation of the input value
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public sealed class ToStringConverter : BaseConverter
    {
        /// <inheritdoc/>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value);
        }

        /// <inheritdoc/>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }
    }

}
