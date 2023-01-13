using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvvm.Primitives
{
    /// <summary>
    /// Abstract base class for AsyncRelayCommands that do not take parameters
    /// </summary>
    public abstract class AbstractAsyncCommand<T> : ObservableObject, IAsyncRelayCommand<T>
    {
        //Adapted from :
        // https://www.youtube.com/watch?v=F7hRmbdE9eY
        // https://dev.azure.com/dcomengineering/FaultTrack/_git/FaultTrack/commit/6dcaa6a5de96507f0cfcacb33802bd732bf2787d?refName=refs/heads/master&path=/Code/Desktop%20Client/FaultTrack.Shell/Windows/AsyncCommand.cs

        #region < Event Arg Singletons >

        /// <summary> Event Args for when IsRunning is changed </summary>
        public static readonly INotifyArgs IsRunningChangedArgs = AbstractAsyncCommand.IsRunningChangedArgs;
        /// <summary> Event Args for when CanBeCanceled is changed </summary>
        public static readonly INotifyArgs CanBeCanceledChangedArgs = AbstractAsyncCommand.CanBeCanceledChangedArgs;
        /// <summary> Event Args for when IsCancellationRequested is changed </summary>
        public static readonly INotifyArgs IsCancellationRequestedChangedArgs = AbstractAsyncCommand.IsCancellationRequestedChangedArgs;
        /// <summary> Event Args for when RunningTasks is changed </summary>
        public static readonly INotifyArgs RunningTasksChangedArgs = AbstractAsyncCommand.RunningTasksChangedArgs;
        /// <summary> Event Args for when RunningTasks is changing </summary>
        public static readonly INotifyArgs ExecutionTaskChangedArgs = AbstractAsyncCommand.ExecutionTaskChangedArgs;

        #endregion

        #region < Static Methods >

        /// <summary>
        /// Static method to be used in place of Func{bool} methods to always return true
        /// </summary>
        /// <returns><see langword="true"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool ReturnTrue(T parameter) => true;

        /// <inheritdoc cref="AbstractCommand{T}.ThrowIfInvalidParameter(object)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static T ThrowIfInvalidParameter(object parameter)
            => AbstractCommand<T>.ThrowIfInvalidParameter(parameter);

        #endregion

        #region < Constructors >

        /// <inheritdoc/>
        protected AbstractAsyncCommand() : this(true) { }

        /// <inheritdoc/>
        protected AbstractAsyncCommand(bool subscribeToCommandManager) : base()
        {
            SubscribeToCommandManager = subscribeToCommandManager;
            runningTasks = new ObservableCollection<Task>();
            runningTasks.CollectionChanged += RunningTasks_CollectionChanged;
        }

        #endregion

        #region < Properties and Events >

        private readonly ObservableCollection<Task> runningTasks;

        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged;

        /// <inheritdoc cref="AbstractCommand.SubscribeToCommandManager"/>
        public bool SubscribeToCommandManager
        {
            get => SubscribeToCommandManagerField;
            set
            {
                if (value)
                    CommandManager.RequerySuggested += CanExecuteChanged;
                else
                    CommandManager.RequerySuggested -= CanExecuteChanged;
                SubscribeToCommandManagerField = value;
            }
        }
        private bool SubscribeToCommandManagerField;

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
        public async Task ExecuteAsync(T parameter)
        {
            Task runningTask = StartTask(parameter);
            runningTasks.Add(runningTask);
            try
            {
                await runningTask;
            }
            finally
            {
                runningTasks.Remove(runningTask);
            }
        }

        /// <summary>
        /// This is the function that will start the task to be consumed by the public <see cref="ExecuteAsync"/> method
        /// </summary>
        /// <returns></returns>
        protected abstract Task StartTask(T parameter);

        /// <inheritdoc/>
        public abstract bool CanExecute(T parameter);

        /// <inheritdoc/>
        public abstract void Cancel();

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NotifyCanExecuteChanged(object sender, EventArgs e) => NotifyCanExecuteChanged();

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

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
            OnPropertyChanged(AllPropertiesChangedArgs);
            NotifyCanExecuteChanged();
        }

        #region < Interface Implementations >

        bool ICommand.CanExecute(object parameter)
        {
            // Special case a null value for a value type argument type.
            // This ensures that no exceptions are thrown during initialization.
            //https://github.com/CommunityToolkit/dotnet/blob/e8969781afe537ea41a964a15b4ccfee32e095df/src/CommunityToolkit.Mvvm/Input/RelayCommand%7BT%7D.cs#L87
            if (parameter is null && default(T) is not null)
            {
                return false;
            }
            return CanExecute(ThrowIfInvalidParameter(parameter));
        }

        async void ICommand.Execute(object parameter) => await ExecuteAsync(ThrowIfInvalidParameter(parameter));
        async void Microsoft.Toolkit.Mvvm.Input.IRelayCommand<T>.Execute(T parameter) => await ExecuteAsync(parameter);
        Task Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand<T>.ExecuteAsync(T parameter) => ExecuteAsync(parameter);
        Task Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.ExecuteAsync(object parameter) => ExecuteAsync(ThrowIfInvalidParameter(parameter));
        
        #endregion
    }
}
