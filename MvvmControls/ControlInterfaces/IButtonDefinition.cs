using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Provide a Command and a ToolTip <br/>
    /// Implements:
    /// <br/> - <see cref="IRelayCommand"/>
    /// <br/> - <see cref="ICommand"/>
    /// <br/> - <see cref="IToolTipProvider"/>
    /// <br/> - <see cref="INotifyPropertyChanged"/>
    /// </summary>
    public interface IButtonDefinition : IRelayCommand, ICommand, IToolTipProvider, INotifyPropertyChanged, IDisplayTextProvider, IControlDefinition
    {
        /// <summary>
        /// Event Handler that allows an external event to raise CanExecuteChanged
        /// </summary>
        void NotifyCanExecuteChanged(object sender, EventArgs e);
    }
}
