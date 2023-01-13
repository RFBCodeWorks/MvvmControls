using System;
using System.Threading;
using System.Threading.Tasks;
using RFBCodeWorks.Mvvvm;

namespace RFBCodeWorks.Mvvvm.Specialized
{
    /// <summary>
    /// A button that starts a task when in the default state, and changes state to cancel the task while the task is running.
    /// </summary>
    public sealed class StartCancelTaskButton : Primitives.AbstractTwoStateButton, IButtonDefinition
    {
        /// <inheritdoc cref="StartCancelTaskButton.StartCancelTaskButton(IAsyncRelayCommand)"/>
        /// <inheritdoc cref="AsyncButtonDefinition.AsyncButtonDefinition(Func{CancellationToken, Task})"/>
        public StartCancelTaskButton(Func<CancellationToken, Task> cancelableExecute)
            : this(new AsyncRelayCommand(cancelableExecute)) { }

        /// <inheritdoc cref="StartCancelTaskButton.StartCancelTaskButton(IAsyncRelayCommand)"/>
        /// <inheritdoc cref="AsyncButtonDefinition.AsyncButtonDefinition(Func{CancellationToken, Task}, Func{bool})"/>
        public StartCancelTaskButton(Func<CancellationToken,Task> cancelableExecute, Func<bool> canExecute) 
            : this(new AsyncRelayCommand(cancelableExecute, canExecute)) { }

        /// <inheritdoc cref="StartCancelTaskButton.StartCancelTaskButton(IAsyncRelayCommand)"/>
        /// <inheritdoc cref="AsyncButtonDefinition.AsyncButtonDefinition(AsyncRelayCommand)"/>
        public StartCancelTaskButton(AsyncRelayCommand asyncRelayCommand) 
            :this(command: asyncRelayCommand ?? throw new ArgumentNullException(nameof(asyncRelayCommand))) { }

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

        private IAsyncRelayCommand relayCommand;

        /// <inheritdoc/>
        public override event EventHandler CanExecuteChanged
        {
            add => relayCommand.CanExecuteChanged += value;
            remove => relayCommand.CanExecuteChanged -= value;
        }
        

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected sealed override async void DefaultExecute()
        {
            try
            {
                base.IsDefaultState = false;
                await relayCommand.ExecuteAsync();
            }
            finally
            {
                base.IsDefaultState = true;
            }
        }

        /// <summary>
        /// Invokes the 'Cancel' method fore the <see cref="IAsyncRelayCommand"/>
        /// </summary>
        protected sealed override void AlternateExecute()
        {
            relayCommand.Cancel();
        }

        protected override bool DefaultCanExecute()
        {
            return !relayCommand.IsRunning && relayCommand.CanExecute();
        }

        protected override bool AlternateCanExecute()
        {
            return relayCommand.IsRunning && relayCommand.CanBeCanceled;
        }

        public override void NotifyCanExecuteChanged()
        {
            relayCommand.NotifyCanExecuteChanged();
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
