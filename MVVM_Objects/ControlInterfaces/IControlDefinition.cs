using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects.ControlInterfaces
{
    /// <summary>
    /// base interface for control binding definitions
    /// </summary>
    public interface IControlDefinition : IToolTipProvider, INotifyPropertyChanged
    {
        /// <summary>
        /// Flag to set the visibility of the control
        /// </summary>
        System.Windows.Visibility Visibility { get; set; }

        /// <inheritdoc cref="BaseControlDefinition.IsVisible" path="*"/>
        bool IsVisible { get; set; }

        /// <inheritdoc cref="BaseControlDefinition.IsEnabled"/>
        bool IsEnabled { get; }

    }

    /// <summary>
    /// Interface that enforces the 'ToolTip' property
    /// </summary>
    public interface IToolTipProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Provide a tooltip for some UI control
        /// </summary>
        string ToolTip { get; }
    }

    /// <summary>
    /// Interface that provides a string of text to display to the user
    /// </summary>
    public interface IDisplayTextProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the text to be displayed to the user
        /// </summary>
        string DisplayText { get; }
    }
}
