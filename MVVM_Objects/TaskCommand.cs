using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MVVMObjects
{
    /// <summary>
    /// Command that executes takes a method with a return type of Task and monitors its execution. This will prevent the task being started if it is already in progress.
    /// </summary>
    public class TaskCommand : AbstractButtonDefinition, IButtonDefinition
    {
        public TaskCommand(Func<Task> TaskGenerator) { TaskAction = TaskGenerator; }

        public TaskCommand(Func<Task> TaskGenerator, Action<Exception> taskFaultedAction) { TaskAction = TaskGenerator; TaskFaultedAction = taskFaultedAction; }

        public TaskCommand(Func<Task> TaskGenerator, Action<Exception> taskFaultedAction, Action taskCancelledAction) { TaskAction = TaskGenerator; TaskFaultedAction = taskFaultedAction; TaskCancelledAction = taskCancelledAction; }

        /// <summary>
        /// The method that will produce the Task that will be monitored
        /// </summary>
        protected Func<Task> TaskAction { get; }
        public Task Task { get; private set; }
        private bool TaskStarting;

        /// <summary>
        /// Action to take when the task was cancelled
        /// </summary>
        public Action TaskCancelledAction { get; init; }

        /// <summary>
        /// Action to take when an exception occuring that ends the task prematurely
        /// </summary>
        public Action<Exception> TaskFaultedAction { get; init; }

        public override bool CanExecute(object parameter)
        {
            if (Task is null) return true;
            if (!TaskStarting) return true; // Task not started
            if (Task.Status > TaskStatus.RanToCompletion) // Task Started but ran to completion/faulted
            {
                return true;
            }
            return false;
        }

        public override async void Execute(object parameter)
        {
            TaskStarting = true;
            NotifyCanExecuteChanged();
            Task = TaskAction();
            if (Task.Status < TaskStatus.RanToCompletion)
            {
                try
                {
                    await Task;
                }
                catch (TaskCanceledException _)
                {
                    if (TaskCancelledAction != null) TaskCancelledAction();
                }
                catch (Exception e)
                {
                    if (Task.IsCanceled)
                    {
                        if (TaskCancelledAction != null) TaskCancelledAction();
                    }
                    else
                    {
                        if (TaskFaultedAction != null) TaskFaultedAction(e);
                    }
                }
                TaskStarting = false;
                NotifyCanExecuteChanged();
            }
        }
    }


    /// <summary>
    /// Command that executes takes a method with a return type of Task and monitors its execution. <br/>
    /// This also provides a binding for the text to update depending on the task state
    /// </summary>
    public class CancellableTaskCommand : AbstractButtonDefinition, IButtonDefinition
    {
        public CancellableTaskCommand(Func<TaskPair> TaskGenerator)
        {
            TaskAction = TaskGenerator;
        }

        /// <summary>
        /// The method that will produce the Task that will be monitored
        /// </summary>
        protected Func<TaskPair> TaskAction { get; }
        
        /// <summary>
        /// The currently running task
        /// </summary>
        public Task Task { get; private set; }

        /// <summary>
        /// The Task's assoicated cancellation token
        /// </summary>
        public CancelAction CancelToken { get; private set; }

        private bool isRunning;

        /// <summary>
        /// State of the task
        /// </summary>
        public bool IsRunning { get => isRunning;
            private set
            {
                SetProperty(ref isRunning, value, nameof(IsRunning));
                OnPropertyChanged(nameof(ButtonText));
            }
        }

        public string ButtonText => IsRunning ? StopTaskText : StartTaskText;

        /// <summary>
        /// Text to display when the task is not running
        /// </summary>
        public string StartTaskText { get => StartText; set => SetProperty(ref StartText, value, nameof(StartTaskText)); }
            private string StartText = "Start";

        /// <summary>
        /// Text to display while the task is running
        /// </summary>
        public string StopTaskText { get => StopText; set => SetProperty(ref StopText, value, nameof(StopTaskText)); }
        private string StopText = "Cancel";


        public override bool CanExecute(object parameter)
        {
            if (Task is null) return true;
            bool CanCancel = CancelToken != null && !CancelToken.IsCancellationRequested;
            if (CanCancel | Task.Status >= TaskStatus.RanToCompletion) return true;
            return false;
        }

        public override async void Execute(object parameter)
        {
            if (IsRunning)
                StopTask();
            else
                StartTask();
        }

        private async void StartTask()
        {
            var TP = TaskAction();
            Task = TP.Task;
            CancelToken = TP.CancelAction;
            NotifyCanExecuteChanged();
            IsRunning = true;
            await Task;
            IsRunning = false;
            NotifyCanExecuteChanged();
        }

        private async void StopTask()
        {
            if (CancelToken?.IsCancellationRequested ?? false) CancelToken?.Cancel();
            NotifyCanExecuteChanged();
            await Task;
            NotifyCanExecuteChanged();
        }

        
    }

    /// <summary>
    /// Helper Class that represents a Task and its associated Cancellation Token
    /// </summary>
    public class TaskPair
    {
        /// <summary>
        /// Create a TaskPair object for a task that is not cancellable.
        /// </summary>
        public TaskPair(Task task) { Task = task; }

        /// <summary>
        /// Create a TaskPair object for a cancellable task.
        /// </summary>
        public TaskPair(Task task, CancelAction cancelSource) { Task = task; CancelAction= cancelSource; }

        /// <summary>
        /// The Task
        /// </summary>
        public Task Task { get; }

        /// <summary>
        /// The action object that will cancel the associated task
        /// </summary>
        public CancelAction CancelAction { get; }
    }


    /// <summary>
    /// Represents an action to take in order to cancel some task. <br/>
    /// This is effectively a wrapper for a <see cref="CancellationTokenSource"/> for objects that provide a method to cancel instead of exposing the token directly.
    /// </summary>
    public class CancelAction : IDisposable
    {
        /// <summary>
        /// Create a new CancelAction that utilizes a <see cref="CancellationTokenSource"/>
        /// </summary>
        /// <param name="tokenSource"></param>
        public CancelAction(CancellationTokenSource tokenSource)
        {
            if (tokenSource is null) throw new ArgumentNullException(nameof(tokenSource));
            Token = tokenSource;
            CancelRequest = Token.Cancel;
        }

        /// <summary>
        /// Create a new CancelAction from some action
        /// </summary>
        /// <param name="action"></param>
        public CancelAction(Action action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            Token = new CancellationTokenSource();
            CancelRequest = action;
        }

        /// <summary>
        /// Create a new CancelAction from some action
        /// </summary>
        /// <param name="action"></param>
        public CancelAction(Action action, CancellationTokenSource source)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            Token = source ?? new();
            CancelRequest = action;
        }

        /// <summary>
        /// This token will store the state of the cancellation request
        /// </summary>
        private CancellationTokenSource Token { get; }

        /// <summary>
        /// This action will be called by the Cancel method if cancellation is not already requested
        /// </summary>
        private Action CancelRequest { get; }

        /// <inheritdoc cref="CancellationTokenSource.IsCancellationRequested"/>        
        public bool IsCancellationRequested => Token.IsCancellationRequested;

        /// <inheritdoc cref="CancellationTokenSource.Cancel()"/>        
        public void Cancel()
        {
            if (!IsCancellationRequested)
            {
                CancelRequest();
                if (!IsCancellationRequested) Token.Cancel();
            }
        }

        public void Dispose()
        {
            ((IDisposable)Token).Dispose();
        }
    }

}
