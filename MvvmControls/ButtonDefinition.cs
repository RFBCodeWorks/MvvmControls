using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using RelayCmd = Microsoft.Toolkit.Mvvm.Input.RelayCommand;

namespace RFBCodeWorks.MvvmControls
{

    /// <summary>
    /// Class that wraps an <see cref="IRelayCommand"/> to provide the remaining implementation of <see cref="IButtonDefinition"/>
    /// </summary>
    public class ButtonDefinition : AbstractButtonDefinition, IButtonDefinition
    {
        /// <inheritdoc cref="ButtonDefinition.ButtonDefinition(Action, Func{bool})"/>
        public ButtonDefinition(Action execute) : this(execute, ReturnTrue) { }

        /// <summary>
        /// Create a new ButtonDefinition using the specified <paramref name="execute"/> action
        /// </summary>
        /// <inheritdoc cref="RelayCmd.RelayCommand(Action, Func{bool})"/>
        /// <exception cref="ArgumentNullException"/>
        public ButtonDefinition(Action execute, Func<bool> canExecute)
        {
            Command = new RelayCommand(execute, canExecute);
        }

        /// <summary>
        /// Create a new ButtonDefinition from the specified IRelayCommand
        /// </summary>
        /// <param name="command">the command</param>
        /// <exception cref="ArgumentNullException"/>
        public ButtonDefinition(IRelayCommand command)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <summary>
        /// The IRelayCommand object through which the <see cref="ICommand"/> interface is implemented
        /// </summary>
        public IRelayCommand Command { get; }

        /// <inheritdoc cref="RelayCommand.CanExecute()"/>
        public bool CanExecute() => Command.CanExecute();

        /// <inheritdoc cref="RelayCommand.Execute()"/>
        public virtual void Execute() => Command.Execute();

        /// <inheritdoc/>
        public sealed override event EventHandler CanExecuteChanged
        {
            add => Command.CanExecuteChanged += value;
            remove => Command.CanExecuteChanged -= value;
        }
        /// <inheritdoc/>
        public override void NotifyCanExecuteChanged() => Command.NotifyCanExecuteChanged();
        /// <inheritdoc/>
        protected override bool CanExecute(object parameter) => Command.CanExecute(parameter);
        /// <inheritdoc/>
        protected override void Execute(object parameter) => Command.Execute(parameter);
    }

}
