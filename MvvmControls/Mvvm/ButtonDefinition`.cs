﻿using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using RelayCmd = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Class that wraps an <see cref="IRelayCommand"/> to provide the remaining implementation of <see cref="IButtonDefinition"/>
    /// </summary>
    /// <inheritdoc cref="IRelayCommand{T}"/>
    public sealed class ButtonDefinition<T> : Primitives.AbstractButtonDefinition<T>, IButtonDefinition, IRelayCommand<T>, IButtonDefinition<T>
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
        public override bool CanExecute(T parameter) => Command.CanExecute(parameter);

        /// <inheritdoc cref="RelayCommand{T}.Execute(T)"/>
        public override void Execute(T parameter) => Command.Execute(parameter);

        /// <inheritdoc/>
        public override void NotifyCanExecuteChanged() => Command.NotifyCanExecuteChanged();
    }
}
