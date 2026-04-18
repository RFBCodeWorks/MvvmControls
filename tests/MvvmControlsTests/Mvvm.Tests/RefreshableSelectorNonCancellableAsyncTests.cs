using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Tests
{
    /// <summary>
    /// Demonstrates a ViewModel-style flow using the non-cancellable async
    /// <see cref="RefreshableSelector{T, TList, TSelectedValue}"/> constructor (<c>Func&lt;Task&lt;TList&gt;&gt;</c>).
    /// <br/> Constructor parameters for Selector2 are exposed as class-level properties and configured per-test in <see cref="TestInitialize"/>.
    /// </summary>
    [TestClass]
    public class RefreshableSelectorNonCancellableAsyncTests
    {
        public TestContext TestContext { get; set; }

        private CancellationToken Token => TestContext.CancellationToken;

        /* Selector2 constructor parameters (Func<Task<TList>> overload) */
        private Func<Task<SelectorTestItem[]>> refreshAsync = null!;
        private Func<bool>? canRefresh;
        private Action? onCollectionChanged;
        private Action? onSelectionChanged;
        private bool refreshOnFirstCollectionRequest;

        private async Task<SelectorTestItem[]> GetItemsAsync()
        {
            await Task.Delay(100, Token);
            return SelectorTestItem.CreateArray();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            refreshAsync = GetItemsAsync;
            canRefresh = null;
            onCollectionChanged = null;
            onSelectionChanged = null;
            refreshOnFirstCollectionRequest = false;
        }

        private RefreshableSelector<SelectorTestItem, SelectorTestItem[], string> CreateSelector2() =>
            new(refreshAsync, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest);

        /// <summary>
        /// Demonstrates a ViewModel flow:
        /// <list type="number">
        ///   <item>Selector1 is populated (<see cref="IRefreshableItemSource.EnsureInitialized"/>)</item>
        ///   <item>A Selector1 item is picked, which fires <c>onSelectionChanged</c> and starts Selector2's non-cancellable async refresh</item>
        ///   <item><see cref="IRefreshableItemSource.EnsureInitializedAsync"/> waits for Selector2's in-progress refresh to complete</item>
        ///   <item>A value is then selected from Selector2</item>
        /// </list>
        /// </summary>
        [TestMethod]
        [Timeout(10000)]
        public async Task Test_ViewModel_TwoSelector_NonCancellableAsyncFlow()
        {
            var selector2 = CreateSelector2();

            // Selector1 uses a non-cancellable async refresh;
            // its onSelectionChanged starts Selector2's async refresh (without waiting for it)
            var selector1 = new RefreshableSelector<SelectorTestItem, SelectorTestItem[], string>(
                GetItemsAsync,
                onSelectionChanged: () => selector2.RefreshCommand.Execute(null),
                refreshOnFirstCollectionRequest: false);

            // Step 1: Ensure Selector1 is populated (blocks until async refresh completes)
            selector1.EnsureInitialized();
            Assert.IsTrue(selector1.HasItems, "\n >> Selector1 should have items after EnsureInitialized");

            // Step 2: Pick a Selector1 item — fires onSelectionChanged, starting Selector2's async refresh
            selector1.SelectedItem = selector1.Items[0];
            Assert.IsNotNull(selector1.SelectedItem, "\n >> Selector1.SelectedItem should not be null");
            Assert.IsTrue(selector2.IsRefreshing, "\n >> Selector2 should be refreshing after Selector1 item is selected");

            // Step 3: Wait for Selector2's in-progress refresh to complete, then select a value
            await selector2.EnsureInitializedAsync(Token);
            Assert.IsFalse(selector2.IsRefreshing, "\n >> Selector2 should not be refreshing after EnsureInitializedAsync completes");
            Assert.IsTrue(selector2.HasItems, "\n >> Selector2 should have items after EnsureInitializedAsync");

            selector2.SelectedValuePath = nameof(SelectorTestItem.Name);
            selector2.SelectedItem = selector2.Items[0];
            Assert.IsNotNull(selector2.SelectedItem, "\n >> Selector2.SelectedItem should not be null");
            Assert.IsNotNull(selector2.SelectedValue, "\n >> Selector2.SelectedValue should not be null");
            Assert.AreEqual(selector2.Items[0].Name, selector2.SelectedValue, "\n >> Selector2.SelectedValue should match the selected item's Name");
        }
    }
}
