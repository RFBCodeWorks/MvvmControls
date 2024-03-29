﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm.Primitives
{
    /// <summary>
    /// Abstract base class for AsyncRelayCommands that do not take parameters
    /// </summary>
    public abstract class AbstractAsyncCommand : CommandBase, IAsyncRelayCommand
    {
        //Adapted from :
        // https://www.youtube.com/watch?v=F7hRmbdE9eY
        // https://dev.azure.com/dcomengineering/FaultTrack/_git/FaultTrack/commit/6dcaa6a5de96507f0cfcacb33802bd732bf2787d?refName=refs/heads/master&path=/Code/Desktop%20Client/FaultTrack.Shell/Windows/AsyncCommand.cs


        #region < Event Arg Singletons >

        /// <summary> Event Args for when IsRunning is changed </summary>
        public static readonly INotifyArgs IsRunningChangedArgs = new(nameof(IsRunning));
        /// <summary> Event Args for when CanBeCanceled is changed </summary>
        public static readonly INotifyArgs CanBeCanceledChangedArgs = new(nameof(CanBeCanceled));
        /// <summary> Event Args for when IsCancellationRequested is changed </summary>
        public static readonly INotifyArgs IsCancellationRequestedChangedArgs = new(nameof(IsCancellationRequested));
        /// <summary> Event Args for when RunningTasks is changed </summary>
        public static readonly INotifyArgs RunningTasksChangedArgs = new(nameof(RunningTasks));
        /// <summary> Event Args for when RunningTasks is changing </summary>
        public static readonly INotifyArgs ExecutionTaskChangedArgs = new(nameof(ExecutionTask));

        #endregion

        #region < Constructors >

        /// <inheritdoc cref="AbstractAsyncCommand.AbstractAsyncCommand(bool, Action{Exception})"/>
        protected AbstractAsyncCommand(Action<Exception> errorHandler = null) : this(true, errorHandler) { }

        /// <summary> Initialize the object </summary>
        /// <param name="subscribeToCommandManager"><inheritdoc cref="CommandBase.SubscribeToCommandManager" path="*"/></param>
        /// <param name="errorHandler">The error handler to use if an error occurs when executing via <see cref="ICommand.Execute(object)"/> interface method</param>
        protected AbstractAsyncCommand(bool subscribeToCommandManager, Action<Exception> errorHandler = null) : base(subscribeToCommandManager)
        {
            ErrorHandler = errorHandler;
            runningTasks = new ObservableCollection<Task>();
            runningTasks.CollectionChanged += RunningTasks_CollectionChanged;
        }

        #endregion

        #region < Properties and Events >
        
        /// <summary>An action that will take place if the task throws an exception</summary>
        protected readonly Action<Exception> ErrorHandler;
        private readonly ObservableCollection<Task> runningTasks;

        /// <inheritdoc />
        public IEnumerable<Task> RunningTasks => runningTasks;

        /// <inheritdoc/>
        public Task ExecutionTask { get; private set; }

        /// <inheritdoc/>
        public bool IsRunning => runningTasks.Any();

        /// <inheritdoc/>
        public abstract bool CanBeCanceled { get; }
        
        /// <inheritdoc/>
        public abstract bool IsCancellationRequested { get; }

        #endregion

        /// <summary>
        /// Start the task, and add it to the collection of currently running tasks
        /// </summary>
        /// <inheritdoc/>
        public async Task ExecuteAsync()
        {
            Task runningTask = StartTask();
            lock (runningTask)
                runningTasks.Add(runningTask);
            try
            {
                await runningTask;
            }
            finally
            {
                lock (runningTasks)
                    runningTasks.Remove(runningTask);
            }
        }

        /// <summary>
        /// This is the function that will start the task to be consumed by the public <see cref="ExecuteAsync"/> method
        /// <br/> This task will be added to the collection of <see cref="RunningTasks"/> and 
        /// </summary>
        /// <returns></returns>
        protected abstract Task StartTask();

        /// <inheritdoc/>
        public abstract bool CanExecute();

        /// <inheritdoc/>
        public abstract void Cancel();

        /// <summary> Check if any tasks are running, then return the inverse </summary>
        /// <returns> The inverse of <see cref="IsRunning"/> </returns>
        protected bool NoneRunning() => !IsRunning;

        /// <summary> Notify that a new task has started / completed </summary>
        private void RunningTasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ExecutionTask = runningTasks.Last();
            }
            OnPropertyChanged(INotifyAllProperties);
            NotifyCanExecuteChanged();
        }

        #region < Interface Implementations >

        bool ICommand.CanExecute(object parameter) => CanExecute();
        void ICommand.Execute(object parameter) => ExecuteAsync().FireAndForgetErrorHandling(ErrorHandler);
        Task CommunityToolkit.Mvvm.Input.IAsyncRelayCommand.ExecuteAsync(object parameter) => ExecuteAsync();

        #endregion
    }
}
