using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class RefreshableSelectorSequenceTests
    {
        public TestContext TestContext { get; set; }

        private CancellationToken Token => TestContext.CancellationToken;


#pragma warning disable CA1859 // Intentionally tests via the interface

        private IDisposable? tokenRegistration;
        private IRefreshableSelector Selector1 = null!;
        private IRefreshableSelector Selector2 = null!;
        private IRefreshableSelector Selector3 = null!;

        private int selector2RefreshCount;
        private int selector3RefreshCount;
        private int selector1SelectedItemChangedCount;
        private int selector2SelectedItemChangedCount;
        private int selector3SelectedItemChangedCount;

        private int s2_cancelCount = 0;
        private int s3_cancelCount = 0;

#pragma warning restore CA1859

        private static int[] GetIntegers() => [5, 4, 3, 2, 1, 0];

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

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        { }

        [TestCleanup]
        public void TestCleanup()
        {
            tokenRegistration?.Dispose();
        }

        [TestInitialize]
        public async Task TestInitialize()
        {
            int delayPeriod = 200;

            selector2RefreshCount = 0;
            selector3RefreshCount = 0;
            selector1SelectedItemChangedCount = 0;
            selector2SelectedItemChangedCount = 0;
            selector3SelectedItemChangedCount = 0;
            s2_cancelCount = 0;
            s3_cancelCount = 0;

            // selector 1's refresh is triggered on initialization and when .Items is requested for first time.
            // When Selector1.SelectedItem changes, it triggers selector 2's refresh
            Selector1 = new RefreshableSelector<int, int[], object>(
                    async token =>
                    {
                        await Task.Delay(delayPeriod, TestContext.CancellationToken);
                        return GetIntegers();
                    },
                    onCollectionChanged: () => SelectFirstItem(Selector1),
                    onSelectionChanged: () =>
                    {
                        selector1SelectedItemChangedCount++;
                        Selector2.RefreshCommand.Execute(null);
                    },
                    refreshOnFirstCollectionRequest: true)
            {
                DisplayMemberPath = nameof(Selector1),
            };

            // selector 2's refresh is triggered by selector 1's selection change
            Selector2 = new RefreshableSelector<int, int[], object>(
                async token =>
                {
                    try
                    {
                        selector2RefreshCount++;
                        var i = selector2RefreshCount;
                        await Task.Delay(delayPeriod, token);
                        return [i + 1, i + 2, i + 3];
                    }
                    catch (OperationCanceledException)
                    {
                        s2_cancelCount++;
                        throw;
                    }
                },
                onCollectionChanged: () => SelectFirstItem(Selector2),
                onSelectionChanged: () =>
                {
                    selector2SelectedItemChangedCount++;
                    Selector3.RefreshCommand.Execute(null);
                },
                refreshOnFirstCollectionRequest: false)
            {
                DisplayMemberPath = nameof(Selector2),
            };

            // selector 3's refresh is triggered by selector 2's selection change
            Selector3 = new RefreshableSelector<int, int[], object>(
                async token =>
                {
                    try
                    {
                        selector3RefreshCount++;
                        var i = selector3RefreshCount;
                        await Task.Delay(delayPeriod, token);
                        return [i];
                    }
                    catch (OperationCanceledException)
                    {
                        s3_cancelCount++;
                        throw;
                    }
                },
                onCollectionChanged: () => SelectFirstItem(Selector3),
                onSelectionChanged: () => 
                {
                    selector3SelectedItemChangedCount++;
                },
                refreshOnFirstCollectionRequest: false)
            {
                DisplayMemberPath = nameof(Selector3),
            };

            tokenRegistration = Token.Register(() =>
            {
                Selector1.CancelRefreshCommand.Execute(null);
                Selector2.CancelRefreshCommand.Execute(null);
                Selector3.CancelRefreshCommand.Execute(null);
             });
        }


        /// <summary>
        /// Selector 1 has had its collection updated, triggering refresh on selector 2.
        /// Verifies that Selector 2 has received 2 successful refreshes and calls its OnCollectionChanged logic the correct number of times
        /// </summary>
        [TestMethod]
        [Timeout(10000, CooperativeCancellation =true)]
        public Task Test_MultipleRefreshesOnSelector2()
        {
            return Run_MultipleRefreshesOnSelector2();
        }

        private async Task Run_MultipleRefreshesOnSelector2()
        {
            // trigger selector 1 async refresh
            Assert.HasCount(0, Selector1.Items, "\n >> Selector1 should report 0 items initially");
            Assert.IsTrue(Selector1.IsRefreshing, "\n >> Selector1 should be refreshing after initialization");
            await Selector1.EnsureInitializedAsync(Token);
            Assert.IsFalse(Selector1.IsRefreshing, "\n >> Selector1 should not be refreshing after initialization completes");
            Assert.HasCount(6, Selector1.Items, "\n >> Selector1 should report 6 items after refresh");
            Assert.AreEqual(0, Selector1.SelectedIndex, "\n >> Selector1 should have selected index 0 after refresh");

            Assert.IsTrue(Selector2.IsRefreshing, "\n >> Selector2 should be refreshing after Selector1's first refresh");
            await Selector2.EnsureInitializedAsync(Token); // ensure selector 2 has completed its refresh triggered by selector 1's selection change
            await Selector3.EnsureInitializedAsync(Token);
            Assert.AreEqual(1, selector2RefreshCount);
            Assert.AreEqual(1, selector2SelectedItemChangedCount);

            // selector 2 should now be refreshing for the second time (since first refresh was triggered when selector 1 had selectedIndex set to 0 via OnCollectionChanged)
            Selector1.SelectedIndex = 1;
            Assert.AreEqual(2, selector1SelectedItemChangedCount, "\n >> Selector1 should have had its selection changed 2 times (Once from first refresh, second explicitly)");

            Assert.IsTrue(Selector2.IsRefreshing, "\n >> Selector2 should be refreshing after Selector1 selection change");
            await Selector2.EnsureInitializedAsync(Token);
            await Selector3.EnsureInitializedAsync(Token);
            Assert.IsFalse(Selector2.IsRefreshing, "\n >> Selector2 should not be refreshing after initialization completes");

            Assert.AreEqual(0, Selector2.SelectedIndex, "\n >> Selector2 should have selected index 0");
            Assert.AreEqual(2, selector2RefreshCount, "\n >> Selector2 should have refreshed 2 times (One cancelled, one completed)");
            Assert.AreEqual(2, selector2SelectedItemChangedCount, "\n >> Selector2 should have had its selection changed 2 times (once per successful refresh)");
            Assert.AreEqual(3, Selector2.SelectedItem, "\n >> Selector2 value should = 3 (number of refreshes + 1)");
        }

        /// <summary>
        /// this test performs the same steps as 'Test_MultipleRefreshesOnSelector2',
        /// but then continues to change the selected index of selector 2 to trigger a refresh on selector 3,
        /// to ensure that the refreshes are happening in the correct sequence and that the correct number of refreshes are being triggered for each selector.
        /// </summary>
        [TestMethod]
        [Timeout(10000, CooperativeCancellation = true)]
        public async Task Test_MultipleRefreshesOnSelector3()
        {
            
            await Run_MultipleRefreshesOnSelector2();
            // assert results from the previous call to ensure expected state
            Assert.AreEqual(2, selector2RefreshCount, "\n >> Selector2 is in incorrect state");
            Assert.AreEqual(0, Selector2.SelectedIndex, "\n >> Selector2 is in incorrect state");
            Assert.AreEqual(2, selector2SelectedItemChangedCount, "\n >> Selector2 is in incorrect state");
            Assert.AreEqual(3, Selector2.SelectedItem, "\n >> Selector2 is in incorrect state");
            Assert.IsFalse(Selector2.IsRefreshing, "\n >> Selector2 should not be refreshing prior to explicit change");


            // select 3 should now be on its 2nd refresh, since select2 has had its item changed 2 times.
            await Selector3.EnsureInitializedAsync(Token);
            Assert.AreEqual(selector2SelectedItemChangedCount, selector3RefreshCount, "\n >> Selector3 should have refreshed 2 times from selector 2's selection changes");

            // select 3 should now be on its 3rd refresh, since select2 has had its item changed 3 times.
            Selector2.SelectedIndex = 2;
            Assert.AreEqual(3, selector2SelectedItemChangedCount, @"
>> Selector2 should have had its selection changed 3 times:
    >> Once from first refresh of selector1 when Selector1 performed OnSelectionChanged (selected index 0) 
    >> second from selector 1 explicitly changed SelectedIndex to 1 -> selector 2 refreshes -> selector 2 OnCollectionChanged selects index 0
    >> third from explicit change");

            Assert.IsTrue(Selector3.IsRefreshing, "\n >> Selector3 should be refreshing after Selector2 selection change");
            await Selector3.EnsureInitializedAsync(Token);
            Assert.IsFalse(Selector3.IsRefreshing, "\n >> Selector3 should not be refreshing after initialization completes");

            Assert.AreEqual(3, selector3RefreshCount, @"
>> Selector3 should have refreshed 3 times: 
    >> 1st = Selector1 Selected item -> Selector2 refresh -> Selector2 OnCollectionChanged selected index 0 -> Selector3 refresh 
    >> 2nd = Selector1 Changed SelectedIndex to 1 -> Selector2 refresh (cancelling previous) -> Selector2 OnCollectionChanged selected an item -> Selector3 refresh 
    >> 3rd = Selector2 Changed SelectedIndex to 2 -> Selector3 refresh");
            Assert.AreEqual(0, Selector3.SelectedIndex, "\n >> Selector3 should have selected index 0");
            Assert.AreEqual(3, Selector3.SelectedItem, "\n >> Selector3 should have selected item 3 (equal to number of refreshes)");
        }

        /// <summary>
        /// Tests a complex interaction between selectors that are refreshing in response to each other's collection and selection changes, 
        /// to ensure that the refreshes are happening in the correct sequence and that the correct number of refreshes are being triggered for each selector.
        /// Also ensures that the 'EnsureInitializedAsync' method correctly waits for the last active refresh to complete.
        /// </summary>
        /// <remarks>
        /// Primary difference between this and the other tests is that the refresh for Selectors 2 and 3 is cancelled 
        /// instead of waiting for completion prior to updated the prior selector's SelectedIndex.
        /// </remarks>
        [TestMethod]
        [Timeout(10000, CooperativeCancellation = true)]
        public async Task Test_MultipleRefreshWithCancellation()
        {
            /* Overview
             *  "ProcessViewModel()" here represents an async call that occurred on the viewmodel after binding was established
             * 
             * Sequence:
             *  - View binds to Selector1, triggering its refresh
             *  - ProcessViewModel() ensures Selector1 is initialized, then selects an item from the collection
             *  - Selector1's selection change triggers a refresh on Selector2 (Cancelling the previous refresh if it hasn't completed yet)
             *      - Since the first refresh is cancelled, Selector3 should not refresh yet
             *   - ProcessViewModel() then ensures Selector2 is initialized (which should be against the second collection) and selects an item from that collection
             *      - Selector3 should have now had 2 refreshes triggered, the first of which is cancelled due to selector 2's selection change
             *  - ProcessViewModel() exits after ensuring Selector3 is initialized against the second refresh
             */

            // trigger selector 1 async refresh
            Assert.HasCount(0, Selector1.Items, "\n >> Selector1 should report 0 items initially");
            Assert.IsTrue(Selector1.IsRefreshing, "\n >> Selector1 should be refreshing after initialization");
            await Selector1.EnsureInitializedAsync(Token);
            Assert.IsFalse(Selector1.IsRefreshing, "\n >> Selector1 should not be refreshing after initialization completes");
            Assert.HasCount(6, Selector1.Items, "\n >> Selector1 should report 6 items after refresh");
            Assert.AreEqual(0, Selector1.SelectedIndex, "\n >> Selector1 should have selected index 0 after refresh");

            // selector 2 should now be refreshing for the second time (since first refresh was triggered when selector 1 had selectedIndex set to 0 via OnCollectionChanged)
            // The first refresh should have been cancelled.
            Selector1.SelectedIndex = 1;
            Assert.AreEqual(2, selector1SelectedItemChangedCount, "\n >> Selector1 should have had its selection changed 2 times (first cancelled, second success)");

            Assert.IsTrue(Selector2.IsRefreshing, "\n >> Selector2 should be refreshing after Selector1 selection change");
            await Selector2.EnsureInitializedAsync(Token);
            Assert.IsFalse(Selector2.IsRefreshing, "\n >> Selector2 should not be refreshing after initialization completes");

            Assert.AreEqual(2, selector2RefreshCount, "\n >> Selector2 should have refreshed 2 times (One cancelled, one completed)");
            Assert.AreEqual(1, s2_cancelCount, "\n >> Selector2 should have cancelled 1 refresh");
            Assert.AreEqual(0, Selector2.SelectedIndex, "\n >> Selector2 should have selected index 0");
            Assert.AreEqual(1, selector2SelectedItemChangedCount, "\n >> Selector2 should have had its selection changed 2 times (once per successful refresh)");
            Assert.AreEqual(3, Selector2.SelectedItem, "\n >> Selector2 value should = 3 (number of refreshes + 1)");

            // select 3 should now be on its 3rd refresh, since select2 has had its item changed 3 times.
            Selector2.SelectedIndex = 2;
            Assert.AreEqual(2, selector2SelectedItemChangedCount, @"
>> Selector2 should have had its selection changed 2 times:
    >> Once after first successful refresh
    >> second from selector 1 explicitly changed SelectedIndex to 2 
    ");
            Assert.IsTrue(Selector3.IsRefreshing, "\n >> Selector3 should be refreshing after Selector2 selection change");
            await Selector3.EnsureInitializedAsync(Token);
            Assert.IsFalse(Selector3.IsRefreshing, "\n >> Selector3 should not be refreshing after initialization completes");

            Assert.AreEqual(2, selector3RefreshCount, $@"
>> Selector3 should have refreshed 2 times: 
    >> 1st = Selector2 successful refresh -> Selector2 OnCollectionChanged selected index 0 -> Selector3 refresh     
    >> 2nd = Selector2 Changed SelectedIndex to 2 -> Selector3 refresh");

            Assert.AreEqual(1, s3_cancelCount, "\n >> Selector3 should have cancelled 1 refresh");
            Assert.AreEqual(0, Selector3.SelectedIndex, "\n >> Selector3 should have selected index 0");
            Assert.AreEqual(2, Selector3.SelectedItem, "\n >> Selector3 should have selected item 2 (equal to number of refreshes)");
        }
    }
}
