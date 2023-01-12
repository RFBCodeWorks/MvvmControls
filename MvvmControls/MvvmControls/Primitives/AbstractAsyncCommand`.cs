using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.MvvmControls.Primitives
{
    /// <summary>
    /// Abstract base class for AsyncRelayCommands that do not take parameters
    /// </summary>
    public abstract class AbstractAsyncCommand<T> : AbstractCommandBase, IAsyncRelayCommand<T>
    {
        //Adapted from :
        // https://www.youtube.com/watch?v=F7hRmbdE9eY
        // https://dev.azure.com/dcomengineering/FaultTrack/_git/FaultTrack/commit/6dcaa6a5de96507f0cfcacb33802bd732bf2787d?refName=refs/heads/master&path=/Code/Desktop%20Client/FaultTrack.Shell/Windows/AsyncCommand.cs

        /// <inheritdoc/>
        protected AbstractAsyncCommand() : this(true) { }

        /// <inheritdoc/>
        protected AbstractAsyncCommand(bool subscribeToCommandManager) : base(subscribeToCommandManager)
        {
            runningTasks = new ObservableCollection<Task>();
            runningTasks.CollectionChanged += RunningTasks_CollectionChanged;
        }

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

        /// <summary> Check if any tasks are running, then return the inverse </summary>
        /// <returns> The inverse of <see cref="IsRunning"/> </returns>
        protected bool NoneRunning() => !IsRunning;

        async void ICommand.Execute(object parameter) => await ExecuteAsync(ThrowExceptionIfInvalidType<T>(parameter));
        async void Microsoft.Toolkit.Mvvm.Input.IRelayCommand<T>.Execute(T parameter) => await ExecuteAsync(parameter);
        Task Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand<T>.ExecuteAsync(T parameter) => ExecuteAsync(parameter);
        Task Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.ExecuteAsync(object parameter) => ExecuteAsync(ThrowExceptionIfInvalidType<T>(parameter));
        bool ICommand.CanExecute(object parameter) => CanExecute(ThrowExceptionIfInvalidType<T>(parameter));

        private void RunningTasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ExecutionTask = runningTasks.Last();
            }
            OnPropertyChanged(AllPropertiesChangedArgs);
            OnPropertyChanged(AbstractAsyncCommand.ExecutionTaskChangedArgs);
            NotifyCanExecuteChanged();
        }

        //Task IAsyncRelayCommand.ExecuteAsync() => throw new NotImplementedException("RFBCodeWorks.MvvmControls.IAsyncRelayCommand.ExecuteAsync() is not supported unless explicitly implemented by the dervied class");
        //void IRelayCommand.Execute() => throw new NotImplementedException("RFBCodeWorks.MvvmControls.IRelayCommand.Execute() is not supported unless explicitly implemented by the dervied class");
        //bool IRelayCommand.CanExecute() => ((ICommand)this).CanExecute(null);

    }
}
