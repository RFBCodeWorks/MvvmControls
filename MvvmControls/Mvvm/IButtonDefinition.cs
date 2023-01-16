using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Provide the details about a Button
    /// <br/> Implements:
    /// <br/> - <see cref="ICommand"/>
    /// <br/> - <see cref="CommunityToolkit.Mvvm.Input.IRelayCommand"/>
    /// <br/> - <see cref="INotifyPropertyChanged"/>
    /// <br/> - <see cref="IToolTipProvider"/>
    /// <br/> - <see cref="IDisplayTextProvider"/>
    /// </summary>
    public interface IButtonDefinition : ICommand, IRelayCommand, CommunityToolkit.Mvvm.Input.IRelayCommand, INotifyPropertyChanged, IControlDefinition, IDisplayTextProvider
    {
        
    }

    /// <summary>
    /// <inheritdoc cref="IButtonDefinition" path="/summary"/>
    /// <br/> - <see cref="IRelayCommand{T}"/>
    /// </summary>
    public interface IButtonDefinition<T> : IButtonDefinition, IRelayCommand<T>, CommunityToolkit.Mvvm.Input.IRelayCommand<T>
    {

    }
}
