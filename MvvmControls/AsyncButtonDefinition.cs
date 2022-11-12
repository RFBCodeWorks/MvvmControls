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
    public class AsyncButtonDefinition : ButtonDefinition, IAsyncRelayCommand
    {
        /// <summary>
        /// Create a new ButtonDefinition that wraps a Fire-And-Forget task
        /// </summary>
        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{Task})"/>
        public AsyncButtonDefinition(Func<Task> execute)
            : this(new AsyncRelayCommand(execute)) { }

        /// <summary>
        /// Create a new ButtonDefinition that wraps a Fire-And-Forget task
        /// </summary>
        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{Task}, Func{bool})"/>
        public AsyncButtonDefinition(Func<Task> execute, Func<bool> canExecute)
            :this(new AsyncRelayCommand(execute, canExecute)) { }


        /// <summary>
        /// Create a new ButtonDefinition that wraps a Cancellable Task
        /// </summary>
        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task})"/>
        public AsyncButtonDefinition(Func<CancellationToken, Task> execute)
            : this(new AsyncRelayCommand(execute)) { }

        /// <summary>
        /// Create a new ButtonDefinition that wraps a Cancellable Task
        /// </summary>
        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task}, Func{bool})"/>
        public AsyncButtonDefinition(Func<CancellationToken,Task> cancelableExecute, Func<bool> canExecute)
            : this(new AsyncRelayCommand(cancelableExecute, canExecute)) { }

        /// <summary>
        /// Create a new ButtonDefinition from the specified <paramref name="asyncRelayCommand"/>
        /// </summary>
        /// <param name="asyncRelayCommand">the command to wrap</param>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public AsyncButtonDefinition(AsyncRelayCommand asyncRelayCommand) 
            : this(command: asyncRelayCommand ?? throw new ArgumentNullException(nameof(asyncRelayCommand))) { }

        /// <summary>
        /// Create a new ButtonDefinition from the specified <paramref name="command"/>
        /// </summary>
        /// <param name="command">the command to wrap</param>
        /// <exception cref="ArgumentNullException"/>
        public AsyncButtonDefinition(IAsyncRelayCommand command) : base(command)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <summary>
        /// The IAsyncRelayCommand object through which the <see cref="ICommand"/> interface is implemented
        /// </summary>
        new public IAsyncRelayCommand Command { get; }


        /// <inheritdoc cref="Primitives.AbstractAsyncCommand.ExecuteAsync"/>
        public Task ExecuteAsync() => Command.ExecuteAsync();

        /// <summary>Start the asynchronous task - Fire and Forget</summary>
        public override async void Execute()
        {
            await ExecuteAsync();
        }

        #region < IAsyncRelayCommand Implementation >

        IEnumerable<Task> IAsyncRelayCommand.RunningTasks => Command.RunningTasks;
        Task Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.ExecutionTask => Command.ExecutionTask;
        bool Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.CanBeCanceled => Command.CanBeCanceled;
        bool Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.IsCancellationRequested => Command.IsCancellationRequested;
        bool Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.IsRunning => Command.IsRunning;
        Task Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.ExecuteAsync(object parameter) => Command.ExecuteAsync(parameter);
        void Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.Cancel() => Command.Cancel();

        #endregion
    }
}
