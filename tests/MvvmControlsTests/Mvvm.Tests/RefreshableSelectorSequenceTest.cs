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


        private static int[] GetIntegers() => [0, 1, 2, 3, 4, 5];

        private async Task<int[]> GetIntegersAsync(CancellationToken token)
        {
            await Task.Delay(250, TestContext.CancellationToken);
            return GetIntegers();
        }

        private async Task<int[]> Selector2RefreshTask(CancellationToken token)
        {
                var i = selector2RefreshCount++;
                await Task.Delay(250, token);
                return [i, i, i, i, i, i];
        }

        private async Task<int[]> Selector3RefreshTask(CancellationToken token)
        {
                var i = selector3RefreshCount++;
                await Task.Delay(250, token);
                return [i, i, i, i, i, i];
        }

        private static RefreshableSelector<T, T[], object> CreateSelector<T>(Func<CancellationToken, Task<T[]>> getItems, Action onCollectionChanged, Action? onSelectedItemChanged = null)
        {
            return new RefreshableSelector<T, T[], object>
                (
                refreshAsyncCancellable: getItems,
                canRefresh: () => true,
                onCollectionChanged: onCollectionChanged,
                onSelectionChanged: onSelectedItemChanged,
                refreshOnFirstCollectionRequest: false
                );
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Selector1 = CreateSelector(GetIntegersAsync, () => Selector1.SelectedIndex = 1, () => Selector2.RefreshCommand.Execute(null));
            Selector2 = CreateSelector(Selector2RefreshTask, () => Selector2.SelectedIndex = 0, () => Selector3.RefreshCommand.Execute(null));
            Selector3 = CreateSelector(Selector3RefreshTask, () => Selector3.SelectedIndex = 0);
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
            Assert.AreEqual(1, Selector1.SelectedIndex);
            Assert.IsTrue(Selector2.IsRefreshing);
            Selector1.SelectedIndex = 2;

            // selector 2 should now be refreshing for the second time (since first refresh was triggered when selector 1 had selectedIndex set to 0 via OnCollectionChanged)
            Assert.IsTrue(Selector2.IsRefreshing);
            await Selector2.EnsureInitializedAsync(Token);
            Assert.IsFalse(Selector2.IsRefreshing);
            Assert.AreEqual(2, selector2RefreshCount);
            Assert.AreEqual(0, Selector2.SelectedIndex);
            Assert.AreEqual(2, Selector2.SelectedItem);
            Selector2.SelectedIndex = 2;

            // select 3 should now be on its 3rd refresh, since select2 has had its item changed 3 times.
            Assert.IsTrue(Selector3.IsRefreshing);
            await Selector3.EnsureInitializedAsync(Token);
            Assert.IsFalse(Selector3.IsRefreshing);
            Assert.AreEqual(3, selector2RefreshCount);
            Assert.AreEqual(0, Selector2.SelectedIndex);
            Assert.AreEqual(3, Selector2.SelectedItem);
        }

    }
}
