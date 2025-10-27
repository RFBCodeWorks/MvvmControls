using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using RFBCodeWorks.Mvvm.Primitives;

using Toolkit = CommunityToolkit.Mvvm.Input;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// RelayCommand that that inherits from <see cref="ControlBase"/>
    /// <para>
    /// Implements : 
    /// <br/> - <see cref="IRelayCommand"/>
    /// <br/> - <see cref="ICommand"/> - This is explicitly implemented via the protected abstract methods
    /// <br/> - <see cref="IToolTipProvider"/>
    /// <br/> - <see cref="INotifyPropertyChanged"/>
    /// <br/> - <see cref="IButtonDefinition"/>
    /// </para>
    /// </summary>
    public sealed class ButtonDefinition<T> : ControlBase, IButtonDefinition, IButtonDefinition<T>, ICommand, IDisplayTextProvider, Input.IRelayCommand<T>, Toolkit.IRelayCommand<T>, Toolkit.IRelayCommand
    {
        private readonly Toolkit.IRelayCommand<T>? _command;
        private readonly Action<T>? _execute;
        private readonly Func<T, bool>? _canExecute;
        private bool _enabled = true;
        private string _displayText;

        private event EventHandler? PrivateCanExecuteChanged;

        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_command is null)
                {
                    PrivateCanExecuteChanged += value;
                }
                else
                {
                    _command.CanExecuteChanged += value;
                }

            }
            remove
            {
                if (_command is null)
                {
                    PrivateCanExecuteChanged -= value;
                }
                else
                {
                    _command.CanExecuteChanged -= value;
                }
            }
        }


        /// <inheritdoc cref="ButtonDefinition.ButtonDefinition(Action, Func{bool})"/>
        public ButtonDefinition(Action<T> execute) : this(execute, ReturnTrue) { }

        /// <summary>
        /// Create a new ButtonDefinition using the specified <paramref name="execute"/> action
        /// </summary>
        /// <inheritdoc cref="Toolkit.RelayCommand.RelayCommand(Action, Func{bool})"/>
        public ButtonDefinition(Action<T> execute, Func<T,bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Create a new ButtonDefinition from the specified IRelayCommand
        /// </summary>
        /// <param name="command">the command</param>
        public ButtonDefinition(Toolkit.IRelayCommand<T> command)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _command.CanExecuteChanged += (_, _) => NotifyCanExecuteChangedPrivate();
        }

        /// <summary>
        /// Returns the result of the last <see cref="ICommand.CanExecute(object)"/> call.
        /// </summary>
        /// <remarks>
        /// By default, this will return the last result of the <see cref="CanExecute(T)"/> evaluation.<br/>
        /// If set to false in code, disables the button entirely, including ignoring checks for <see cref="CanExecute(T)"/>
        /// </remarks>
        public override bool IsEnabled 
        { 
            get => _enabled && base.IsEnabled; 
            set { 
                _enabled = value; 
                base.IsEnabled = value; 
            }  
        }

        /// <inheritdoc/>
        public string DisplayText
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

        /// <summary>
        /// The IRelayCommand object through which the <see cref="ICommand"/> interface is implemented
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("property has been deprecated.", true)]
        public Toolkit.IRelayCommand<T> Command => _command ?? this;
        
        /// <inheritdoc cref="Toolkit.RelayCommand{T}.CanExecute(T)"/>
        public bool CanExecute(T parameter)
        {
            if (_enabled is false) return false;
            bool result =  _command is null ? _canExecute(parameter) : _command.CanExecute(parameter);
            base.IsEnabled = result;
            return result;
        }

        /// <inheritdoc cref="Toolkit.RelayCommand{T}.Execute(T)"/>
        public void Execute(T parameter)
        {
            if (_command is null) 
                _execute(parameter);
            else
                _command.Execute(parameter);
        }

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged()
        {
            if (_command is null)
            {
                NotifyCanExecuteChangedPrivate();
            }
            else
            {
                _command.NotifyCanExecuteChanged();
            }
        }

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged(object sender, EventArgs e) => NotifyCanExecuteChanged();
        private void NotifyCanExecuteChangedPrivate() => PrivateCanExecuteChanged?.Invoke(this, EventArgs.Empty);

        void ICommand.Execute(object parameter) => Execute(AbstractCommand<T>.ThrowIfInvalidParameter(parameter));
        bool ICommand.CanExecute(object parameter)
        {
            // Special case a null value for a value type argument type.
            // This ensures that no exceptions are thrown during initialization.
            //https://github.com/CommunityToolkit/dotnet/blob/e8969781afe537ea41a964a15b4ccfee32e095df/src/CommunityToolkit.Mvvm/Input/RelayCommand%7BT%7D.cs#L87
            if (parameter is null && default(T) is not null)
            {
                return false;
            }
            base.IsEnabled = CanExecute(AbstractCommand<T>.ThrowIfInvalidParameter(parameter));
            return IsEnabled;
        }
    }
}
