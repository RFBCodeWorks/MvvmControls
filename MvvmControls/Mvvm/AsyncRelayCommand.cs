using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// A RelayCommand that supports Async Functionality, can react to the task generating an error, and can react to a task being cancelled
    /// </summary>
    public sealed class AsyncRelayCommand : Primitives.AbstractAsyncCommand
    {
        #region < Fire-And-Forget Constructors >

        /// <summary>
        /// Create a new AsyncRelayCommand that will execute a cancellable task
        /// </summary>
        /// <inheritdoc cref="Primitives.AbstractCommand.AbstractCommand(bool)"/>
        /// <inheritdoc cref="CommunityToolkit.Mvvm.Input.AsyncRelayCommand.AsyncRelayCommand(Func{Task}, Func{bool})"/>
        /// <exception cref="ArgumentNullException"/>
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute) : base(true)
        {
            ExecuteAction = execute ?? throw new ArgumentNullException(nameof(execute));
            CanExecuteFunction = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{Task}, Func{bool})"/>
        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task}, Func{bool}, Action{Exception}, Action)"/>
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute, Action<Exception> errorHandler) 
            : this(execute, canExecute)
        {
            ErrorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        }

        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{Task}, Func{bool}, Action{Exception})"/>
        public AsyncRelayCommand(Func<Task> execute, Action<Exception> errorHandler) 
            : this(execute, ReturnTrue, errorHandler) { }

        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{Task}, Func{bool}, Action{Exception})"/>
        public AsyncRelayCommand(Func<Task> execute)
            : this(execute, ReturnTrue) { }

        #endregion

        #region < Cancellable Constructors >

        /// <summary>
        /// Create a new AsyncRelayCommand that will execute a cancellable task
        /// </summary>
        /// <inheritdoc cref="Primitives.AbstractCommand.AbstractCommand(bool)"/>
        /// <inheritdoc cref="CommunityToolkit.Mvvm.Input.AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task}, Func{bool})"/>
        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task})"/>
        /// <exception cref="ArgumentNullException"/>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute) 
            : base(true)
        {
            CanExecuteFunction = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            CancellableExecuteAction = cancelableExecute ?? throw new ArgumentNullException(nameof(cancelableExecute));
            CancellationTokens = new Dictionary<Task, CancellationTokenSource>();
        }

        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task}, Func{bool}, Action{Exception}, Action)"/>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute, Action<Exception> errorHandler) 
            : this(cancelableExecute, canExecute)
        {
            ErrorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        }

        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task}, Action{Exception})"/>
        /// <param name="cancelableExecute"/><param name="canExecute"/>
        /// <param name="errorHandler"><inheritdoc cref="ErrorHandler" path="*"/></param>
        /// <param name="cancelReaction"><inheritdoc cref="CancelReaction" path="*"/></param>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute, Action<Exception> errorHandler, Action cancelReaction) 
            : this(cancelableExecute, canExecute, errorHandler)
        {
            CancelReaction = cancelReaction ?? throw new ArgumentNullException(nameof(cancelReaction));
            //SwallowCancellations = true;
        }

        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task}, Func{bool}, Action{Exception}, Action)"/>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute, Action cancelReaction) : this(cancelableExecute, canExecute)
        {
            CancelReaction = cancelReaction ?? throw new ArgumentNullException(nameof(cancelReaction));
            //SwallowCancellations = true;
        }

        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task}, Func{bool}, Action{Exception}, Action)"/>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Action cancelReaction)
            : this(cancelableExecute, ReturnTrue, cancelReaction) { }

        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task},Func{bool})"/>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute) 
            : this(cancelableExecute, ReturnTrue) { }

        /// <inheritdoc cref="AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task}, Func{bool}, Action{Exception}, Action)"/>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Action<Exception> errorHandler) 
            : this(cancelableExecute, ReturnTrue)
        {
            ErrorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        }

        #endregion

        private readonly Func<bool> CanExecuteFunction;
        private readonly Func<Task> ExecuteAction;
        private readonly Func<CancellationToken, Task> CancellableExecuteAction;
        private readonly Dictionary<Task, CancellationTokenSource> CancellationTokens;
        /// <summary>An action that will take place if the task throws an exception</summary>
        private readonly Action<Exception> ErrorHandler;
        /// <summary>An action that will take place if the task throws an <see cref="OperationCanceledException"/></summary>
        private readonly Action CancelReaction;

        /// <summary>
        /// Set <see langword="true"/> to allow this command to start multiple tasks and run them concurrently. <br/>
        /// Default value is <see langword="false"/>
        /// </summary>
        public bool AllowConcurrentExecution { get; set; }

        /// <summary>
        /// If an ErrorHandler was provided in the constructor, this setting decides whether any caught exceptions are passed up the stack.
        /// <br/> If <see langword="false"/>, exceptions will be passed to the caller. If no ErrorHandler was specified, this will always be the case.
        /// <br/> If <see langword="true"/>, exceptions will be passed to the ErrorHandler only. This is default functionality if an ErrorHandler is specified to the constructor.
        /// </summary>
        public bool SwallowHandledExceptions { get => ErrorHandler is not null && SwallowExceptionsField; set => SwallowExceptionsField = value; }
        private bool SwallowExceptionsField = true;

        /// <summary>
        /// Determine if <see cref="OperationCanceledException"/> exceptions are passed up the stack. These are typically generated when the task is cancelled.
        /// </summary>
        public bool SwallowCancellations { get; set; }

        /// <inheritdoc/>
        public sealed override bool CanBeCanceled => CancellableExecuteAction != null && IsRunning;

        /// <inheritdoc/>
        public override bool IsCancellationRequested => IsCancellationRequestedField;
        private bool IsCancellationRequestedField;

        /// <summary>
        /// Cancel all running tasks
        /// </summary>
        /// <inheritdoc/>
        public sealed override void Cancel()
        {
            if (!CanBeCanceled | IsCancellationRequested) return;
            IsCancellationRequestedField = true; 
            OnPropertyChanged(IsCancellationRequestedChangedArgs);

            foreach (var source in CancellationTokens.Values)
            {
                if (!source.IsCancellationRequested)
                    source.Cancel();
            }
        }

        /// <inheritdoc/>
        public sealed override bool CanExecute()
        {
            if (IsRunning)
                return AllowConcurrentExecution && CanExecuteFunction();
            else
                return CanExecuteFunction();
        }

        /// <remarks>This method cannot be overridden</remarks>
        /// <inheritdoc/>
        protected async sealed override Task StartTask()
        {
            if (ExecuteAction != null)
            {
                try
                {
                    await ExecuteAction();
                }
                catch (OperationCanceledException) when (SwallowCancellations) { }
                catch (Exception e) when (ErrorHandler != null)
                {
                    ErrorHandler(e);
                    if (!SwallowHandledExceptions) throw;
                }
            }
            else if (CancellableExecuteAction != null)
            {
                if (IsCancellationRequested)
                {
                    IsCancellationRequestedField = false;
                    OnPropertyChanged(IsCancellationRequestedChangedArgs);
                }

                //Generate the cancellable task
                var source = new CancellationTokenSource();
                Task task = CancellableExecuteAction(source.Token);
                lock (CancellationTokens)
                    CancellationTokens.Add(task, source);

                try
                {
                    await task;
                }
                catch (OperationCanceledException) when (SwallowCancellations | CancelReaction != null)
                {
                    CancelReaction?.Invoke();
                    if (!SwallowCancellations) throw;
                }
                catch (Exception e) when (ErrorHandler != null && e is not OperationCanceledException)
                {
                    ErrorHandler(e);
                    if (!SwallowHandledExceptions) throw;
                }
                finally
                {
                    lock (CancellationTokens)
                        CancellationTokens.Remove(task);
                }
            }
        }
    }
}
