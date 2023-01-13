using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvvm
{
    /// <summary>
    /// Provide the details about a Button
    /// <br/> Implements:
    /// <br/> - <see cref="ICommand"/>
    /// <br/> - <see cref="Microsoft.Toolkit.Mvvm.Input.IRelayCommand"/>
    /// <br/> - <see cref="INotifyPropertyChanged"/>
    /// <br/> - <see cref="IToolTipProvider"/>
    /// <br/> - <see cref="IDisplayTextProvider"/>
    /// </summary>
    public interface IButtonDefinition : ICommand, IRelayCommand, Microsoft.Toolkit.Mvvm.Input.IRelayCommand, INotifyPropertyChanged, IControlDefinition, IDisplayTextProvider
    {
        
    }

    /// <summary>
    /// <inheritdoc cref="IButtonDefinition" path="/summary"/>
    /// <br/> - <see cref="IRelayCommand{T}"/>
    /// </summary>
    public interface IButtonDefinition<T> : IButtonDefinition, IRelayCommand<T>, Microsoft.Toolkit.Mvvm.Input.IRelayCommand<T>
    {

    }
}
