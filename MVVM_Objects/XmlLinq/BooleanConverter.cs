﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RFBCodeWorks.MVVMObjects.XmlLinq
{
    /// <summary>
    /// Interface that facilitates converting between a string and a boolean for use with the <see cref="XmlBooleanSetter" /> object
    /// </summary>
    public interface IBooleanConverter
    {
        /// <summary> Convert from the string to the boolean value </summary>
        /// <param name="value">the string value to convert</param>
        /// <returns>boolean representation of the <paramref name="value"/></returns>
        bool Convert(string value);

        /// <summary> Convert from the boolean to the string value </summary>
        /// <param name="value">the boolean value to convert</param>
        /// <returns>string representation of the <paramref name="value"/></returns>
        string Convert(bool value);
    }
     
    /// <summary>
    /// Convert to/from a boolean value and the xml representation of that value
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanConverter : IBooleanConverter, IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to either a '1' or a '0' where '1' == true
        /// </summary>
        public static BooleanConverter StoreAsBinary { get; } = new()
        {
            ConvertBool = b => b ? "1" : "0",
        };

        /// <summary>
        /// Converts a boolean to either 'true' or 'false' per the xml schema specification
        /// </summary>
        public static BooleanConverter StoreAsString { get; } = new()
        {
            ConvertBool = b => b ? "true" : "false",
        };

        /// <summary>
        /// Regex to identify a TRUE value - Accepts '1' or 'true', ignoring case.
        /// </summary>
        /// <remarks>
        /// ^[1]|[Tt][Rr][Uu][Ee]$
        /// </remarks>
        public static Regex TrueRegex { get; } = new("^[1]|[Tt][Rr][Uu][Ee]$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);

        /// <summary>
        /// Regex to identify a FALSE value - Accepts '0' or 'false', ignoring case.
        /// </summary>
        /// <remarks>
        /// ^[0]|[Ff][Aa][Ll][Ss][Ee]$
        /// </remarks>
        public static Regex FalseRegex { get; } = new("^[0]|[Ff][Aa][Ll][Ss][Ee]$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);

        /// <summary>
        /// The function that will convert the string value to a boolean value
        /// </summary>
        public Func<string, bool> ConvertString { get; init; } = DefaultStringConversion;

        /// <summary>
        /// The function that will convert the boolean value to a string value
        /// </summary>
        public Func<bool, string> ConvertBool { get; init; }

        /// <summary>
        /// Convert from the string to the boolean value
        /// </summary>
        /// <param name="value">the string value to convert</param>
        /// <returns>The result of <see cref="ConvertString"/></returns>
        public bool Convert(string value) => ConvertString.Invoke(value);

        /// <summary>
        /// Convert from the boolean to the string value
        /// </summary>
        /// <param name="value">the boolean value to convert</param>
        /// <returns>The result of <see cref="ConvertBool"/></returns>
        public string Convert(bool value) => ConvertBool.Invoke(value);

        /// <summary>
        /// Evaluate a string to determine if it represents a TRUE or FALSE value
        /// </summary>
        /// <param name="booleanString">the string to test, should be a 1 / 0 / true / false - ignored case</param>
        /// <returns>TRUE if the <see cref="TrueRegex"/> has a match, otherwise false</returns>
        public static bool DefaultStringConversion(string booleanString) => TrueRegex.IsMatch(booleanString ?? "");

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => ConvertBool?.Invoke((bool)value);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => ConvertString?.Invoke((string)value);
    }
}
