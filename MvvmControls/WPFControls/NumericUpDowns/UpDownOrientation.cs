using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.WPFControls
{
    /// <summary>
    /// Enum describing the various orientations of the NumericUpDown controls
    /// </summary>
    public enum UpDownOrientation
    {
        /// <summary>
        /// Buttons will be on the right of the textbox
        /// </summary>
        OnRight,
        /// <summary>
        /// Buttons will be on the sides of the textbox
        /// </summary>
        OnSides,
        /// <summary>
        /// Buttons will be on the Above/Below the textbox
        /// </summary>
        Stacked,
        /// <summary>
        /// No buttons will be visible
        /// </summary>
        NoButtons
    }
}
