using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace RFBCodeWorks.WPF.Converters
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public sealed class InverseBooleanConverter : BaseConverter
    {
        /// <inheritdoc/>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null ? false : !(bool)value;
        }

        /// <inheritdoc/>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null ? false : !(bool)value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public BooleanToVisibilityConverter()
        {
            converter = new();
        }

        private System.Windows.Controls.BooleanToVisibilityConverter converter { get; }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((IValueConverter)converter).Convert(value, targetType, parameter, culture);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((IValueConverter)converter).ConvertBack(value, targetType, parameter, culture);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BooleanToHiddenConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public BooleanToHiddenConverter()
        {
            converter = new();
        }

        private System.Windows.Controls.BooleanToVisibilityConverter converter { get; }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility ret = (Visibility)((IValueConverter)converter).Convert(value, targetType, parameter, culture);
            if (ret == Visibility.Collapsed) return Visibility.Hidden;
            return ret;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Hidden) value = Visibility.Collapsed;
            return ((IValueConverter)converter).ConvertBack(value, targetType, parameter, culture);
        }
    }
}
