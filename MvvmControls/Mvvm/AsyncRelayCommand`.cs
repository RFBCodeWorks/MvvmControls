﻿using System;
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
    public sealed class AsyncRelayCommand<T> : Primitives.AbstractAsyncCommand<T>
    {

        /// <summary>
        /// Create a new AsyncRelayCommand that will execute a Fire-And-Forget task
        /// </summary>
        /// <inheritdoc cref="Primitives.AbstractAsyncCommand{T}.AbstractAsyncCommand(bool, Action{Exception})"/>
        /// <inheritdoc cref="CommunityToolkit.Mvvm.Input.AsyncRelayCommand.AsyncRelayCommand(Func{Task}, Func{bool})"/>
        /// <exception cref="ArgumentNullException"/><exception cref="ArgumentNullException"/>
        public AsyncRelayCommand(
            Func<T, Task> execute, 
            Func<T,bool> canExecute = null, 
            Action<Exception> errorHandler = null
            ) : base(true, errorHandler)
        {
            ExecuteAction = execute ?? throw new ArgumentNullException(nameof(execute));
            CanExecuteFunction = canExecute ?? ReturnTrue;
        }

        /// <inheritdoc cref="AsyncRelayCommand{T}.AsyncRelayCommand(Func{T, Task}, Func{T, bool}, Action{Exception})"/>
        public AsyncRelayCommand(Func<T, Task> execute, Action<Exception> errorHandler) : this(execute, null, errorHandler) { }

        /// <inheritdoc cref="AsyncRelayCommand{T}.AsyncRelayCommand(Func{T, Task}, Func{T, bool}, Action{Exception})"/>
        public AsyncRelayCommand(Func<T, Task> execute) : this(execute, null, null) { }

        /// <summary>
        /// Create a new AsyncRelayCommand that will execute a cancellable task
        /// </summary>
        /// <param name="cancelReaction"><inheritdoc cref="CancelReaction" path="*"/></param>
        /// <inheritdoc cref="CommunityToolkit.Mvvm.Input.AsyncRelayCommand.AsyncRelayCommand(Func{CancellationToken, Task}, Func{bool})"/>
        /// <inheritdoc cref="Primitives.AbstractAsyncCommand{T}.AbstractAsyncCommand(bool, Action{Exception})"/>
        /// <param name="errorHandler"/><param name="canExecute"/><param name="cancelableExecute"/>
        /// <exception cref="ArgumentNullException"/>
        public AsyncRelayCommand(
            Func<T, CancellationToken, Task> cancelableExecute, 
            Func<T,bool> canExecute = null, 
            Action<Exception> errorHandler = null, 
            Action<T> cancelReaction = null
            )  : base(true, errorHandler)
        {
            CanExecuteFunction = canExecute ?? ReturnTrue;
            CancellableExecuteAction = cancelableExecute ?? throw new ArgumentNullException(nameof(cancelableExecute));
            CancellationTokens = new Dictionary<Task, CancellationTokenSource>();
            CancelReaction = cancelReaction;
            SwallowCancellations = cancelReaction != null;
        }

        /// <inheritdoc cref="AsyncRelayCommand{T}.AsyncRelayCommand(Func{T, CancellationToken, Task}, Func{T, bool}, Action{Exception}, Action{T})"/>
        public AsyncRelayCommand(Func<T, CancellationToken, Task> cancelableExecute, Action<Exception> errorHandler, Action<T> cancelReaction = null ) 
            : this (cancelableExecute, null, errorHandler, cancelReaction) { }

        /// <inheritdoc cref="AsyncRelayCommand{T}.AsyncRelayCommand(Func{T, CancellationToken, Task}, Func{T, bool}, Action{Exception}, Action{T})"/>
        public AsyncRelayCommand(Func<T, CancellationToken, Task> cancelableExecute, Action<T> cancelReaction)
            : this(cancelableExecute, null, null, cancelReaction) { }

        /// <inheritdoc cref="AsyncRelayCommand{T}.AsyncRelayCommand(Func{T, CancellationToken, Task}, Func{T, bool}, Action{Exception}, Action{T})"/>
        public AsyncRelayCommand(Func<T, CancellationToken, Task> cancelableExecute)
            : this(cancelableExecute, null, null, null) { }

        private readonly Func<T,bool> CanExecuteFunction;
        private readonly Func<T, Task> ExecuteAction;
        private readonly Func<T,CancellationToken, Task> CancellableExecuteAction;
        private readonly Dictionary<Task, CancellationTokenSource> CancellationTokens;
        /// <summary>An action that will take place if the task throws an <see cref="OperationCanceledException"/>. The input parameter will be passed into this action when invoked.</summary>
        private readonly Action<T> CancelReaction;

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
        public sealed override bool CanExecute(T parameter)
        {
            if (IsRunning)
                return AllowConcurrentExecution && CanExecuteFunction(parameter);
            else
                return CanExecuteFunction(parameter);
        }

        /// <remarks>This method cannot be overridden</remarks>
        /// <inheritdoc/>
        protected async sealed override Task StartTask(T parameter)
        {
            if (ExecuteAction != null)
            {
                try
                {
                    await ExecuteAction(parameter);
                }
                catch (OperationCanceledException) when (SwallowCancellations) { }
                catch (Exception e) when (ErrorHandler != null)
                {
                    ErrorHandler(e);
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
                Task task = CancellableExecuteAction(parameter,source.Token);
                lock (CancellationTokens)
                    CancellationTokens.Add(task, source);

                try
                {
                    await task;
                }
                catch (OperationCanceledException) when (SwallowCancellations | CancelReaction != null)
                {
                    CancelReaction?.Invoke(parameter);
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
