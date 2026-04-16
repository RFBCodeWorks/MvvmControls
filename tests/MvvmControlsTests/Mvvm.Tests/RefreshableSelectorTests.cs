using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass]
    public class RefreshableSelectorTests
    {
        public TestContext TestContext { get; set; }

        public CancellationToken Token => TestContext.CancellationToken;

        private static int[] GetIntegers() => [0, 1, 2, 3, 4, 5];
        private static async Task<int[]> GetIntegersAsync(CancellationToken token)
        {
            await Task.Delay(250, token);
            return GetIntegers();
        }
        private static int[] Throws() => throw new NotImplementedException();
        private async Task<int[]> ThrowsAsync()
        {
            await Task.Delay(100, TestContext.CancellationToken);
            return Throws();
        }

        private static void AssertInitialCount(ISelector selector, bool refreshOnFirstRequest, bool isAsync)
        {
            if (refreshOnFirstRequest && !isAsync)
                Assert.HasCount(6, selector.Items, "\n >> Collection was not refreshed when accessing Selector.Items");
            else
                Assert.HasCount(0, selector.Items, "\n >> Collection had values unexpectedly on first access of Selector.Items");
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Test_EnsureInitialized_DoesNotRefreshIfAlreadyInitialized(bool refreshOnFirstRequest)
        {
            int count = 0;
            int[] Run() { count++; return GetIntegers(); }
            var selector = new RefreshableSelector<int, int[], object>(Run, refreshOnFirstCollectionRequest: refreshOnFirstRequest);
            AssertInitialCount(selector, refreshOnFirstRequest, false);
            Assert.AreEqual(refreshOnFirstRequest ? 1 : 0, count);

            // reset for ease of testing
            count = 0;
            selector.Refresh();
            Assert.AreEqual(1, count, "\n >> count was not updated after refresh");
            selector.EnsureInitialized();
            Assert.AreEqual(1, count, "\n >> count was updated unexpected after EnsureInitialize");
            Assert.HasCount(6, selector.Items, "\n >> Collection was not expected count after EnsureInitialized was called");
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Test_EnsureInitializedAsync_DoesNotRefreshIfAlreadyInitialized(bool refreshOnFirstRequest)
        {
            int count = 0;
            Task<int[]> Run() { count++; return GetIntegersAsync(Token); }
            var selector = new RefreshableSelector<int, int[], object>(Run, refreshOnFirstCollectionRequest: refreshOnFirstRequest);
            AssertInitialCount(selector, refreshOnFirstRequest, true);
            Assert.AreEqual(refreshOnFirstRequest ? 1 : 0, count);
            await selector.EnsureInitializedAsync(Token); // waits for first refresh complete
            Assert.AreEqual(1, count, "\n >> count was not updated on first refresh");

            await selector.RefreshAsync(Token);
            Assert.AreEqual(2, count, "\n >> count was not updated after refresh");
            await selector.EnsureInitializedAsync(Token);
            Assert.AreEqual(2, count, "\n >> count was updated unexpected after EnsureInitialize");
            Assert.HasCount(6, selector.Items, "\n >> Collection was not expected count after EnsureInitialized was called");
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Test_EnsureInitialized(bool refreshOnFirstRequest)
        {
            var selector = new RefreshableSelector<int, int[], object>(GetIntegers, refreshOnFirstCollectionRequest: refreshOnFirstRequest);
            AssertInitialCount(selector, refreshOnFirstRequest, false);
            selector.EnsureInitialized();
            Assert.HasCount(6, selector.Items, "\n >> Collection was not expected count after EnsureInitialized was called");
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Test_EnsureInitialized_AsynchronousRefresh(bool refreshOnFirstRequest)
        {
            var selector = new RefreshableSelector<int, int[], object>(GetIntegersAsync, refreshOnFirstCollectionRequest: refreshOnFirstRequest);
            AssertInitialCount(selector, refreshOnFirstRequest, true);
            selector.EnsureInitialized();
            Assert.HasCount(6, selector.Items, "\n >> Collection was not expected count after EnsureInitialized was called");
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Test_EnsureInitializedAsync(bool refreshOnFirstRequest)
        {
            var selector = new RefreshableSelector<int, int[], object>(GetIntegersAsync, refreshOnFirstCollectionRequest: refreshOnFirstRequest);
            AssertInitialCount(selector, refreshOnFirstRequest, true);
            await selector.EnsureInitializedAsync(TestContext.CancellationToken);
            Assert.HasCount(6, selector.Items, "\n >> Collection was not expected count after EnsureInitializedAsync was called");
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Test_EnsureInitializedAsync_SynchronousRefresh(bool refreshOnFirstRequest)
        {
            var selector = new RefreshableSelector<int, int[], object>(GetIntegers, refreshOnFirstCollectionRequest: refreshOnFirstRequest);
            AssertInitialCount(selector, refreshOnFirstRequest, false);
            await selector.EnsureInitializedAsync(TestContext.CancellationToken);
            Assert.HasCount(6, selector.Items, "\n >> Collection was not expected count after EnsureInitializedAsync was called");
        }

        [TestMethod]
        public void Test_EnsureInitialized_Throws_SynchronousRefresh()
        {
            var selector = new RefreshableSelector<int, int[], object>(Throws);
            Assert.Throws<RefreshFailedException>(() => selector.EnsureInitialized());
        }

        [TestMethod]
        public void Test_EnsureInitialized_Throws_AynchronousRefresh()
        {
            var selector = new RefreshableSelector<int, int[], object>(ThrowsAsync);
            Assert.Throws<RefreshFailedException>(() => selector.EnsureInitialized());
        }

        [TestMethod]
        public void Test_EnsureInitialized_Throws_ExceedsWaitTime()
        {
            var selector = new RefreshableSelector<int, int[], object>(ThrowsAsync);
            Assert.Throws<RefreshFailedException>(() => selector.EnsureInitialized(TimeSpan.Zero));
        }

        [TestMethod]
        public async Task Test_EnsureInitializedAsync_Throws_SynchronousRefresh()
        {
            var selector = new RefreshableSelector<int, int[], object>(Throws);
            await Assert.ThrowsAsync<RefreshFailedException>(() => selector.EnsureInitializedAsync(TestContext.CancellationToken));
        }

        [TestMethod]
        public async Task Test_EnsureInitializedAsync_Throws_AsynchronousRefresh()
        {
            var selector = new RefreshableSelector<int, int[], object>(ThrowsAsync);
            await Assert.ThrowsAsync<RefreshFailedException>(() => selector.EnsureInitializedAsync(TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Test_RefreshOnInitialize_Synchronous(bool refreshOnFirstRequest)
        {
            var selector = new RefreshableSelector<int, int[], object>(GetIntegers, refreshOnFirstCollectionRequest: refreshOnFirstRequest);
            AssertInitialCount(selector, refreshOnFirstRequest, false);
            Assert.IsFalse(selector.IsRefreshing);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Test_RefreshOnInitialize_Asynchronous(bool enabled)
        {
            bool eventRaised = false;
            bool propChanged = false;
            var selector = new RefreshableSelector<int, int[], object>(GetIntegersAsync, refreshOnFirstCollectionRequest: enabled);
            selector.PropertyChanged += (o, e) =>
            {
                eventRaised = true;
                Console.WriteLine(e.PropertyName);
                propChanged |= e.PropertyName == nameof(selector.Items);
            };
            Assert.HasCount(0, selector.Items); // first result - triggers the async execution
            Assert.AreEqual(enabled, selector.IsRefreshing);

            if (enabled)
            {
                await selector.RefreshAsync(Token);
                Assert.IsTrue(eventRaised);
                Assert.IsTrue(propChanged);
                Assert.HasCount(6, selector.Items); // updated value
            }

            Assert.IsFalse(selector.IsRefreshing);
        }

        [TestMethod]
        public void Test_Refresh()
        {
            var selector = new RefreshableSelector<int, int[], object>(GetIntegers, refreshOnFirstCollectionRequest: false);
            Assert.HasCount(0, selector.Items);
            selector.Refresh();
            Assert.HasCount(6, selector.Items);
        }

        [TestMethod]
        public void Test_Refresh_WhenMethodIsAsync()
        {
            var selector = new RefreshableSelector<int, int[], object>(GetIntegersAsync, refreshOnFirstCollectionRequest: false);
            Assert.HasCount(0, selector.Items);
            selector.Refresh();
            Assert.HasCount(6, selector.Items);
        }

        [TestMethod]
        public async Task Test_RefreshAsync_WhenMethodIsSynchronous()
        {
            var selector = new RefreshableSelector<int, int[], object>(GetIntegers, refreshOnFirstCollectionRequest: false);
            Assert.HasCount(0, selector.Items);
            await selector.RefreshAsync(Token);
            Assert.HasCount(6, selector.Items);
        }

        [TestMethod]
        public async Task Test_RefreshAsync_MultipleCommandExecutions_CancelsOldTask()
        {
            var factory = new TaskFactory();
            var selector = new RefreshableSelector<int, int[], object>(factory.Create);
            var command = Assert.IsInstanceOfType<IAsyncRelayCommand>(selector.RefreshCommand);

            Dictionary<TaskCompletionSource<int[]>, CancellationTokenRegistration> tasks = [];
            factory.Created += (o, e) => tasks[e.tcs] = e.reg;

            var t1 = command.ExecuteAsync(Token);
            Assert.HasCount(1, tasks);
            
            var t2 = command.ExecuteAsync(Token);
            Assert.HasCount(2, tasks);

            await Assert.ThrowsAsync<OperationCanceledException>(() => t1);
            Assert.IsTrue(t1.IsCanceled);

            Assert.IsFalse(t2.IsCompleted);
            factory.LastTCS!.SetResult([]);
            Assert.IsTrue(t2.IsCompleted);
        }
        private class TaskFactory
        {
            public TaskCompletionSource<int[]>? LastTCS { get; private set; }

            public event EventHandler<(TaskCompletionSource<int[]> tcs, CancellationTokenRegistration reg)>? Created;
            public Task<int[]> Create(CancellationToken token)
            {
                var tcs = new TaskCompletionSource<int[]>();
                var reg = token.Register(() => tcs.SetCanceled());
                Created?.Invoke(this, (tcs, reg));
                LastTCS = tcs;
                return tcs.Task;
            }
        }

        [TestMethod]
        public async Task Test_RefreshAsync_MultipleCalls_ReturnsRunningTask()
        {
            var tcs = new TaskCompletionSource<int[]>();
            var selector = new RefreshableSelector<int, int[], object>(() => tcs.Task);

            var firstTask = selector.RefreshAsync(Token);
            var secondTask = selector.RefreshAsync(Token);
            Assert.AreSame(firstTask, secondTask);
            tcs.TrySetResult(GetIntegers());
            Assert.HasCount(6, await tcs.Task);
        }

        [TestMethod]
        public async Task Test_RefreshAsync_Cancellation_ViaCancellationCommand()
        {
            var tcs = new TaskCompletionSource<int[]>();
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(Token);
            cts.Token.Register(() => tcs.TrySetCanceled(cts.Token));
            
            var selector = new RefreshableSelector<int, int[], object>((CancellationToken token) =>
            {
                token.Register(() => cts.Cancel());
                return tcs.Task;
            });

            Assert.IsFalse(tcs.Task.IsCompleted);

            var command = Assert.IsInstanceOfType<IAsyncRelayCommand>(selector.RefreshCommand);
            var cancelCommand = Assert.IsInstanceOfType<ICommand>(selector.CancelRefreshCommand);
            
            var refreshTask = selector.RefreshAsync(Token);
            
            Assert.IsTrue(selector.IsRefreshing);
            Assert.IsTrue(command.IsRunning);
            Assert.IsTrue(cancelCommand.CanExecute(null));
            
            cancelCommand.Execute(null);
            Assert.IsFalse(command.IsRunning);
            Assert.IsFalse(selector.IsRefreshing);
            await Assert.ThrowsAsync<OperationCanceledException>(() => refreshTask);   
        }

        [TestMethod]
        public async Task Test_RefreshAsync_Cancellation_ViaToken()
        {
            var tcs = new TaskCompletionSource<int[]>();
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(Token);
            cts.Token.Register(() => tcs.TrySetCanceled(cts.Token));

            var selector = new RefreshableSelector<int, int[], object>((CancellationToken token) =>
            {
                token.Register(() => cts.Cancel());
                return tcs.Task;
            });

            Assert.IsFalse(tcs.Task.IsCompleted);

            var command = Assert.IsInstanceOfType<IAsyncRelayCommand>(selector.RefreshCommand);
            var refreshTask = selector.RefreshAsync(Token);

            Assert.IsTrue(selector.IsRefreshing);
            Assert.IsTrue(command.IsRunning);

            cts.Cancel();
            Assert.IsFalse(command.IsRunning);
            Assert.IsFalse(selector.IsRefreshing);
            await Assert.ThrowsAsync<OperationCanceledException>(() => refreshTask);
        }

        /// <summary>
        /// If the incoming task did not accept a cancellation token, then cancellation requests will not be respected.
        /// </summary>
        [TestMethod]
        public void Test_RefreshAsync_Cancellation_NonCancellableTask()
        {
            var tcs = new TaskCompletionSource<int[]>();
            using var testContextRegistration = Token.Register(() => tcs.TrySetCanceled(Token));

            var selector = new RefreshableSelector<int, int[], object>(() => tcs.Task);
            
            var command = Assert.IsInstanceOfType<IAsyncRelayCommand>(selector.RefreshCommand);
            var cancelCommand = Assert.IsInstanceOfType<ICommand>(selector.CancelRefreshCommand);
            var refreshTask = selector.RefreshAsync(Token);

            Assert.IsTrue(selector.IsRefreshing);
            Assert.IsTrue(command.IsRunning);

            // the cancellation command cannot be executed
            Assert.IsFalse(cancelCommand.CanExecute(null));

            // call the cancelCommand
            cancelCommand.Execute(null);
            Assert.IsTrue(command.IsRunning);
            Assert.IsTrue(selector.IsRefreshing);

            // cancel from the command itself
            command.Cancel();
            Assert.IsTrue(command.IsRunning);
            Assert.IsTrue(selector.IsRefreshing);
        }
    }
}
