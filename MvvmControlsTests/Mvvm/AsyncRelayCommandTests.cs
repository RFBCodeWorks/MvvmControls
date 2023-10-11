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
                if (ThrowError) throw new ArgumentException();
                token.ThrowIfCancellationRequested();
                await Task.Delay(2);
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

        private async Task TestCanExecute(AsyncRelayCommand cmd, bool methodSupplied = false)
        {
            Console.WriteLine("Current Line# 41");
            ExpectedCanExecute = true;
            Assert.AreEqual(ExpectedCanExecute, cmd.CanExecute(), "\n\nCanExecute returned FALSE when expected TRUE");
            if (methodSupplied)
            {
                ExpectedCanExecute = false;
                Assert.IsFalse(cmd.CanExecute(), "\n\nCanExecute returned TRUE with invalid input");
                ExpectedCanExecute = true;
            }

            cmd.AllowConcurrentExecution = true;
            _ = cmd.ExecuteAsync();
            var task = cmd.ExecutionTask;
            Console.WriteLine("Current Line# 54");
            Assert.IsNotNull(task, "ExecutionTask Property was not set!");
            Assert.IsTrue(cmd.CanExecute(), "\n\nCanExecute returned FALSE when expected TRUE");
            Assert.IsTrue(cmd.IsRunning, "\n\nIsRunning returned FALSE while task was running");
            await task;
            Assert.IsFalse(cmd.IsRunning, "\n\nIsRunning returned TRUE after task completed");

            cmd.AllowConcurrentExecution = false;
            task = cmd.ExecuteAsync();
            Assert.IsFalse(cmd.CanExecute(), "\n\nCanExecute Returned TRUE while task was executing and AllowConcurrentExecution was false");
            await task;

            //Test Cancellation
            Console.WriteLine("Current Line# 67");
            cmd.AllowConcurrentExecution = true;
            var t1 = cmd.ExecuteAsync();
            var t2 = cmd.ExecuteAsync();
            var t3 = cmd.ExecuteAsync();
            Assert.AreNotSame(t1, t2, "\n\n Task1 and Task2 are the same take when expected to be different");
            Assert.AreNotSame(t1, t3, "\n\n Task1 and Task3 are the same take when expected to be different");
            Assert.AreNotSame(t2, t3, "\n\n Task2 and Task3 are the same take when expected to be different");
            Assert.IsFalse(cmd.IsCancellationRequested, "\n\n IsCancellationRequested is TRUE prior to being cancelled");
            if (cmd.CanBeCanceled)
            {
                Console.WriteLine("Current Line# 78");
                cmd.SwallowCancellations = false;
                cmd.Cancel();
                Assert.IsTrue(cmd.IsCancellationRequested, "\n\nIsCancellationRequested is FALSE when expected to be true");
                while (cmd.IsRunning) { await Task.Delay(3); }
                await Task.Delay(10);
                Assert.IsTrue(t3.IsCanceled, "\n\n Task3 was not cancelled");
                Assert.IsTrue(t2.IsCanceled, "\n\n Task2 was not cancelled");
                Assert.IsTrue(t1.IsCanceled, "\n\n Task1 was not cancelled");
            }
            else
            {
                Console.WriteLine("Current Line# 90");
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
                Console.WriteLine("Current Line# 103");
                ThrowError = false;
                assertTask = Assert.ThrowsExceptionAsync<OperationCanceledException>(() => cmd.ExecuteAsync(), "\n\nOperationCancelledExpection was not passed to caller!");
                await Task.Delay(2);
                cmd.Cancel();
                await assertTask;
                Assert.IsFalse(CancelHandled, "\n\nCancel was handled unexpectedly");
                Assert.IsFalse(ErrorHandled, "\n\nError was handled unexpectedly");
            }
            else if (!hasErrorHandler)
            {
                Console.WriteLine("Current Line# 113");
                ThrowError = true;
                await Assert.ThrowsExceptionAsync<ArgumentException>(() => cmd.ExecuteAsync(), "\n\nArgumentException was not passed to caller!");
                Assert.IsFalse(CancelHandled, "\n\nCancel was handled unexpectedly");
                Assert.IsFalse(ErrorHandled, "\n\nError was handled unexpectedly");
            }
            else if (!hasCancellationHandler && isCancellable)
            {
                Console.WriteLine("Current Line# 122");
                ThrowError = false;
                assertTask = cmd.ExecuteAsync();
                await Task.Delay(2);
                cmd.Cancel();
                await AwaitCompletion(cmd);
                Assert.IsFalse(CancelHandled, "\n\nCancel was handled unexpectedly");
                Assert.IsFalse(ErrorHandled, "\n\nOperationCancelledExpection was not handled by the Error Handler!");
            }
            ThrowError = false;

            //Test Cancellation Handling
            CancelHandled = false;
            if (hasCancellationHandler)
            {
                Console.WriteLine("Current Line# 137");
                Assert.IsFalse(cmd.CanBeCanceled, "\n\nCanBeCanceled returned TRUE while the task was not started");
                Task v = cmd.ExecuteAsync();
                Assert.IsTrue(cmd.CanBeCanceled, "\n\nCanBeCanceled returned FALSE while a Cancellable task was running");
                await v;
                Assert.IsFalse(CancelHandled, "\n\nCancellation was handled while task was not cancelled");
                v = cmd.ExecuteAsync();
                await Task.Delay(5);
                cmd.Cancel();
                await v.ContinueWith((t) => { });
                Assert.IsTrue(CancelHandled, "\n\nCancellation was not handled when it was expected to be handled");
            }

            //Test Error Handling
            ErrorHandled = false;
            if (hasErrorHandler)
            {
                Console.WriteLine("Current Line# 154");
                ThrowError = false;
                await cmd.ExecuteAsync();
                Assert.IsFalse(ErrorHandled, "\n\nErrorHandled was raised unexpectedly");
                ThrowError = true;
                _ = cmd.ExecuteAsync();
                Assert.IsTrue(ErrorHandled, "\n\nError was not handled when it was expected to be handled");
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

        [TestMethod("Cancelleable Task")]
        public void AsyncRelayCommandTest1()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(cancelableExecute: GetTask);
            TestCanExecute(cmd).Wait();
            TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false).Wait();
        }

        [TestMethod("Cancelleable Task + Error Handling")]
        public void AsyncRelayCommandTest2()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(GetTask, errorHandler: HandleError);
            TestCanExecute(cmd).Wait();

            TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false).Wait();
        }

        [TestMethod("Cancelleable Task + Cancellation Handling")]
        public void AsyncRelayCommandTest3()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(cancelableExecute: GetTask, cancelReaction: HandleCancellation);
            TestCanExecute(cmd).Wait();

            TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: true).Wait();
        }

        [TestMethod("Cancelleable Task + CanExecuteValidation")]
        public void AsyncRelayCommandTest4()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(cancelableExecute: GetTask, CanExecute);
            TestCanExecute(cmd, true).Wait();
            TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false).Wait();
        }

        [TestMethod("Cancelleable Task + CanExecuteValidation + Error Handling")]
        public void AsyncRelayCommandTest5()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(cancelableExecute: GetTask, CanExecute, HandleError);
            TestCanExecute(cmd, true).Wait();

            TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false).Wait();
        }

        [TestMethod("Cancelleable Task + CanExecuteValidation + Cancellation Handling")]
        public void AsyncRelayCommandTest6()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(cancelableExecute: GetTask, CanExecute, cancelReaction: HandleCancellation);
            TestCanExecute(cmd, true).Wait();

            TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: true).Wait();
        }

        [TestMethod("Cancelleable Task + CanExecuteValidation + Error Handling + Cancellation Handling")]
        public void AsyncRelayCommandTest7()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(cancelableExecute: GetTask, CanExecute, HandleError, HandleCancellation);
            TestCanExecute(cmd, true).Wait();


            TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: true).Wait();
        }

        [TestMethod("Non-Cancellable Task")]
        public void AsyncRelayCommandTest11()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(execute: GetTask);
            TestCanExecute(cmd, false).Wait();
            TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false, isCancellable: false).Wait();
        }

        [TestMethod("Non-Cancellable Task + CanExecuteValidation")]
        public void AsyncRelayCommandTest9()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(execute: GetTask, CanExecute);
            TestCanExecute(cmd, true).Wait();
            TestExceptionThrowing(cmd, hasErrorHandler: false, hasCancellationHandler: false, isCancellable: false).Wait();
        }

        [TestMethod("Non-Cancellable Task + Error Handling")]
        public void AsyncRelayCommandTest8()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(execute: GetTask, errorHandler: HandleError);
            TestCanExecute(cmd, false).Wait();

            TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false, isCancellable: false).Wait();
        }

        [TestMethod("Non-Cancellable Task + CanExecuteValidation + Error Handling")]
        public void AsyncRelayCommandTest10()
        {
            AsyncRelayCommand cmd = new AsyncRelayCommand(execute: GetTask, CanExecute, HandleError);
            TestCanExecute(cmd, true).Wait();

            TestExceptionThrowing(cmd, hasErrorHandler: true, hasCancellationHandler: false, isCancellable: false).Wait();
        }
    }
}