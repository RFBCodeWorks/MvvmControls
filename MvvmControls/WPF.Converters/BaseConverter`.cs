using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace RFBCodeWorks.WPF.Converters
{
    /// <summary>
    /// A strongly typed version of <see cref="IValueConverter"/>
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface IValueConverter<T1, T2> : IValueConverter
    {
        /// <summary>Convert Data from a source (such as a ViewModel) to an output data (for the view)
        /// <br/> Example: Convert boolean true/false to <see cref="System.Windows.Visibility"/>
        /// </summary>
        /// <inheritdoc cref="IValueConverter.Convert(object, Type, object, CultureInfo)"/>
        T2 Convert(T1 value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>Convert Data back to the original value (example being Converting Control.Visibility to a boolean)</summary>
        /// <inheritdoc cref="IValueConverter.ConvertBack(object, Type, object, CultureInfo)"/>
        T1 ConvertBack(T2 value, Type targetType, object parameter, CultureInfo culture);
    }

    /// <summary>
    /// Abstract Base Class for <see cref="IValueConverter"/> interface for strongly typed converters
    /// </summary>
    /// <remarks>
    /// <see href="http://www.wpftutorial.net/ValueConverters.html"/>
    /// </remarks>
    public abstract class BaseConverter<T1, T2> : IValueConverter<T1, T2>
    {
        /// <inheritdoc/>
        public abstract T2 Convert(T1 value, Type targetType, object parameter, CultureInfo culture);

        /// <inheritdoc/>
        public abstract T1 ConvertBack(T2 value, Type targetType, object parameter, CultureInfo culture);

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((T1)value, targetType, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((T2)value, targetType, parameter, culture);
        }
    }
}
