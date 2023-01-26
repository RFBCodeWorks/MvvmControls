using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  RFBCodeWorks.WPF.Converters
{
    /// <summary>
    /// Contains various Converters as static singletons
    /// </summary>
    public static class SingleTons
    {
        /// <inheritdoc cref="RFBCodeWorks.WPF.Converters.MathConverter"/>
        public static MathConverter MathConverter { get; } = new();

        /// <inheritdoc cref="RFBCodeWorks.WPF.Converters.InverseBooleanConverter"/>
        public static InverseBooleanConverter InverseBooleanConverter { get; } = new();

        /// <inheritdoc cref="RFBCodeWorks.WPF.Converters.MultiValueConverter_TakeFirstValue"/>
        public static MultiValueConverter_TakeFirstValue GetFirstValueFromMultiBind { get; } = new();

        /// <inheritdoc cref="RFBCodeWorks.WPF.Converters.ToStringConverter"/>
        public static ToStringConverter ToStringConverter { get; } = new();

        /// <inheritdoc cref="RFBCodeWorks.WPF.Converters.StringToInt"/>
        public static StringToInt StringToIntConverter { get; } = new();

        /// <inheritdoc cref="BulletPointPrefixConverter"/>
        public static BulletPointPrefixConverter BulletPointPrefixer { get; } = new();

        /// <inheritdoc cref="StringFormatConverter"/>
        public static StringFormatConverter StringFormatter { get; } = new();
    }
}
