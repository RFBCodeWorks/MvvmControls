using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolkit = CommunityToolkit.Mvvm.Input;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Class that wraps an <see cref="Input.IAsyncRelayCommand"/> to provide the remaining implementation of <see cref="IButtonDefinition"/>
    /// </summary>
    public sealed class AsyncButtonDefinition<T> : Primitives.AbstractAsyncButtonDefinition<T>, Input.IAsyncRelayCommand<T>, ICommand
    {
        /// <summary>
        /// Create a new ButtonDefinition that wraps a Fire-And-Forget task
        /// </summary>
        /// <inheritdoc cref="Input.AsyncRelayCommand.AsyncRelayCommand(Func{Task}, Func{bool}, Action{Exception})"/>
        public AsyncButtonDefinition(
            Func<T, Task> execute,
            Func<T, bool> canExecute = null,
            Action<Exception> errorHandler = null
            ) : this(new Input.AsyncRelayCommand<T>(execute, canExecute, errorHandler)) { }

        /// <summary>
        /// Create a new ButtonDefinition that wraps a Cancellable Task
        /// </summary>
        /// <inheritdoc cref="Input.AsyncRelayCommand{T}.AsyncRelayCommand(Func{T, CancellationToken, Task}, Func{T, bool}, Action{Exception}, Action{T})"/>
        public AsyncButtonDefinition(
            Func<T, CancellationToken,Task> cancelableExecute, 
            Func<T,bool> canExecute = null, 
            Action<Exception> errorHandler = null,
            Action<T> cancelReaction= null
            ) : this(new Input.AsyncRelayCommand<T>(cancelableExecute, canExecute, errorHandler, cancelReaction)) { }

        /// <summary>
        /// Create a new ButtonDefinition from the specified <paramref name="command"/>
        /// </summary>
        /// <param name="command">the command to wrap</param>
        /// <exception cref="ArgumentNullException"/>
        public AsyncButtonDefinition(Toolkit.IAsyncRelayCommand<T> command) : base()
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <summary>
        /// The IAsyncRelayCommand object through which the <see cref="ICommand"/> interface is implemented
        /// </summary>
        public Toolkit.IAsyncRelayCommand<T> Command { get; }

        /// <inheritdoc/>
        public bool IsRunning => Command.IsRunning;

        /// <inheritdoc/>
        public bool CanBeCanceled => Command.CanBeCanceled;

        /// <inheritdoc/>
        public bool IsCancellationRequested => Command.IsCancellationRequested;

        /// <inheritdoc/>
        public override Task ExecuteAsync(T parameter) => Command.ExecuteAsync(parameter);

        /// <inheritdoc/>
        public override bool CanExecute(T parameter) => Command.CanExecute(parameter);

        /// <inheritdoc/>
        public override void Cancel()
        {
            if (Command.CanBeCanceled && !Command.IsCancellationRequested)
                Command.Cancel();
        }

        #region < IAsyncRelayCommand Implementation >

        /// <inheritdoc/>
        public override event EventHandler CanExecuteChanged
        {
            add => Command.CanExecuteChanged += value;
            remove => Command.CanExecuteChanged -= value;
        }

        /// <inheritdoc/>
        public override void NotifyCanExecuteChanged() => Command?.NotifyCanExecuteChanged();

        IEnumerable<Task> Input.IAsyncRelayCommand.RunningTasks
        {
            get
            {
                if (this.Command is Input.AsyncRelayCommand cmd)
                {
                    foreach (var task in cmd.RunningTasks)
                        yield return task;
                }
                else
                    yield return Command?.ExecutionTask;
            }
        }
        Task CommunityToolkit.Mvvm.Input.IAsyncRelayCommand.ExecutionTask => Command.ExecutionTask;
        Task CommunityToolkit.Mvvm.Input.IAsyncRelayCommand.ExecuteAsync(object parameter) => ExecuteAsync(Primitives.AbstractCommand<T>.ThrowIfInvalidParameter(parameter));
        void CommunityToolkit.Mvvm.Input.IRelayCommand<T>.Execute(T parameter) => Command.Execute(parameter);
        void ICommand.Execute(object parameter) => Command.Execute(parameter);

        #endregion
    }
}
