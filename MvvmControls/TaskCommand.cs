using System;
using System.Threading;
using System.Threading.Tasks;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Abstract base class for the Async Task Commands
    /// </summary>
    public abstract class AbstractTaskCommand : AbstractButtonDefinition, IButtonDefinition
    {
        /// <summary>
        /// Flag that should be set true when the task is being about to start
        /// </summary>
        protected bool IsTaskStarting;

        /// <summary>
        /// The Task that is running / has run
        /// </summary>
        public Task Task { get; protected set; }

        /// <summary>
        /// A Function that controls when the Task can start, assuming it has not started.
        /// </summary>
        public Func<bool> CanExecuteFunc { get; init; }

        /// <summary>
        /// Action to take when an exception occuring that ends the task prematurely
        /// </summary>
        public Action<Exception> TaskFaultedAction { get; init; }

        /// <summary>
        /// Action to take when the task was cancelled
        /// </summary>
        public Action TaskCancelledAction { get; init; }

        /// <summary>
        /// State of the task
        /// </summary>
        public bool IsRunning
        {
            get => isRunning;
            protected set
            {
                SetProperty(ref isRunning, value, nameof(IsRunning));
                OnPropertyChanged(nameof(DisplayText));
            }
        }
        private bool isRunning;

        /// <summary>
        /// Check if the task is running / starting
        /// </summary>
        /// <returns>
        /// If the task is not running (or task has run to completion), return the result of <see cref="CanExecuteFunc"/> or true.  <br/>
        /// Otherwise return false. 
        /// </returns>
        /// <inheritdoc/>
        public override bool CanExecute(object parameter)
        {
            
            if (!IsTaskStarting && !IsRunning) return CanExecuteFunc?.Invoke() ?? true;
            if (IsTaskStarting | IsRunning) return false;
            if (Task != null &&  Task.Status > TaskStatus.RanToCompletion) // Task Started but ran to completion/faulted
            {
                return CanExecuteFunc?.Invoke() ?? true;
            }
            return false;
        }

        /// <summary>
        /// Set the <see cref="Task"/> and <see cref="IsRunning"/> property, await its completion, the invoke the actions if it was cancelled or faulted.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        protected async Task AwaitTaskCompletion(Task task)
        {
            IsTaskStarting = true;
            IsRunning = true;
            NotifyCanExecuteChanged();
            Task = task;
            bool cancelActionRun = false;
            bool faultedActionRun = false;
            try
            {
                await task;
            }
            catch (TaskCanceledException)
            {
                TaskCancelledAction?.Invoke();
                cancelActionRun = true;
            }
            catch (Exception e)
            {
                if (task.IsCanceled)
                {
                    TaskCancelledAction?.Invoke();
                    cancelActionRun = true;
                }
                else
                {
                    TaskFaultedAction?.Invoke(e);
                    faultedActionRun = true;
                }
            }
            finally
            {
                if (!faultedActionRun && task.IsFaulted && task.Exception != null && TaskFaultedAction != null)
                {
                    TaskFaultedAction(task.Exception);
                }
                else if (!cancelActionRun && task.IsCanceled && TaskCancelledAction != null)
                {
                    TaskCancelledAction();
                }
            }
            IsTaskStarting = false;
            IsRunning = false;
            NotifyCanExecuteChanged();
        }
    }


    /// <summary>
    /// Command that executes takes a method with a return type of Task and monitors its execution. This will prevent the task being started if it is already in progress.
    /// </summary>
    public class TaskCommand : AbstractTaskCommand, IButtonDefinition
    {

        #region < Constructors >

        /// <summary>
        /// Create a TaskCommand object designed to prevent the user pressing the button while the task is running
        /// </summary>
        public TaskCommand() : base() { }

        /// <inheritdoc cref="TaskCommand.TaskCommand()"/>
        /// <param name="TaskGenerator"></param>
        public TaskCommand(Func<Task> TaskGenerator) : this()
        {
            TaskAction = TaskGenerator;
        }

        /// <param name="taskFaultedAction"></param>
        /// <inheritdoc cref="TaskCommand.TaskCommand(Func{Task})"/>
        /// <param name="TaskGenerator"/>
        public TaskCommand(Func<Task> TaskGenerator, Action<Exception> taskFaultedAction)
            : this(TaskGenerator)
        {
            TaskFaultedAction = taskFaultedAction;
        }

        /// <param name="taskCancelledAction"></param>
        /// <inheritdoc cref="TaskCommand.TaskCommand(Func{Task}, Action{Exception})"/>
        /// <param name="taskFaultedAction"/><param name="TaskGenerator"/>
        public TaskCommand(Func<Task> TaskGenerator, Action<Exception> taskFaultedAction, Action taskCancelledAction)
        : this(TaskGenerator, taskFaultedAction)
        {
            TaskCancelledAction = taskCancelledAction;
        }

        #endregion

        #region < Properties >

        /// <summary>
        /// The method that will produce the Task that will be monitored
        /// </summary>
        public Func<Task> TaskAction { get; init; }

        #endregion


        /// <inheritdoc/>
        public override async void Execute(object parameter)
        {
            IsTaskStarting = true;
            NotifyCanExecuteChanged();
            Task = TaskAction();
            await base.AwaitTaskCompletion(Task);
        }
    }


    /// <summary>
    /// Command that executes takes a method with a return type of Task and monitors its execution. <br/>
    /// This also provides a binding for the text to update depending on the task state
    /// </summary>
    public class CancellableTaskCommand : AbstractTaskCommand, IButtonDefinition
    {

        #region < Constructors >

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TaskGenerator"></param>
        public CancellableTaskCommand(Func<TaskPair> TaskGenerator)
        {
            TaskAction = TaskGenerator;
        }

        #endregion


        /// <summary>
        /// The method that will produce the Task that will be monitored
        /// </summary>
        public Func<TaskPair> TaskAction { get; init;  }

        /// <summary>
        /// The Task's assoicated cancellation token
        /// </summary>
        public CancelAction CancelToken { get; private set; }

        
        /// <inheritdoc/>
        public override string DisplayText => IsRunning ? StopTaskText : StartTaskText;

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

        /// <inheritdoc/>
        public override bool CanExecute(object parameter)
        {
            bool canCancel = CancelToken != null && !CancelToken.IsCancellationRequested && Task.Status < TaskStatus.RanToCompletion;
            if (IsRunning) return canCancel;
            return base.CanExecute(parameter);
        }

        /// <inheritdoc/>
        public override async void Execute(object parameter)
        {
            if (IsRunning)
                await StopTask();
            else
                await StartTask();
        }

        private async Task StartTask()
        {
            var TP = TaskAction();
            Task = TP.Task;
            CancelToken = TP.CancelAction;
            await base.AwaitTaskCompletion(Task);
        }

        private async Task StopTask()
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
        /// <param name="source"></param>
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

        /// <summary>
        /// Dispose of the underlying CancellationToken
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)Token).Dispose();
        }
    }

}
