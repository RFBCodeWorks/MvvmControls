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
    /// This is a <see cref="MathConverter"/> that returns a <see cref="GridLength"/> instead of an <see cref="Int32"/>
    /// </summary>
    [ValueConversion(typeof(int), typeof(GridLength))]
    public class GridLengthConverter : MathConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new GridLength((int)base.Convert(value, targetType, parameter, culture));
        }
    }
}
