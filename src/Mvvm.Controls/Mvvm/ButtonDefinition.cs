using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using RFBCodeWorks.Mvvm.Primitives;

using Toolkit = CommunityToolkit.Mvvm.Input;
using System.Windows;

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
    public sealed class ButtonDefinition : ControlBase, IButtonDefinition, ICommand, IDisplayTextProvider, Input.IRelayCommand, Toolkit.IRelayCommand
    {
        private readonly Toolkit.IRelayCommand? _command;
        private readonly Action? _execute;
        private readonly Func<bool>? _canExecute;
        private bool _enabled = true;
        private string _displayText;

        private event EventHandler PrivateCanExecuteChanged;

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
        public ButtonDefinition(Action execute) : this(execute, ReturnTrue) { }

        /// <summary>
        /// Create a new ButtonDefinition using the specified <paramref name="execute"/> action
        /// </summary>
        /// <inheritdoc cref="Toolkit.RelayCommand.RelayCommand(Action, Func{bool})"/>
        public ButtonDefinition(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Create a new ButtonDefinition from the specified IRelayCommand
        /// </summary>
        /// <param name="command">the command</param>
        public ButtonDefinition(Toolkit.IRelayCommand command)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _command.CanExecuteChanged += (_, _) => NotifyCanExecuteChangedPrivate();
        }

        /// <summary>
        /// Returns the result of the last <see cref="ICommand.CanExecute(object)"/> call.
        /// </summary>
        /// <remarks>
        /// By default, this will return the last result of the <see cref="CanExecute"/> evaluation.<br/>
        /// If set to false in code, disables the button entirely, including ignoring checks for <see cref="CanExecute"/>
        /// </remarks>
        public override bool IsEnabled
        {
            get => _enabled && base.IsEnabled;
            set
            {
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
        public Toolkit.IRelayCommand Command => _command ?? this;


        /// <inheritdoc cref="Toolkit.RelayCommand{T}.CanExecute(T)"/>
        public bool CanExecute(object? parameter)
        {
            if (_enabled is false) return false;
            bool result = _command is null ? _canExecute() : _command.CanExecute(parameter);
            base.IsEnabled = result;
            return result;
        }

        /// <inheritdoc cref="Toolkit.RelayCommand{T}.Execute(T)"/>
        public void Execute()
        {
            if (_command is null)
                _execute();
            else
                _command.Execute(null);
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
        public void NotifyCanExecuteChanged(object? sender, EventArgs e) => NotifyCanExecuteChanged();
        private void NotifyCanExecuteChangedPrivate() => PrivateCanExecuteChanged?.Invoke(this, EventArgs.Empty);

        void ICommand.Execute(object parameter)
        {
            if (_command is null) 
                _execute();
            else
                _command.Execute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (_enabled is false) return false;
            if (_command is null) return _canExecute();
            return _command.CanExecute(parameter);
        }
    }


    internal sealed class InactiveButton : IButtonDefinition
    {
        public static InactiveButton Instance { get; } = new();

        private InactiveButton() { }

#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }
#pragma warning disable CS0067

        public Visibility Visibility { get => Visibility.Visible; set {} }
        public bool IsVisible { get => true; set { } }
        public bool IsEnabled => false;
        public string ToolTip => string.Empty;
        public string DisplayText => string.Empty;
        public void NotifyCanExecuteChanged(object? sender, EventArgs e) {}
        public void NotifyCanExecuteChanged() {}
        public bool CanExecute(object parameter) => false;
        public void Execute(object parameter) { }
    }


}
