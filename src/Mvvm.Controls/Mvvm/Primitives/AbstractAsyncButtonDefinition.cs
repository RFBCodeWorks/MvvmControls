using CommunityToolkit.Mvvm.Input;
using RFBCodeWorks.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

#nullable enable

namespace RFBCodeWorks.Mvvm.Primitives
{
    /// <summary>
    /// Abstract base object that inherits from the following:
    /// <br/> - <see cref="ControlBase"/>
    /// <br/> - <see cref="IRelayCommand"/>
    /// <br/> - <see cref="ICommand"/> - Explicitly implemented. <see cref="ICommand.Execute(object)"/> will execute synchronously unless derived class overrides the ICommand.Execute(object) implementation.
    /// <br/> - <see cref="IToolTipProvider"/>
    /// <br/> - <see cref="INotifyPropertyChanged"/>
    /// <br/> - <see cref="IButtonDefinition"/>
    /// </summary>
    public abstract class AbstractAsyncButtonDefinition : ControlBase, IButtonDefinition, ICommand
    {
        private string _displayText = string.Empty;

        /// <inheritdoc/>
        public virtual string DisplayText
        {
            get => _displayText;
            set
            {
                if (!EqualityComparer<string>.Default.Equals(_displayText, value))
                {
                    OnPropertyChanging(EventArgSingletons.DisplayText);
                    _displayText = value;
                    OnPropertyChanged(EventArgSingletons.DisplayText);
                }
            }
        }

        /// <inheritdoc cref="CommunityToolkit.Mvvm.Input.RelayCommand{T}.CanExecute(T)"/>
        public abstract bool CanExecute();

        /// <summary> The method through which the abstract base object implements <see cref="ICommand.Execute(object)"/> </summary>
        public abstract Task ExecuteAsync();

        /// <summary> Cancel the task </summary>
        public abstract void Cancel();

        /// <inheritdoc/>
        public abstract void NotifyCanExecuteChanged();

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged(object? sender, EventArgs e) => NotifyCanExecuteChanged();

        /// <inheritdoc/>
        public abstract event EventHandler? CanExecuteChanged;

        void ICommand.Execute(object? parameter)
        {
            // same as community toolkit
            // https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/da6d7d3f6ca9914dbe86d7d394e9a4abef25c9b6/Microsoft.Toolkit.Mvvm/Input/AsyncRelayCommand.cs#L147
            _ = ExecuteAsync();
        }

        bool ICommand.CanExecute(object? parameter)
        {
            base.IsEnabled = CanExecute();
            return base.IsEnabled;
        }
    }

}
