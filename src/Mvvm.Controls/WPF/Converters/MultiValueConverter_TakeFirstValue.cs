using System;
using System.Globalization;
using System.Windows.Data;

namespace RFBCodeWorks.WPF.Converters
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(object), typeof(object))]
    public class MultiValueConverter_TakeFirstValue : BaseMultiConverter
    {
        /// <inheritdoc/>
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object i in values)
                if (!(i is null))
                    return i;
            return default;
        }

        /// <inheritdoc/>
        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { value };
        }
    }
}
