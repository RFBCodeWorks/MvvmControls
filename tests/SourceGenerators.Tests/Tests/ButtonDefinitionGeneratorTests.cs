using CommunityToolkit.Mvvm.Input;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.SourceGenerators;
using RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests
{
    [TestClass]
    public class ButtonDefinitionGeneratorTests
    {
        const string CanExecute_NotEvaluated = "CanExecute was NOT evaluated.";
        const string CanExecute_Evaluated = "CanExecute was evaluated when it should not have been.";

        [TestMethod]
        public void RelayCommand_CanExecute_Test()
        {
            // Validates that CanExecute is NOT called before every execute
            var canExecute = true;
            var wasExecuted = false;
            var wasCanExecuteEvaluated = false;

            var relayCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(
                execute: () => wasExecuted = true,
                canExecute: () => { wasCanExecuteEvaluated = true; return canExecute; }
                );

            Assert.IsFalse(wasCanExecuteEvaluated);
            relayCommand.Execute(null);
            Assert.IsTrue(wasExecuted , "\r\nwas not executed.");
            Assert.IsFalse(wasCanExecuteEvaluated, "\r\n CanExecute was evaulated when 'execute' was called.");
        }

        /// <summary>
        /// Test all of the generated details
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void EvaluateButtonsConstructed() 
        {
            bool canExecute = true;
            var vm = new RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen.ButtonGen(() => canExecute);

            Assert.IsFalse(!canExecute && vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            // synchronous
            Assert.IsNotNull(vm.RunButton);
            Assert.IsNotNull(vm.Run_CanExecuteButton);
            
            // async
            Assert.IsNotNull(vm.Run_AsynchronousButton);
            Assert.IsNotNull(vm.Run_Asynchronous_CancellableButton);
            Assert.IsNotNull(vm.Run_Asynchronous_CanExecuteButton);
            Assert.IsNotNull(vm.Run_Asynchronous_CanExecute_CancellableButton);
                                     
            Assert.IsNotNull(vm.Run_Asynchronous_CanExecute_CancellableButtonCancelCommand);
            Assert.IsNotNull(vm.Run_Asynchronous_CancellableButtonCancelCommand);
        }

        /// <summary>
        /// Test Synchronous command
        /// </summary>
        [TestMethod]
        public void Execute_Test()
        {
            bool canExecute = true;
            var vm = new RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen.ButtonGen(() => canExecute);
            Assert.IsFalse(!canExecute && vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");

            var button = vm.RunButton;
            Assert.AreEqual(ButtonGen.DisplayText, button.DisplayText);

            vm.Reset();
            if (button.CanExecute(new())) button.Execute();
            Assert.IsTrue(vm._WasExecuted);
            Assert.IsFalse(vm._CanExecuteWasEvaluated);
            Assert.IsFalse(vm._WasExecutedAsync);

            canExecute = false;

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            if (button.CanExecute(new())) button.Execute();
            Assert.IsTrue(vm._WasExecuted);
            Assert.IsFalse(vm._CanExecuteWasEvaluated);
            Assert.IsFalse(vm._WasExecutedAsync);
        }

        /// <summary>
        /// Test ASynchronous command
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsync_Test()
        {
            bool canExecute = true;
            var vm = new RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen.ButtonGen(() => canExecute);
            Assert.IsFalse(!canExecute && vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");

            var button = vm.Run_AsynchronousButton;
            Assert.AreEqual(ButtonGen.DisplayText, button.DisplayText);

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            if (button.CanExecute()) await button.ExecuteAsync();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsFalse(vm._CanExecuteWasEvaluated, CanExecute_Evaluated);
            Assert.IsTrue(vm._WasExecutedAsync);

            canExecute = false;

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            if (button.CanExecute()) await button.ExecuteAsync();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsFalse(vm._CanExecuteWasEvaluated, CanExecute_Evaluated);
            Assert.IsTrue(vm._WasExecutedAsync);
        }

        /// <summary>
        /// Test Synchronous command that evaluates CanExecute
        /// </summary>
        [TestMethod]
        public void CanExecute_Test()
        {
            bool canExecute = true;
            var vm = new RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen.ButtonGen(() => canExecute);
            Assert.IsFalse(!canExecute && vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");

            var button = vm.Run_CanExecuteButton;
            Assert.AreEqual(ButtonGen.DisplayText, button.DisplayText);

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            if (button.CanExecute(new())) button.Execute();
            Assert.IsTrue(vm._WasExecuted);
            Assert.IsTrue(vm._CanExecuteWasEvaluated, CanExecute_NotEvaluated);
            Assert.IsFalse(vm._WasExecutedAsync);

            canExecute = false;

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            if (button.CanExecute(new())) button.Execute();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsTrue(vm._CanExecuteWasEvaluated, CanExecute_NotEvaluated);
            Assert.IsFalse(vm._WasExecutedAsync);
        }

        /// <summary>
        /// Test ASynchronous command that evaluates CanExecute
        /// </summary>
        [TestMethod]
        public async Task CanExecuteAsync_Test()
        {
            bool canExecute = true;
            var vm = new RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen.ButtonGen(() => canExecute);
            Assert.IsFalse(!canExecute && vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");

            var button = vm.Run_Asynchronous_CanExecuteButton;
            Assert.AreEqual(ButtonGen.DisplayText, button.DisplayText);

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            if (button.CanExecute()) await button.ExecuteAsync();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsTrue(vm._CanExecuteWasEvaluated, CanExecute_NotEvaluated);
            Assert.IsTrue(vm._WasExecutedAsync);

            canExecute = false;

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            if (button.CanExecute()) await button.ExecuteAsync();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsTrue(vm._CanExecuteWasEvaluated, CanExecute_NotEvaluated);
            Assert.IsFalse(vm._WasExecutedAsync);
        }

        /// <summary>
        /// Test ASynchronous command that evaluates CanExecute with cancellation
        /// </summary>
        [TestMethod]
        public async Task CanExecute_Cancellable_Test()
        {
            bool canExecute = true;
            var vm = new RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen.ButtonGen(() => canExecute);
            Assert.IsFalse(!canExecute && vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");

            var button =       vm.Run_Asynchronous_CanExecute_CancellableButton;
            var cancelButton = vm.Run_Asynchronous_CanExecute_CancellableButtonCancelCommand;
            Assert.AreEqual(ButtonGen.DisplayText, button.DisplayText);

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            if (button.CanExecute()) await button.ExecuteAsync();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsTrue(vm._CanExecuteWasEvaluated, CanExecute_NotEvaluated);
            Assert.IsTrue(vm._WasExecutedAsync);

            canExecute = false;

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            if (button.CanExecute()) await button.ExecuteAsync();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsTrue(vm._CanExecuteWasEvaluated, CanExecute_NotEvaluated);
            Assert.IsFalse(vm._WasExecutedAsync);

            // test cancellation
            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            var tA = new CancellationTokenSource();
            tA.Token.Register(() => button.Cancel()); // register a cancellation
            tA.CancelAfter(Gen.ButtonGen.CancellationDelay);
            await Assert.ThrowsAsync<OperationCanceledException>(button.ExecuteAsync);
            tA.Dispose();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsFalse(vm._WasExecutedAsync);

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            tA = new CancellationTokenSource();
            tA.Token.Register(() => cancelButton.Execute(null)); // register a cancellation
            tA.CancelAfter(Gen.ButtonGen.CancellationDelay);
            await Assert.ThrowsAsync<OperationCanceledException>(button.ExecuteAsync);
            tA.Dispose();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsFalse(vm._WasExecutedAsync);
        }

        /// <summary>
        /// Test ASynchronous command with cancellation
        /// </summary>
        [TestMethod]
        public async Task Cancellable_Test()
        {
            bool canExecute = true;
            var vm = new RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen.ButtonGen(() => canExecute);
            Assert.IsFalse(!canExecute && vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");

            var button =        vm.Run_Asynchronous_CancellableButton;
            var cancelButton =  vm.Run_Asynchronous_CancellableButtonCancelCommand;
            Assert.AreEqual(ButtonGen.DisplayText, button.DisplayText);

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            if (button.CanExecute()) await button.ExecuteAsync();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsFalse(vm._CanExecuteWasEvaluated, CanExecute_Evaluated);
            Assert.IsTrue(vm._WasExecutedAsync);

            canExecute = false;

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            if (button.CanExecute()) await button.ExecuteAsync();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsFalse(vm._CanExecuteWasEvaluated, CanExecute_Evaluated);
            Assert.IsTrue(vm._WasExecutedAsync);

            // test cancellation
            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            var tA = new CancellationTokenSource();
            tA.Token.Register(() => button.Cancel()); // register a cancellation
            tA.CancelAfter(Gen.ButtonGen.CancellationDelay);
            await Assert.ThrowsAsync<OperationCanceledException>(button.ExecuteAsync);
            tA.Dispose();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsFalse(vm._WasExecutedAsync);

            vm.Reset();
            Assert.IsFalse(vm._CanExecuteWasEvaluated && vm._WasExecuted && vm._WasExecutedAsync, "Initial State check failed.");
            tA = new CancellationTokenSource();
            tA.Token.Register(() => cancelButton.Execute(null)); // register a cancellation
            tA.CancelAfter(Gen.ButtonGen.CancellationDelay);
            await Assert.ThrowsAsync<OperationCanceledException>(button.ExecuteAsync);
            tA.Dispose();
            Assert.IsFalse(vm._WasExecuted);
            Assert.IsFalse(vm._WasExecutedAsync);
        }
    }
}
