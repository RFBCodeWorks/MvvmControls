using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass]
    public class RefreshableSelectorNonCancellableAsyncTests
    {
        public TestContext TestContext { get; set; }

        private CancellationToken Token => TestContext.CancellationToken;

        private Func<Task<int[]>> refreshAsync = null!;
        private Func<bool>? canRefresh;
        private Action? onCollectionChanged;
        private Action? onSelectionChanged;
        private bool refreshOnFirstCollectionRequest;

        private static int[] GetIntegers() => [0, 1, 2, 3, 4, 5];

        private async Task<int[]> GetIntegersAsync()
        {
            await Task.Delay(250, TestContext.CancellationToken);
            return GetIntegers();
        }

        private static int[] Throws() => throw new NotImplementedException();

        private async Task<int[]> ThrowsAsync()
        {
            await Task.Delay(100, TestContext.CancellationToken);
            return Throws();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            refreshAsync = GetIntegersAsync;
            canRefresh = null;
            onCollectionChanged = null;
            onSelectionChanged = null;
            refreshOnFirstCollectionRequest = true;
        }

        private RefreshableSelector<int, int[], object> CreateSelector() =>
            new(refreshAsync, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest);

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Test_EnsureInitialized(bool refreshOnFirst)
        {
            refreshOnFirstCollectionRequest = refreshOnFirst;
            var selector = CreateSelector();
            selector.EnsureInitialized();
            Assert.HasCount(6, selector.Items, "\n >> Collection was not expected count after EnsureInitialized was called");
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Test_EnsureInitializedAsync(bool refreshOnFirst)
        {
            refreshOnFirstCollectionRequest = refreshOnFirst;
            var selector = CreateSelector();
            await selector.EnsureInitializedAsync(Token);
            Assert.HasCount(6, selector.Items, "\n >> Collection was not expected count after EnsureInitializedAsync was called");
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Test_EnsureInitialized_DoesNotRefreshIfAlreadyInitialized(bool refreshOnFirst)
        {
            int count = 0;
            refreshAsync = () => { count++; return GetIntegersAsync(); };
            refreshOnFirstCollectionRequest = refreshOnFirst;
            var selector = CreateSelector();
            await selector.EnsureInitializedAsync(Token); // waits for first refresh to complete
            Assert.AreEqual(1, count, "\n >> count was not updated on first refresh");

            selector.EnsureInitialized();
            Assert.AreEqual(1, count, "\n >> count was updated unexpectedly after EnsureInitialized");
            Assert.HasCount(6, selector.Items, "\n >> Collection was not expected count after EnsureInitialized was called");
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Test_EnsureInitializedAsync_DoesNotRefreshIfAlreadyInitialized(bool refreshOnFirst)
        {
            int count = 0;
            refreshAsync = () => { count++; return GetIntegersAsync(); };
            refreshOnFirstCollectionRequest = refreshOnFirst;
            var selector = CreateSelector();
            await selector.EnsureInitializedAsync(Token); // waits for first refresh to complete
            Assert.AreEqual(1, count, "\n >> count was not updated on first refresh");

            await selector.RefreshAsync(Token);
            Assert.AreEqual(2, count, "\n >> count was not updated after refresh");
            await selector.EnsureInitializedAsync(Token);
            Assert.AreEqual(2, count, "\n >> count was updated unexpectedly after EnsureInitializedAsync");
            Assert.HasCount(6, selector.Items, "\n >> Collection was not expected count after EnsureInitializedAsync was called");
        }

        [TestMethod]
        public void Test_EnsureInitialized_Throws()
        {
            refreshAsync = ThrowsAsync;
            var selector = CreateSelector();
            Assert.Throws<RefreshFailedException>(() => selector.EnsureInitialized(TimeSpan.FromMinutes(10)));
        }

        [TestMethod]
        public async Task Test_EnsureInitializedAsync_Throws()
        {
            refreshAsync = ThrowsAsync;
            var selector = CreateSelector();
            await Assert.ThrowsAsync<RefreshFailedException>(() => selector.EnsureInitializedAsync(Token));
        }

        [TestMethod]
        public void Test_CancelRefreshCommand_IsInactive()
        {
            var selector = CreateSelector();
            Assert.IsFalse(selector.CancelRefreshCommand.CanExecute(null),
                "\n >> CancelRefreshCommand should be inactive for non-cancellable async refresh");
        }

        [TestMethod]
        public void Test_Refresh()
        {
            refreshOnFirstCollectionRequest = false;
            var selector = CreateSelector();
            Assert.HasCount(0, selector.Items);
            selector.Refresh();
            Assert.HasCount(6, selector.Items);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Test_OnCollectionChanged_IsInvokedOnRefresh(bool refreshOnFirst)
        {
            bool collectionChangedInvoked = false;
            onCollectionChanged = () => collectionChangedInvoked = true;
            refreshOnFirstCollectionRequest = refreshOnFirst;
            var selector = CreateSelector();
            await selector.EnsureInitializedAsync(Token);
            Assert.IsTrue(collectionChangedInvoked, "\n >> onCollectionChanged was not invoked after refresh");
        }
    }
}
