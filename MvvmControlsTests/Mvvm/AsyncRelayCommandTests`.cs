using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm;
using RFBCodeWorks.Mvvm.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass()]
    public class AsyncRelayCommandTests2 : Mvvm.Primitives.Tests.CommandBaseTests
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

        private async Task TestCanExecute(AsyncRelayCommand<int> cmd, bool methodSupplied = false)
        {
            Assert.IsTrue(cmd.CanExecute(100), "\n\nCanExecute returned FALSE when expected TRUE");
            if (methodSupplied) Assert.IsFalse(cmd.CanExecute(-100), "\n\nCanExecute returned TRUE with invalid input");

            cmd.AllowConcurrentExecution = true;
            _ = cmd.ExecuteAsync(10);
            var task = cmd.ExecutionTask;
            Assert.IsNotNull(task, "ExecutionTask Property was not set!");
            Assert.IsTrue(cmd.CanExecute(100), "\n\nCanExecute returned FALSE when expected TRUE");
            Assert.IsTrue(cmd.IsRunning, "\n\nIsRunning returned FALSE while task was running");
            await task;
            Assert.IsFalse(cmd.IsRunning, "\n\nIsRunning returned TRUE after task completed");

            cmd.AllowConcurrentExecution = false;
            task = cmd.ExecuteAsync(10);
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
                assertTask = Assert.ThrowsExceptionAsync<OperationCanceledException>(() => cmd.ExecuteAsync(100), "\n\nOperationCancelledExpection was not passed to caller!");
                await Task.Delay(2);
                cmd.Cancel();
                await assertTask;
                Assert.IsFalse(CancelHandled, "\n\nCancel was handled unexpectedly");
                Assert.IsFalse(ErrorHandled, "\n\nError was handled unexpectedly");
            }
            else if (!hasErrorHandler)
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(() => cmd.ExecuteAsync(-100), "\n\nArgumentException was not passed to caller!");
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

        [TestMethod("Cancelleable Task")]
        public void AsyncRelayCommandTest1()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask);
            TestCanExecute(cmd).Wait();
            TestExceptionThrowing(cmd, false, false).Wait();
        }

        [TestMethod("Cancelleable Task + Error Handling")]
        public void AsyncRelayCommandTest2()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask, HandleError);
            TestCanExecute(cmd).Wait();
            
            TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false).Wait();
        }

        [TestMethod("Cancelleable Task + Cancellation Handling")]
        public void AsyncRelayCommandTest3()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask, HandleCancellation);
            TestCanExecute(cmd).Wait();
            
            TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: true).Wait();
        }

        [TestMethod("Cancelleable Task + CanExecuteValidation")]
        public void AsyncRelayCommandTest4()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask, CanExecute);
            TestCanExecute(cmd, true).Wait();
            TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false).Wait();
        }

        [TestMethod("Cancelleable Task + CanExecuteValidation + Error Handling")]
        public void AsyncRelayCommandTest5()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask, CanExecute, HandleError);
            TestCanExecute(cmd, true).Wait();
            
            TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false).Wait();
        }

        [TestMethod("Cancelleable Task + CanExecuteValidation + Cancellation Handling")]
        public void AsyncRelayCommandTest6()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask, CanExecute, null, HandleCancellation);
            TestCanExecute(cmd, true).Wait();
            
            TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: true).Wait();
        }

        [TestMethod("Cancelleable Task + CanExecuteValidation + Error Handling + Cancellation Handling")]
        public void AsyncRelayCommandTest7()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(cancelableExecute: GetTask, CanExecute, HandleError, HandleCancellation);
            TestCanExecute(cmd, true).Wait();
            
            
            TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: true).Wait();
        }

        [TestMethod("Non-Cancellable Task")]
        public void AsyncRelayCommandTest11()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(execute: GetTask);
            TestCanExecute(cmd, false).Wait();
            TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false, isCancellable: false).Wait();
        }

        [TestMethod("Non-Cancellable Task + CanExecuteValidation")]
        public void AsyncRelayCommandTest9()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(execute: GetTask, CanExecute);
            TestCanExecute(cmd, true).Wait();
            TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false, isCancellable: false).Wait();
        }

        [TestMethod("Non-Cancellable Task + Error Handling")]
        public void AsyncRelayCommandTest8()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(execute: GetTask, HandleError);
            TestCanExecute(cmd, false).Wait();
            
            TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false, isCancellable: false).Wait();
        }

        [TestMethod("Non-Cancellable Task + CanExecuteValidation + Error Handling")]
        public void AsyncRelayCommandTest10()
        {
            AsyncRelayCommand<int> cmd = new AsyncRelayCommand<int>(execute: GetTask, CanExecute, HandleError);
            TestCanExecute(cmd, true).Wait();
            
            TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false, isCancellable: false).Wait();
        }
    }
}