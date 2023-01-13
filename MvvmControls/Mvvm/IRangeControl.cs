using System;
using System.Windows.Controls.Primitives;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Interface for the definition of a control that interacts with a <see cref="System.Windows.Controls.Primitives.RangeBase"/>
    /// </summary>
    interface IRangeControl : IControlDefinition
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
}
