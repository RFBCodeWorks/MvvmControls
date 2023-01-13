using System.ComponentModel;

namespace RFBCodeWorks.Mvvvm
{
    /// <summary>
    /// base interface for control binding definitions
    /// </summary>
    /// <remarks>
    /// Inherits: 
    /// <br/> - <see cref="IToolTipProvider"/>
    /// <br/> - <see cref="INotifyPropertyChanged"/>
    /// </remarks>
    public interface IControlDefinition : IToolTipProvider, INotifyPropertyChanged
    {
        /// <summary>
        /// Flag to set the visibility of the control
        /// </summary>
        System.Windows.Visibility Visibility { get; set; }

        /// <inheritdoc cref="Primitives.ControlBase.IsVisible" path="*"/>
        bool IsVisible { get; set; }

        /// <inheritdoc cref="Primitives.ControlBase.IsEnabled"/>
        bool IsEnabled { get; }

    }

    /// <summary>
    /// Interface that enforces the 'ToolTip' property
    /// </summary>
    /// <remarks>
    /// Inherits: 
    /// <br/> - <see cref="INotifyPropertyChanged"/>
    /// </remarks>
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
    /// <remarks>
    /// Inherits: 
    /// <br/> - <see cref="INotifyPropertyChanged"/>
    /// </remarks>
    public interface IDisplayTextProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the text to be displayed to the user
        /// </summary>
        /// <remarks>
        /// For Content Controls, such as <see cref="System.Windows.Controls.Button"/>, this would be equivalent to the 'Content' property
        /// </remarks>
        string DisplayText { get; }
    }
}
