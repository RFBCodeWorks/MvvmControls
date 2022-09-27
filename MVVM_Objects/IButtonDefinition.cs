using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.MVVMObjects
{
    /// <summary>
    /// Provide a Command and a ToolTip <br/>
    /// Implements:
    /// <br/> - <see cref="IRelayCommand"/>
    /// <br/> - <see cref="ICommand"/>
    /// <br/> - <see cref="IToolTipProvider"/>
    /// <br/> - <see cref="INotifyPropertyChanged"/>
    /// </summary>
    public interface IButtonDefinition : IRelayCommand, ICommand, IToolTipProvider, INotifyPropertyChanged, IButtonTextProvider, IControlDefinition
    {
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
    /// Interface that enforces the 'ButtonText' property
    /// </summary>
    public interface IButtonTextProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Provide the ButtonText string
        /// </summary>
        string ButtonText { get; }
    }

    /// <summary>
    /// Gets some text
    /// </summary>
    public interface IDisplayTextProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the text to be displayed to the user
        /// </summary>
        string DisplayText { get; }
    }
 
}
