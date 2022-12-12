using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RFBCodeWorks.WPF.Converters
{

    /// <summary>
    /// Abstract base class to be used with the <see cref="Controls.MaskedTextBox"/>
    /// </summary>
    public abstract class MaskedTextBoxMask : IValueConverter<string, string>
    {
        /// <summary>
        /// Convert the text into its masked format. This will be called by the <see cref="Controls.MaskedTextBox"/> every time the text is updated.
        /// </summary>
        /// <param name="text">the text to mask</param>
        /// <returns>The masked text</returns>
        public abstract string Mask(string text);

        /// <summary>
        /// Get the value of the text as if it had never been masked
        /// </summary>
        /// <param name="maskedText">the masked form of the text</param>
        /// <returns>The unmasked text</returns>
        public abstract string UnMask(string maskedText);

        string IValueConverter<string, string>.Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return Mask(value);
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Mask(value as string);
        }

        string IValueConverter<string, string>.ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return UnMask(value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return UnMask(value as string);
        }
    }
}
