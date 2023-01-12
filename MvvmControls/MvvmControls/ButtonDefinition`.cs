using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using RelayCmd = Microsoft.Toolkit.Mvvm.Input.RelayCommand;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Class that wraps an <see cref="IRelayCommand"/> to provide the remaining implementation of <see cref="IButtonDefinition"/>
    /// </summary>
    public class ButtonDefinition<T> : Primitives.AbstractButtonDefinition, IButtonDefinition, IRelayCommand<T>
    {
        /// <inheritdoc cref="ButtonDefinition.ButtonDefinition(Action, Func{bool})"/>
        public ButtonDefinition(Action<T> execute) : this(execute, ReturnTrue) { }

        /// <summary>
        /// Create a new ButtonDefinition using the specified <paramref name="execute"/> action
        /// </summary>
        /// <inheritdoc cref="RelayCmd.RelayCommand(Action, Func{bool})"/>
        public ButtonDefinition(Action<T> execute, Func<T,bool> canExecute)
        {
            Command = new RelayCommand<T>(execute, canExecute);
        }

        /// <summary>
        /// Create a new ButtonDefinition from the specified IRelayCommand
        /// </summary>
        /// <param name="command">the command</param>
        public ButtonDefinition(IRelayCommand<T> command)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <summary>
        /// The IRelayCommand object through which the <see cref="ICommand"/> interface is implemented
        /// </summary>
        public IRelayCommand<T> Command { get; }

        /// <inheritdoc/>
        public sealed override event EventHandler CanExecuteChanged
        {
            add => Command.CanExecuteChanged += value;
            remove => Command.CanExecuteChanged -= value;
        }

        /// <inheritdoc cref="RelayCommand{T}.CanExecute(T)"/>
        public bool CanExecute(T parameter) => Command.CanExecute(parameter);

        /// <inheritdoc cref="RelayCommand{T}.Execute(T)"/>
        public virtual void Execute(T parameter) => Command.Execute(parameter);

        /// <inheritdoc/>
        protected override bool CanExecute(object parameter) => Command.CanExecute(parameter);
        /// <inheritdoc/>
        protected override void Execute(object parameter) => Command.Execute(parameter);


        /// <inheritdoc/>
        public override void NotifyCanExecuteChanged() => Command.NotifyCanExecuteChanged();
    }
}
