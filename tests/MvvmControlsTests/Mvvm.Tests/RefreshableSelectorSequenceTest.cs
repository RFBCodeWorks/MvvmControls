using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass]
    public class RefreshableSelectorSequenceTest
    {
        public TestContext TestContext { get; set; }

        private CancellationToken Token => TestContext.CancellationToken;


#pragma warning disable CA1859 // Intentionally tests via the interface
        private IRefreshableSelector Selector1 = null!;
        private IRefreshableSelector Selector2 = null!;
        private IRefreshableSelector Selector3 = null!;
#pragma warning restore CA1859 
        private int selector2RefreshCount;
        private int selector3RefreshCount;


        private static int[] GetIntegers() => [5, 4, 3, 2, 1, 0];

        private async Task<int[]> GetIntegersAsync(CancellationToken token)
        {
            await Task.Delay(250, TestContext.CancellationToken);
            return GetIntegers();
        }

        private async Task<int[]> Selector2RefreshTask(CancellationToken token)
        {
                var i = selector2RefreshCount++;
                await Task.Delay(2500, token);
                return [i, i + 1, i + 2];
        }

        private async Task<int[]> Selector3RefreshTask(CancellationToken token)
        {
                var i = selector3RefreshCount++;
                await Task.Delay(2500, token);
                return [i];
        }

        private static void SelectIfOnlyOneItem(IRefreshableSelector selector)
        {
            if (selector.Items.Count == 1)
                selector.SelectedIndex = 0;
        }

        private static void SelectFirstItem(IRefreshableSelector selector)
        {
            if (selector.Items.Count > 0)
                selector.SelectedIndex = 0;
        }

        private static RefreshableSelector<T, T[], object> CreateSelector<T>(
            Func<CancellationToken, Task<T[]>> getItems, 
            //Func<Task<T[]>> getItems, 
            Action onCollectionChanged, 
            Action? onSelectedItemChanged = null,
            bool refreshOnFirstCollectionRequest = false
            )
        {
            return new RefreshableSelector<T, T[], object>
                (
                getItems,
                canRefresh: () => true,
                onCollectionChanged: onCollectionChanged,
                onSelectionChanged: onSelectedItemChanged,
                refreshOnFirstCollectionRequest: refreshOnFirstCollectionRequest
                );
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Selector1 = CreateSelector(GetIntegersAsync, () => SelectFirstItem(Selector1), () => Selector2.RefreshCommand.Execute(null), true);
            Selector2 = CreateSelector(Selector2RefreshTask, () => SelectFirstItem(Selector2), () => Selector3.RefreshCommand.Execute(null));
            Selector3 = CreateSelector(Selector3RefreshTask, () => SelectFirstItem(Selector3));
        }

        /// <summary>
        /// Tests a complex interaction between selectors that are refreshing in response to each other's collection and selection changes, 
        /// to ensure that the refreshes are happening in the correct sequence and that the correct number of refreshes are being triggered for each selector.
        /// Also ensures that the 'EnsureInitializedAsync' method correctly waits for the last active refresh to complete.
        /// </summary>
        [TestMethod]
        public async Task RefreshSequenceTest()
        {
            // trigger selector 1 async refresh
            Assert.HasCount(0, Selector1.Items);
            await Selector1.EnsureInitializedAsync(Token);
            Assert.IsFalse(Selector1.IsRefreshing);
            Assert.HasCount(6, Selector1.Items);
            Assert.AreEqual(0, Selector1.SelectedIndex); // item was not yet selected due to multiple items in collection.

            await Selector2.EnsureInitializedAsync(CancellationToken.None);
            Assert.IsFalse(Selector2.IsRefreshing);
            Selector1.SelectedIndex = 2;
            Assert.IsTrue(Selector2.IsRefreshing);

            // selector 2 should now be refreshing for the second time (since first refresh was triggered when selector 1 had selectedIndex set to 0 via OnCollectionChanged)
            Assert.IsTrue(Selector2.IsRefreshing);
            await Selector2.EnsureInitializedAsync(CancellationToken.None);
            Assert.IsFalse(Selector2.IsRefreshing);
            Assert.AreEqual(2, selector2RefreshCount);
            Assert.AreEqual(0, Selector2.SelectedIndex);
            Assert.AreEqual(2, Selector2.SelectedItem);
            Selector2.SelectedIndex = 2;

            // select 3 should now be on its 3rd refresh, since select2 has had its item changed 3 times.
            Assert.IsTrue(Selector3.IsRefreshing);
            await Selector3.EnsureInitializedAsync(CancellationToken.None);
            Assert.IsFalse(Selector3.IsRefreshing);
            Assert.AreEqual(3, selector2RefreshCount);
            Assert.AreEqual(0, Selector2.SelectedIndex);
            Assert.AreEqual(3, Selector2.SelectedItem);
        }

    }
}
