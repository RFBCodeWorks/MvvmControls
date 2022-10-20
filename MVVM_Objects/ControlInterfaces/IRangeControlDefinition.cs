using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace RFBCodeWorks.MVVMObjects.ControlInterfaces
{
    /// <summary>
    /// Interface for the definition of a control that interacts with a <see cref="System.Windows.Controls.Primitives.RangeBase"/>
    /// </summary>
    interface IRangeControlDefinition : IControlDefinition
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
