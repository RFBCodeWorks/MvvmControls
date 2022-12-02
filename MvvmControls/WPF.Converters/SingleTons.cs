using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  RFBCodeWorks.WPF.Converters
{
    public static class SingleTons
    {
        public static MathConverter MathConverter { get; } = new();

        public static InverseBooleanConverter InverseBooleanConverter { get; } = new();

        public static MultiValueConverter_TakeFirstValue GetFirstValueFromMultiBind { get; } = new();

        public static ToStringConverter ToStringConverter { get; } = new();

        public static StringToInt StringToIntConverter { get; } = new();

    }
}
