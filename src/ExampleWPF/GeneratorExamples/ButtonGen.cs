using System;
using System.Threading.Tasks;
using System.Threading;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen
{
    /// <summary>
    /// Class to run the Source Generator, which will then be unit tested for integration test.
    /// </summary>
    public partial class ButtonGen
    {
        private const int taskDelay = 350;
        public const int CancellationDelay = taskDelay/2;

        public const string DisplayText = "PERFORM TEST";
        public const string DisplayText_WithCanExecute = "PERFORM TEST";

        public ButtonGen(Func<bool> canExecute) 
        {
            _canExecute = canExecute ?? throw new ArgumentNullException();
        }

        
        private readonly Func<bool> _canExecute;

        public bool _CanExecuteWasEvaluated { get; private set; }
        public bool _WasExecuted { get; private set; }
        public bool _WasExecutedAsync { get; private set; }

        public void Reset()
        {
            _WasExecuted = false;
            _WasExecutedAsync = false;
            _CanExecuteWasEvaluated = false;
        }

        /// <summary>
        /// Set <see cref="_WasExecuted"/> to <see langword="true"/>
        /// </summary>
        [Button(DisplayText = ButtonGen.DisplayText, Tooltip = "RUN THE METHOD")]
        private void Run_() => _WasExecuted = true;

        [Button(DisplayText = ButtonGen.DisplayText, CanExecute =nameof(CanExecute))]
        private void Run_CanExecute() => _WasExecuted = true;

        [Button(DisplayText = ButtonGen.DisplayText)]
        private async Task Run_Asynchronous()
        {
            await Task.Delay(50);
            _WasExecutedAsync = true;
        }

        [Button(DisplayText = ButtonGen.DisplayText, CanExecute = nameof(CanExecute))]
        private async Task Run_Asynchronous_CanExecute()
        {
            await Task.Delay(50);
            _WasExecutedAsync = true;
        }

        [Button(DisplayText = ButtonGen.DisplayText, CanExecute = nameof(CanExecute), IncludeCancelCommand = true)]
        private async Task Run_Asynchronous_CanExecute_Cancellable(CancellationToken token)
        {
            await Task.Delay(taskDelay, token);
            _WasExecutedAsync = true;
        }        

        [Button(DisplayText = ButtonGen.DisplayText, IncludeCancelCommand =true)]
        private async Task Run_Asynchronous_Cancellable(CancellationToken token)
        {
            await Task.Delay(taskDelay, token);
            _WasExecutedAsync = true;
        }

        [Button(DisplayText = ButtonGen.DisplayText, IncludeCancelCommand = false, AllowConcurrentExecutions = true)]
        private async Task Run_ConcurrentAsync(CancellationToken token)
        {
            await Task.Delay(taskDelay*2, token);
            _WasExecutedAsync = true;
        }

        private bool CanExecute()
        {
            _CanExecuteWasEvaluated = true;
            return _canExecute();
        }
    }
}
