using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using RFBCodeWorks.Mvvm;

namespace RFBCodeWorks.Mvvm.Specialized
{
    /// <summary>
    /// A button that starts a task when in the default state, and changes state to cancel the task while the task is running.
    /// </summary>
    public sealed class StartCancelTaskButton : Primitives.AbstractTwoStateAsyncButton, IButtonDefinition, ICommand, IAsyncRelayCommand
    {

        /// <inheritdoc cref="StartCancelTaskButton.StartCancelTaskButton(IAsyncRelayCommand)"/>
        /// <inheritdoc cref="AsyncButtonDefinition.AsyncButtonDefinition(Func{CancellationToken, Task}, Func{bool}, Action{Exception}, Action)"/>
        public StartCancelTaskButton(
            Func<CancellationToken,Task> cancelableExecute, 
            Func<bool> canExecute = null, 
            Action<Exception> errorHandler = null, 
            Action cancelReaction = null
            ) : this(new AsyncRelayCommand(cancelableExecute, canExecute, errorHandler, cancelReaction)) { }

        /// <summary>
        /// Create a new ButtonDefinition that will change between two states, where the default state can start a task and the secondary state will cancel the task
        /// </summary>
        /// <inheritdoc cref="AsyncButtonDefinition.AsyncButtonDefinition(IAsyncRelayCommand)"/>
        public StartCancelTaskButton(IAsyncRelayCommand command) : base()
        {
            relayCommand = command ?? throw new ArgumentNullException(nameof(command));
            AlternateDisplayText = "Cancel";
            AlternateToolTip = null;
        }

        private readonly IAsyncRelayCommand relayCommand;

        IEnumerable<Task> IAsyncRelayCommand.RunningTasks => relayCommand.RunningTasks;
        Task CommunityToolkit.Mvvm.Input.IAsyncRelayCommand.ExecutionTask => relayCommand.ExecutionTask;
        bool CommunityToolkit.Mvvm.Input.IAsyncRelayCommand.CanBeCanceled => relayCommand.CanBeCanceled;
        bool CommunityToolkit.Mvvm.Input.IAsyncRelayCommand.IsCancellationRequested => relayCommand.IsCancellationRequested;
        bool CommunityToolkit.Mvvm.Input.IAsyncRelayCommand.IsRunning => relayCommand.IsRunning;

        /// <inheritdoc/>
        public override event EventHandler CanExecuteChanged
        {
            add => relayCommand.CanExecuteChanged += value;
            remove => relayCommand.CanExecuteChanged -= value;
        }
        

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected sealed override async Task DefaultExecute()
        {
            try
            {
                base.IsDefaultState = false;
                await relayCommand.ExecuteAsync(null);
            }
            finally
            {
                base.IsDefaultState = true;
            }
        }

        /// <summary>
        /// Invokes the 'Cancel' method fore the <see cref="IAsyncRelayCommand"/>
        /// </summary>
        protected sealed override Task AlternateExecute()
        {
            relayCommand.Cancel();
            return Task.CompletedTask;
        }

        public override void Cancel() => relayCommand.Cancel();

        protected override bool DefaultCanExecute()
        {
            return !relayCommand.IsRunning && relayCommand.CanExecute(null);
        }

        protected override bool AlternateCanExecute()
        {
            return relayCommand.IsRunning && relayCommand.CanBeCanceled;
        }

        public override void NotifyCanExecuteChanged()
        {
            relayCommand.NotifyCanExecuteChanged();
        }

        Task CommunityToolkit.Mvvm.Input.IAsyncRelayCommand.ExecuteAsync(object parameter) => relayCommand.ExecuteAsync(parameter);
        async void ICommand.Execute(object parameter) => await base.ExecuteAsync();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
