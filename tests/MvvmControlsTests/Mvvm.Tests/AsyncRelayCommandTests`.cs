using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Input;
using RFBCodeWorks.Mvvm.Primitives;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace RFBCodeWorks.Mvvm.Tests
{
    [STATestClass]
    public class AsyncRelayCommandParameterTests : Mvvm.Primitives.Tests.CommandBaseTests
    {
        private Task GetTask(int v) => GetTask(v, new CancellationTokenSource().Token);
        private async Task GetTask(int loops, CancellationToken token)
        {
            ErrorHandled = false;
            CancelHandled = false;
            int i = 0;
            if (loops < i) throw new ArgumentException();
            while (i < loops)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(1);
                i++;
            }
        }

        protected override CommandBase GetCommand() => new AsyncRelayCommand<int>(cancelableExecute: GetTask);

        private bool CanExecute(int value) => value > 0;
        private bool ErrorHandled;
        private bool CancelHandled;
        private void HandleCancellation(int value) => CancelHandled = true;
        private void HandleError(Exception e) => ErrorHandled = true;

        private static async Task TestCanExecuteAsync(AsyncRelayCommand<int> cmd, bool constructedWithCanExecuteMethod = false)
        {
            Assert.IsTrue(cmd.CanExecute(100), "\n\nCanExecute returned FALSE when expected TRUE");
            if (constructedWithCanExecuteMethod) Assert.IsFalse(cmd.CanExecute(-100), "\n\nCanExecute returned TRUE with invalid input");

            Assert.IsTrue(cmd.CanExecute(10));
            cmd.AllowConcurrentExecution = true;
            var task = cmd.ExecuteAsync(100);
            Assert.IsNotNull(cmd.ExecutionTask, "ExecutionTask Property was not set!");
            Assert.IsTrue(cmd.CanExecute(100), "\n\nCanExecute returned FALSE when expected TRUE");
            Assert.IsTrue(cmd.IsRunning, "\n\nIsRunning returned FALSE while task was running");
            await task;
            Assert.IsFalse(cmd.IsRunning, "\n\nIsRunning returned TRUE after task completed");

            cmd.AllowConcurrentExecution = false;
            task = cmd.ExecuteAsync(100);
            Assert.IsFalse(cmd.CanExecute(100), "\n\nCanExecute Returned TRUE while task was executing and AllowConcurrentExecution was false");
            await task;
            AsyncRelayCommandTests.TestIAsyncRelayCommand(cmd);
        }

        private async Task TestExceptionThrowing(AsyncRelayCommand<int> cmd, bool hasErrorHandler, bool hasCancellationHandler, bool isCancellable = true)
        {
            CancelHandled = false;
            ErrorHandled = false;
            cmd.AllowConcurrentExecution = false;
            Task assertTask;
            if (!hasCancellationHandler && !hasErrorHandler && isCancellable)
            {
                Func<Task> runTask = () => cmd.ExecuteAsync(100);
                assertTask = Assert.ThrowsAsync<OperationCanceledException>(runTask, "\n\nOperationCancelledExpection was not passed to caller!");
                await Task.Delay(2);
                cmd.Cancel();
                await assertTask;
                Assert.IsFalse(CancelHandled, "\n\nCancel was handled unexpectedly");
                Assert.IsFalse(ErrorHandled, "\n\nError was handled unexpectedly");
            }
            else if (!hasErrorHandler)
            {
                Func<Task> runTask = () => cmd.ExecuteAsync(-100);
                await runTask.ThrowAsync<ArgumentException>("\n\nArgumentException was not passed to caller!");
                Assert.IsFalse(CancelHandled, "\n\nCancel was handled unexpectedly");
                Assert.IsFalse(ErrorHandled, "\n\nError was handled unexpectedly");
            }
            else if (!hasCancellationHandler && isCancellable)
            {
                assertTask = cmd.ExecuteAsync(100);
                await Task.Delay(2);
                cmd.Cancel();
                await AsyncRelayCommandTests.AwaitCompletion(cmd);
                Assert.IsFalse(CancelHandled, "\n\nCancel was handled unexpectedly");
                Assert.IsFalse(ErrorHandled, "\n\nOperationCancelledExpection was not handled by the Error Handler!");
            }

            //Test Cancellation Handling
            CancelHandled = false;
            if (hasCancellationHandler)
            {
                Assert.IsFalse(cmd.CanBeCanceled, "\n\nCanBeCanceled returned TRUE while the task was not started");
                Task v = cmd.ExecuteAsync(10);
                Assert.IsTrue(cmd.CanBeCanceled, "\n\nCanBeCanceled returned FALSE while a Cancellable task was running");
                await v;
                Assert.IsFalse(CancelHandled, "\n\nCancellation was handled while task was not cancelled");
                v = cmd.ExecuteAsync(100);
                await Task.Delay(5);
                cmd.Cancel();
                await v;
                Assert.IsTrue(CancelHandled, "\n\nCancellation was not handled when it was expected to be handled");
            }

            //Test Error Handling
            ErrorHandled = false;
            if (hasErrorHandler)
            {
                await cmd.ExecuteAsync(5);
                Assert.IsFalse(ErrorHandled, "\n\nErrorHandled was raised unexpectedly");
                _ = cmd.ExecuteAsync(-100);
                Assert.IsTrue(ErrorHandled, "\n\nError was not handled when it was expected to be handled");
            }

        }

        [TestMethod(DisplayName ="Cancelleable Task")]
        public async Task AsyncRelayCommandTest1()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask);
            await TestCanExecuteAsync(cmd);
            await TestExceptionThrowing(cmd, false, false);
        }

        [TestMethod(DisplayName ="Cancelleable Task + Error Handling")]
        public async Task AsyncRelayCommandTest2()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask, HandleError);
            await TestCanExecuteAsync(cmd);
            await TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false);
        }

        [TestMethod(DisplayName ="Cancelleable Task + Cancellation Handling")]
        public async Task AsyncRelayCommandTest3()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask, HandleCancellation);
            await TestCanExecuteAsync(cmd);
            await TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: true);
        }

        [TestMethod(DisplayName ="Cancelleable Task + CanExecuteValidation")]
        public async Task AsyncRelayCommandTest4()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask, CanExecute);
            await TestCanExecuteAsync(cmd, true);
            await TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false);
        }

        [TestMethod(DisplayName ="Cancelleable Task + CanExecuteValidation + Error Handling")]
        public async Task AsyncRelayCommandTest5()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask, CanExecute, HandleError);
            await TestCanExecuteAsync(cmd, true);
            await TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false);
        }

        [TestMethod(DisplayName ="Cancelleable Task + CanExecuteValidation + Cancellation Handling")]
        public async Task AsyncRelayCommandTest6()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask, CanExecute, null, HandleCancellation);
            await TestCanExecuteAsync(cmd, true);
            await TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: true);
        }

        [TestMethod(DisplayName ="Cancelleable Task + CanExecuteValidation + Error Handling + Cancellation Handling")]
        public async Task AsyncRelayCommandTest7()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask, CanExecute, HandleError, HandleCancellation);
            await TestCanExecuteAsync(cmd, true);
            await TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: true);
        }

        [TestMethod(DisplayName ="Non-Cancellable Task")]
        public async Task AsyncRelayCommandTest11()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(execute: GetTask);
            await TestCanExecuteAsync(cmd, false);
            await TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false, isCancellable: false);
        }

        [TestMethod(DisplayName ="Non-Cancellable Task + CanExecuteValidation")]
        public async Task AsyncRelayCommandTest9()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(execute: GetTask, CanExecute);
            await TestCanExecuteAsync(cmd, true);
            await TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false, isCancellable: false);
        }

        [TestMethod(DisplayName ="Non-Cancellable Task + Error Handling")]
        public async Task AsyncRelayCommandTest8()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(execute: GetTask, HandleError);
            await TestCanExecuteAsync(cmd, false);
            await TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false, isCancellable: false);
        }

        [TestMethod(DisplayName ="Non-Cancellable Task + CanExecuteValidation + Error Handling")]
        public async Task AsyncRelayCommandTest10()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(execute: GetTask, CanExecute, HandleError);
            await TestCanExecuteAsync(cmd, true);
            await TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false, isCancellable: false);
        }
    }
}