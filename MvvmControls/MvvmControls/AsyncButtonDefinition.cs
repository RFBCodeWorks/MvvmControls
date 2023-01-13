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
    public sealed class AsyncButtonDefinition : Primitives.AbstractAsyncButtonDefinition, IAsyncRelayCommand
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
        public AsyncButtonDefinition(IAsyncRelayCommand command) : base()
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <summary>
        /// The IAsyncRelayCommand object through which the <see cref="ICommand"/> interface is implemented
        /// </summary>
        public IAsyncRelayCommand Command { get; }

        /// <inheritdoc cref="Primitives.AbstractAsyncCommand.ExecuteAsync"/>
        public override Task ExecuteAsync() => Command.ExecuteAsync(null);

        /// <inheritdoc/>
        public bool IsRunning => Command.IsRunning;

        /// <inheritdoc/>
        public bool CanBeCanceled => Command.CanBeCanceled;

        /// <inheritdoc/>
        public bool IsCancellationRequested => Command.IsCancellationRequested;

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

        /// <inheritdoc/>
        public override bool CanExecute() => Command?.CanExecute(null) ?? false;

        IEnumerable<Task> IAsyncRelayCommand.RunningTasks => Command.RunningTasks;
        Task Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.ExecutionTask => Command.ExecutionTask;
        Task Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.ExecuteAsync(object parameter) => ExecuteAsync();

        #endregion
    }
}
