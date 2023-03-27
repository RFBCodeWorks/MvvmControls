using System;
using System.Windows.Controls.Primitives;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Interface for the definition of a control that interacts the following classes:
    /// <br/> - <see cref="System.Windows.Controls.Primitives.RangeBase"/>
    /// <br/> - <see cref="RFBCodeWorks.WPF.Controls.DecimalUpDown"/>
    /// <br/> - <see cref="RFBCodeWorks.WPF.Controls.IntegerUpDown"/>
    /// </summary>
    public interface IRangeControl : IControlDefinition
    {
        /// <inheritdoc cref="RangeBase.ValueChanged"/>
        event EventHandler ValueChanged;

        /// <inheritdoc cref="RangeBase.Minimum"/>
        double Minimum { get; set; }

        /// <inheritdoc cref="RangeBase.Maximum"/>
        double Maximum { get; set; }

        /// <inheritdoc cref="RangeBase.SmallChange"/>
        double SmallChange { get; set; }

        /// <inheritdoc cref="RangeBase.LargeChange"/>
        double LargeChange { get; set; }

        /// <inheritdoc cref="RangeBase.Value"/>
        double Value { get; set; }

    }

    ///// <summary>
    ///// A strongly-typed version of the <see cref="IRangeControl"/> interface, that allows conversion to the other numeric values
    ///// </summary>
    ///// <typeparam name="T">The type of value (<see cref="int"/>, <see cref="double"/>, <see cref="long"/>, etc )</typeparam>
    //public interface IRangeControl<T> : IRangeControl
    //    where T: struct, IComparable<T>, IEquatable<T>, IFormattable
    //{
    //    /// <inheritdoc cref="RangeBase.Minimum"/>
    //    new T Minimum { get; set; }

    //    /// <inheritdoc cref="RangeBase.Maximum"/>
    //    new T Maximum { get; set; }

    //    /// <inheritdoc cref="RangeBase.SmallChange"/>
    //    new T SmallChange { get; set; }

    //    /// <inheritdoc cref="RangeBase.LargeChange"/>
    //    new T LargeChange { get; set; }

    //    /// <inheritdoc cref="RangeBase.Value"/>
    //    new T Value { get; set; }
    //}

}
