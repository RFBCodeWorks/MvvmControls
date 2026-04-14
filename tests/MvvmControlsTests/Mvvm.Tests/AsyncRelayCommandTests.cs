using Microsoft.VisualStudio.TestTools.UnitTesting;

using RFBCodeWorks.Mvvm.Input;
using RFBCodeWorks.Mvvm.Primitives;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Tests
{
    [STATestClass]
    public class AsyncRelayCommandTests : Primitives.Tests.CommandBaseTests
    {
        private Task GetTask() => GetTask(new CancellationTokenSource().Token);
        private async Task GetTask(CancellationToken token)
        {
            ErrorHandled = false;
            CancelHandled = false;
            int i = 0;
            while (i < 25)
            {
                if (ThrowError) throw new ArgumentException("");
                token.ThrowIfCancellationRequested();
                await Task.Delay(3, token);
                i++;
            }
        }

        protected override CommandBase GetCommand() => new AsyncRelayCommand(cancelableExecute: GetTask);

        private bool ThrowError;
        private bool ExpectedCanExecute;

        private bool CanExecute() => ExpectedCanExecute;

        private bool ErrorHandled;
        private bool CancelHandled;
        private void HandleCancellation() => CancelHandled = true;
        private void HandleError(Exception e) => ErrorHandled = true;
        internal static async Task AwaitCompletion(IAsyncRelayCommand cmd) { while (cmd.IsRunning) await Task.Delay(1); }

        private async Task TestCanExecuteAsync(AsyncRelayCommand cmd, bool methodSupplied = false)
        {
            ExpectedCanExecute = true;
            Assert.AreEqual(ExpectedCanExecute, cmd.CanExecute(), $"\n\nCanExecute returned FALSE when expected TRUE\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            if (methodSupplied)
            {
                ExpectedCanExecute = false;
                Assert.IsFalse(cmd.CanExecute(), $"\n\nCanExecute returned TRUE with invalid input\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                ExpectedCanExecute = true;
            }

            cmd.AllowConcurrentExecution = true;
            var task = cmd.ExecuteAsync();
            Assert.IsNotNull(cmd.ExecutionTask, $"ExecutionTask Property was not set!\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            Assert.IsTrue(cmd.CanExecute(), $"\n\nCanExecute returned FALSE when expected TRUE\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            Assert.IsTrue(cmd.IsRunning, $"\n\nIsRunning returned FALSE while task was running\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            await task;
            Assert.IsFalse(cmd.IsRunning, $"\n\nIsRunning returned TRUE after task completed\nLine # : {new StackFrame(true).GetFileLineNumber()}");

            cmd.AllowConcurrentExecution = false;
            task = cmd.ExecuteAsync();
            Assert.IsFalse(cmd.CanExecute(), $"\n\nCanExecute Returned TRUE while task was executing and AllowConcurrentExecution was false\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            await task;

            //Test Cancellation
            cmd.AllowConcurrentExecution = true;
            var t1 = cmd.ExecuteAsync();
            var t2 = cmd.ExecuteAsync();
            var t3 = cmd.ExecuteAsync();
            Assert.AreNotSame(t1, t2, $"\n\n Task1 and Task2 are the same take when expected to be different\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            Assert.AreNotSame(t1, t3, $"\n\n Task1 and Task3 are the same take when expected to be different\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            Assert.AreNotSame(t2, t3, $"\n\n Task2 and Task3 are the same take when expected to be different\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            Assert.IsFalse(cmd.IsCancellationRequested, $"\n\n IsCancellationRequested is TRUE prior to being cancelled\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            if (cmd.CanBeCanceled)
            {
                cmd.SwallowCancellations = false;
                cmd.Cancel();
                Assert.IsTrue(cmd.IsCancellationRequested, $"\n\nIsCancellationRequested is FALSE when expected to be true\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                while (cmd.IsRunning) { await Task.Delay(3); }
                await Task.Delay(10);
                Assert.IsTrue(t3.IsCanceled, $"\n\n Task3 was not cancelled\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                Assert.IsTrue(t2.IsCanceled, $"\n\n Task2 was not cancelled\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                Assert.IsTrue(t1.IsCanceled, $"\n\n Task1 was not cancelled\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            }
            else
            {
                await Task.WhenAll(t1, t2, t3);
            }
            TestIAsyncRelayCommand(cmd);
        }

        private async Task TestExceptionThrowing(AsyncRelayCommand cmd, bool hasErrorHandler, bool hasCancellationHandler, bool isCancellable = true)
        {
            CancelHandled = false;
            ErrorHandled = false;
            cmd.AllowConcurrentExecution = false;
            Task assertTask;
            if (!hasCancellationHandler && !hasErrorHandler && isCancellable)
            {
                Console.WriteLine(" > Testing Cancellation");
                ThrowError = false;
                var func = cmd.ExecuteAsync;
                assertTask = Assert.ThrowsAsync<OperationCanceledException>(func, $"\n\nOperationCancelledExpection was not passed to caller!\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                await Task.Delay(2);
                cmd.Cancel();
                await assertTask;
                Assert.IsFalse(CancelHandled, $"\n\nCancel was handled unexpectedly\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                Assert.IsFalse(ErrorHandled, $"\n\nError was handled unexpectedly\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            }
            else if (!hasErrorHandler)
            {
                Console.WriteLine(" > Testing Error is returned to caller");
                ThrowError = true;
                var func = cmd.ExecuteAsync;
                await Assert.ThrowsAsync<ArgumentException>(func, $"\n\nArgumentException was not passed to caller!\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                Assert.IsFalse(CancelHandled, $"\n\nCancel was handled unexpectedly\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                Assert.IsFalse(ErrorHandled, $"\n\nError was handled unexpectedly\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            }
            else if (!hasCancellationHandler && isCancellable)
            {
                Console.WriteLine(" > Testing OperationCancelledExceptions are ignored");
                ThrowError = false;
                assertTask = cmd.ExecuteAsync();
                await Task.Delay(2);
                cmd.Cancel();
                await AwaitCompletion(cmd);
                Assert.IsFalse(CancelHandled, $"\n\nCancel was handled unexpectedly\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                Assert.IsFalse(ErrorHandled, $"\n\nOperationCancelledExpection was not handled by the Error Handler!\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            }
            ThrowError = false;

            //Test Cancellation Handling
            CancelHandled = false;
            if (hasCancellationHandler)
            {
                Console.WriteLine(" > Testing OperationCancelledExceptions are ignored");

                Assert.IsFalse(cmd.CanBeCanceled, $"\n\nCanBeCanceled returned TRUE while the task was not started\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                Task v = cmd.ExecuteAsync();
                Assert.IsTrue(cmd.CanBeCanceled, $"\n\nCanBeCanceled returned FALSE while a Cancellable task was running\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                await v;
                Assert.IsFalse(CancelHandled, $"\n\nCancellation was handled while task was not cancelled\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                v = cmd.ExecuteAsync();
                await Task.Delay(5);
                cmd.Cancel();
                await v.ContinueWith((t) => { });
                Assert.IsTrue(CancelHandled, $"\n\nCancellation was not handled when it was expected to be handled\nLine # : {new StackFrame(true).GetFileLineNumber()}");
            }

            //Test Error Handling
            ErrorHandled = false;
            if (hasErrorHandler)
            {
                ThrowError = false;
                await cmd.ExecuteAsync();
                Assert.IsFalse(ErrorHandled, $"\n\nErrorHandled was raised unexpectedly\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                ThrowError = true;
                _ = cmd.ExecuteAsync();
                Assert.IsTrue(ErrorHandled, $"\n\nError was not handled when it was expected to be handled\nLine # : {new StackFrame(true).GetFileLineNumber()}");
                ThrowError = false;
            }
        }

        /// <summary>
        /// Test the properties to ensure they don't throw a NotImplementedException
        /// </summary>
        /// <param name="cmd"></param>
        internal static void TestIAsyncRelayCommand(IAsyncRelayCommand cmd)
        {
            _ = cmd.CanBeCanceled;
            _ = cmd.ExecutionTask;
            _ = cmd.IsCancellationRequested;
            _ = cmd.IsRunning;
            _ = cmd.RunningTasks;
        }

        [STATestMethod(DisplayName ="Cancelleable Task")]
        public async Task AsyncRelayCommandTest1()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(cancelableExecute: GetTask);
            await TestCanExecuteAsync(cmd);
            await TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false);
        }

        [STATestMethod(DisplayName ="Cancelleable Task + Error Handling")]
        public async Task AsyncRelayCommandTest2()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(GetTask, errorHandler: HandleError);
            await TestCanExecuteAsync(cmd);
            await TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false);
        }

        [STATestMethod(DisplayName ="Cancelleable Task + Cancellation Handling")]
        public async Task AsyncRelayCommandTest3()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(cancelableExecute: GetTask, cancelReaction: HandleCancellation);
            await TestCanExecuteAsync(cmd);
            await TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: true);
        }

        [STATestMethod(DisplayName ="Cancelleable Task + CanExecuteValidation")]
        public async Task AsyncRelayCommandTest4()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(cancelableExecute: GetTask, CanExecute);
            await TestCanExecuteAsync(cmd, true);
            await TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false);
        }

        [STATestMethod(DisplayName ="Cancelleable Task + CanExecuteValidation + Error Handling")]
        public async Task AsyncRelayCommandTest5()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(cancelableExecute: GetTask, CanExecute, HandleError);
            await TestCanExecuteAsync(cmd, true);
            await TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false);
        }

        [STATestMethod(DisplayName ="Cancelleable Task + CanExecuteValidation + Cancellation Handling")]
        public async Task AsyncRelayCommandTest6()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(cancelableExecute: GetTask, CanExecute, cancelReaction: HandleCancellation);
            await TestCanExecuteAsync(cmd, true);
            await TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: true);
        }

        [STATestMethod(DisplayName ="Cancelleable Task + CanExecuteValidation + Error Handling + Cancellation Handling")]
        public async Task AsyncRelayCommandTest7()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(cancelableExecute: GetTask, CanExecute, HandleError, HandleCancellation);
            await TestCanExecuteAsync(cmd, true);
            await TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: true);
        }

        [STATestMethod(DisplayName ="Non-Cancellable Task")]
        public async Task AsyncRelayCommandTest11()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(execute: GetTask);
            await TestCanExecuteAsync(cmd, false);
            await TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false, isCancellable: false);
        }

        [STATestMethod(DisplayName ="Non-Cancellable Task + CanExecuteValidation")]
        public async Task AsyncRelayCommandTest9()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(execute: GetTask, CanExecute);
            await TestCanExecuteAsync(cmd, true);
            await TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false, isCancellable: false);
        }

        [STATestMethod(DisplayName ="Non-Cancellable Task + Error Handling")]
        public async Task AsyncRelayCommandTest8()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(execute: GetTask, errorHandler: HandleError);
            await TestCanExecuteAsync(cmd, false);
            await TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false, isCancellable: false);
        }

        [STATestMethod(DisplayName ="Non-Cancellable Task + CanExecuteValidation + Error Handling")]
        public async Task AsyncRelayCommandTest10()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(execute: GetTask, CanExecute, HandleError);
            await TestCanExecuteAsync(cmd, true);
            await TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false, isCancellable: false);
        }
    }
}