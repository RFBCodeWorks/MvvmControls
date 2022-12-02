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
    /// Provide support for the automatic styles subset of <see cref="GridUnitType"/>
    /// </summary>
    public enum AutoGridLength
    {
        /// <inheritdoc cref="GridUnitType.Auto"/>
        Auto,
        /// <inheritdoc cref="GridUnitType.Star"/>
        Star
    }

    /// <summary>
    /// Class to convert between <see cref="AutoGridLength"/> and <see cref="GridUnitType"/>
    /// </summary>
    [ValueConversion(typeof(AutoGridLength), typeof(GridUnitType))]
    public class AutoGridLengthConverter : IValueConverter
    {
        #region CTORS & Properties 

        /// <summary>
        /// Initializer used for IValueConverter interface
        /// </summary>
        public AutoGridLengthConverter() { }

        /// <summary>
        /// Create a new converter struct object of the specified type
        /// </summary>
        /// <param name="unitType"></param>
        public AutoGridLengthConverter(AutoGridLength unitType) { UnitType = unitType; }

        /// <inheritdoc cref="GridUnitType.Auto"/>
        public static AutoGridLengthConverter Auto { get; } = new(AutoGridLength.Auto);

        /// <inheritdoc cref="GridUnitType.Star"/>
        public static AutoGridLengthConverter Star { get; } = new(AutoGridLength.Star);

        private AutoGridLength UnitType;

        #endregion

        #region Implicit / Explicit Conversion 


        /// <summary>
        /// Implicit conversion to the <see cref="GridUnitType"/>
        /// </summary>
        public static implicit operator GridUnitType(AutoGridLengthConverter e) => e.UnitType == AutoGridLength.Auto ? GridUnitType.Auto : GridUnitType.Star;

        /// <summary>
        /// Implicit conversion to the <see cref="AutoGridLength"/>
        /// </summary>
        public static implicit operator AutoGridLength(AutoGridLengthConverter e) => e.UnitType;

        /// <summary>
        /// Implicit conversion from the <see cref="AutoGridLength"/>
        /// </summary>
        public static implicit operator AutoGridLengthConverter(AutoGridLength e) => new AutoGridLengthConverter(e);

        /// <summary>
        /// Explicit conversion from the <see cref="GridUnitType"/>
        /// </summary>
        public static explicit operator AutoGridLengthConverter(GridUnitType e)
        {
            if (e == GridUnitType.Auto) return Auto;
            if (e == GridUnitType.Star) return Star;
            throw new InvalidCastException("Expected either 'Star' or 'Auto' value, received 'Pixel'.");
        }

        #endregion

        //#region Comparisons

        ///// <summary>
        ///// Compare the underlying values of the <see cref="AutoGridLength"/> objects
        ///// </summary>
        ///// <returns>TRUE if the underlying values are the same. Otherwise false.</returns>
        //public static bool operator ==(AutoGridLengthConverter obj1, AutoGridLengthConverter obj2)
        //{
        //    return obj1?.UnitType == obj2?.UnitType;
        //}

        ///// <summary>
        ///// Compare the underlying values of the <see cref="AutoGridLength"/> objects
        ///// </summary>
        ///// <returns>TRUE if the underlying values are the different. Otherwise false.</returns>
        //public static bool operator !=(AutoGridLengthConverter obj1, AutoGridLengthConverter obj2)
        //{
        //    return obj1?.UnitType != obj2?.UnitType;
        //}

        ///// <inheritdoc/>
        //public override bool Equals(object obj)
        //{
        //    if (obj is AutoGridLength | obj is GridUnitType | obj is AutoGridLengthConverter)
        //    {
        //        return this.UnitType == (AutoGridLengthConverter)obj;
        //    }
        //    else
        //        return base.Equals(obj);
        //}

        //#endregion

        #region IValueConverter

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AutoGridLength) return (GridUnitType)((AutoGridLengthConverter)value);
            if (value is GridUnitType) return (AutoGridLength)((AutoGridLengthConverter)value);
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((IValueConverter)this).Convert(value, targetType, parameter, culture);
        }

        #endregion

    }
}
