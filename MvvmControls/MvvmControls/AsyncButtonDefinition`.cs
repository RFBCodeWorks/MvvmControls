using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Class that wraps an <see cref="IAsyncRelayCommand"/> to provide the remaining implementation of <see cref="IButtonDefinition"/>
    /// </summary>
    public class AsyncButtonDefinition<T> : ButtonDefinition<T>, IAsyncRelayCommand<T>
    {
        /// <summary>
        /// Create a new ButtonDefinition that wraps a Fire-And-Forget task
        /// </summary>
        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{Task})"/>
        public AsyncButtonDefinition(Func<T,Task> execute)
            : this(new AsyncRelayCommand<T>(execute)) { }

        /// <summary>
        /// Create a new ButtonDefinition that wraps a Fire-And-Forget task
        /// </summary>
        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{Task}, Func{bool})"/>
        public AsyncButtonDefinition(Func<T,Task> execute, Func<T,bool> canExecute)
            :this(new AsyncRelayCommand<T>(execute, canExecute)) { }


        /// <summary>
        /// Create a new ButtonDefinition that wraps a Cancellable Task
        /// </summary>
        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task})"/>
        public AsyncButtonDefinition(Func<T,CancellationToken, Task> execute)
            : this(new AsyncRelayCommand<T>(execute)) { }

        /// <summary>
        /// Create a new ButtonDefinition that wraps a Cancellable Task
        /// </summary>
        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task}, Func{bool})"/>
        public AsyncButtonDefinition(Func<T, CancellationToken,Task> cancelableExecute, Func<T,bool> canExecute)
            : this(new AsyncRelayCommand<T>(cancelableExecute, canExecute)) { }

        /// <summary>
        /// Create a new ButtonDefinition from the specified <paramref name="asyncRelayCommand"/>
        /// </summary>
        /// <param name="asyncRelayCommand">the command to wrap</param>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public AsyncButtonDefinition(AsyncRelayCommand<T> asyncRelayCommand) 
            : this(command: asyncRelayCommand ?? throw new ArgumentNullException(nameof(asyncRelayCommand))) { }

        /// <summary>
        /// Create a new ButtonDefinition from the specified <paramref name="command"/>
        /// </summary>
        /// <param name="command">the command to wrap</param>
        /// <exception cref="ArgumentNullException"/>
        public AsyncButtonDefinition(IAsyncRelayCommand<T> command) : base(command)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <summary>
        /// The IAsyncRelayCommand object through which the <see cref="ICommand"/> interface is implemented
        /// </summary>
        new public IAsyncRelayCommand<T> Command { get; }

        /// <inheritdoc/>
        public bool IsRunning => Command.IsRunning;

        /// <inheritdoc/>
        public bool CanBeCanceled => Command.CanBeCanceled;

        /// <inheritdoc/>
        public bool IsCancellationRequested => Command.IsCancellationRequested;

        /// <inheritdoc cref="Primitives.AbstractAsyncCommand{T}.ExecuteAsync"/>
        public Task ExecuteAsync(T parameter) => Command.ExecuteAsync(parameter);

        /// <summary>Start the asynchronous task - Fire and Forget</summary>
        public sealed override void Execute(T parameter)
        {
            _ = ExecuteAsync(parameter);
        }

        /// <inheritdoc/>
        public void Cancel()
        {
            if (Command.CanBeCanceled && !Command.IsCancellationRequested)
                Command.Cancel();
        }

        #region < IAsyncRelayCommand Implementation >

        IEnumerable<Task> IAsyncRelayCommand.RunningTasks => Command.RunningTasks;
        Task Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.ExecutionTask => Command.ExecutionTask;
        Task Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.ExecuteAsync(object parameter) => ExecuteAsync((T)parameter);

        #endregion
    }
}
