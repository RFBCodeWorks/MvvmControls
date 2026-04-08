using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;


namespace RFBCodeWorks.Mvvm.Tests
{
    [STATestClass]
    public class CursorServiceTests_MockService : CursorServiceImplementationTests<UIServices.MockCursorService>
    {
        protected override UIServices.MockCursorService GetService() => UIServices.GetMockCursorService();
        private bool tick;
        protected override Task PumpDispatcher()
        {
            if (CursorService.IsBusy && tick)
            {
                CursorService.Reset();
            }
            tick = CursorService.IsBusy;
            return Task.CompletedTask;
        }
    }

    [STATestClass]
    public class CursorServiceTests_DispatchService : CursorServiceImplementationTests<ICursorService>
    {
        protected override ICursorService GetService() => UIServices.GetDispatcherCursorService();
        protected override Task PumpDispatcher()
        {
            ApplicationInitializer.PumpDispatcher();
            return Task.Delay(100, TestContext.CancellationToken);
        }
    }

    /// <summary>
    /// Integration tests for CursorService implementation using MSTest
    /// </summary>
    [STATestClass]
    public abstract class CursorServiceImplementationTests<T> where T : ICursorService
    {
        protected abstract T GetService();
        protected abstract Task PumpDispatcher();
     
        private T? cursorService;
        protected T CursorService => cursorService ??= GetService();

        private readonly SemaphoreSlim testGate = new(1);

        /// <summary>
        /// Gets or sets the test context which provides information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public async Task TestInitialize()
        {
            await testGate.WaitAsync(TestContext.CancellationToken);
            TestContext.WriteLine($"Type of CursorService : {typeof(T).FullName}");
            cursorService = GetService();
            await PumpDispatcher();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            TestContext?.WriteLine("Cleaning up test: {0}", TestContext?.TestName);
            if (CursorService != null && CursorService.IsBusy)
            {
                CursorService.Reset();
                TestContext?.WriteLine("Service was busy, reset called");
            }
            await PumpDispatcher();
            testGate.Release();
        }

        #region Property Tests

        [TestMethod, Timeout(3000, CooperativeCancellation = true)]
        [Description("Verify BusyCursor can be changed")]
        public void BusyCursor_CanBeChanged()
        {
            // Arrange
            var newCursor = Cursors.Hand;
            TestContext.WriteLine("Changing BusyCursor to: {0}", newCursor);

            // Act
            CursorService.BusyCursor = newCursor;

            // Assert
            Assert.AreEqual(newCursor, CursorService.BusyCursor);
            TestContext.WriteLine("BusyCursor successfully changed to: {0}", newCursor);
        }

        [TestMethod, Timeout(3000, CooperativeCancellation = true)]
        [Description("Verify IsBusy defaults to false")]
        public void IsBusy_DefaultsToFalse()
        {
            // Assert
            Assert.IsFalse(CursorService.IsBusy);
            TestContext.WriteLine("IsBusy verified to default to false");
        }

        #endregion


        #region SetBusy Tests

        [TestMethod, Timeout(3000, CooperativeCancellation = true)]
        [Description("Verify SetBusy sets IsBusy to true")]
        public async Task SetBusy_SetsIsBusyToTrue()
        {
            // Act
            CursorService.SetBusy();
            TestContext.WriteLine("SetBusy called");
            ApplicationInitializer.PumpDispatcher();

            // Assert
            Assert.IsTrue(CursorService.IsBusy);
            TestContext.WriteLine("IsBusy is now: {0}", CursorService.IsBusy);

            await PumpDispatcher();
            await PumpDispatcher();
            Assert.IsFalse(CursorService.IsBusy);
        }

        [TestMethod, Timeout(3000, CooperativeCancellation = true)]
        [Description("Verify SetBusy can be called multiple times")]
        public void SetBusy_CanBeCalledMultipleTimes()
        {
            // Act
            CursorService.SetBusy();
            CursorService.SetBusy();
            CursorService.SetBusy();
            TestContext.WriteLine("SetBusy called 3 times");

            // Assert
            Assert.IsTrue(CursorService.IsBusy);
            TestContext.WriteLine("IsBusy is: {0}", CursorService.IsBusy);
        }

        #endregion

        #region SetBusyAsync Tests

        [TestMethod, Timeout(3000, CooperativeCancellation = true)]
        [Description("Verify SetBusyAsync sets IsBusy to true")]
        public async Task SetBusyAsync_SetsIsBusyToTrue()
        {
            // Act
            var t = CursorService.SetBusyAsync(CancellationToken.None);
            await t;
            TestContext.WriteLine("SetBusyAsync called and awaited");

            // Assert
            Assert.IsTrue(CursorService.IsBusy);
            TestContext.WriteLine("IsBusy is now: {0}", CursorService.IsBusy);

            await PumpDispatcher();
            await PumpDispatcher();
            Assert.IsFalse(CursorService.IsBusy);
        }

        [TestMethod, Timeout(3000, CooperativeCancellation = true)]
        [Description("Verify SetBusyAsync completes within reasonable time")]
        public async Task SetBusyAsync_CompletesWithinReasonableTime()
        {
            // Arrange
            var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationToken);
            timeout.CancelAfter(5000);

            // Act
            var task = CursorService.SetBusyAsync(CancellationToken.None);
            await PumpDispatcher();
            var completed = await Task.Run(async () =>
            {
                while (task.IsCompleted == false)
                {
                    timeout.Token.ThrowIfCancellationRequested();
                }
                return true;
            }, TestContext.CancellationToken);

            // Assert
            Assert.IsTrue(completed, "SetBusyAsync did not complete within timeout");
            TestContext.WriteLine("SetBusyAsync completed within timeout");
        }

        [TestMethod, Timeout(3000, CooperativeCancellation = true)]
        [Description("Verify SetBusyAsync respects cancellation token")]
        public async Task SetBusyAsync_RespectsCancellation()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();
            TestContext.WriteLine("Created cancellation token source, canceling after 50ms");

            // Act
            await Assert.ThrowsAsync<OperationCanceledException>(() => CursorService.SetBusyAsync(cts.Token));
            TestContext.WriteLine("SetBusyAsync was cancelled");
        }

        
        #endregion

        #region Reset Tests

        [TestMethod, Timeout(3000, CooperativeCancellation = true)]
        [Description("Verify Reset sets IsBusy to false")]
        public async Task Reset_SetIsBusyToFalse()
        {
            // Arrange
            CursorService.SetBusy();
            await PumpDispatcher();
            TestContext.WriteLine("SetBusy called, IsBusy is: {0}", CursorService.IsBusy);

            // Act
            CursorService.Reset();
            TestContext.WriteLine("Reset called");
            await PumpDispatcher();
            await PumpDispatcher();

            // Assert
            Assert.IsFalse(CursorService.IsBusy);
            TestContext.WriteLine("IsBusy after Reset is: {0}", CursorService.IsBusy);
        }

        [TestMethod, Timeout(3000, CooperativeCancellation = true)]
        [Description("Verify Reset can be called when not busy")]
        public void Reset_CanBeCalledWhenNotBusy()
        {
            // Act & Assert
            try
            {
                CursorService.Reset();
                TestContext.WriteLine("Reset called while not busy - no exception");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Reset threw exception when not busy: {ex.Message}");
            }
        }

        [TestMethod, Timeout(3000, CooperativeCancellation = true)]
        [Description("Verify Reset can be called multiple times")]
        public async Task Reset_CanBeCalledMultipleTimes()
        {
            // Arrange
            CursorService.SetBusy();
            TestContext.WriteLine("SetBusy called");

            // Act & Assert
            bool pump = true;
            try
            {
                var pumper = Task.Run(async () => { while (pump && !TestContext.CancellationToken.IsCancellationRequested) { await Task.Delay(20, TestContext.CancellationToken); await PumpDispatcher(); } }, TestContext.CancellationToken);
                CursorService.Reset();
                CursorService.Reset();
                CursorService.Reset();

                pump = false;
                await pumper;
                TestContext.WriteLine("Reset called 3 times - no exceptions");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Multiple Reset calls threw exception: \n{ex.Message}");
            }
            
            await PumpDispatcher();
            Assert.IsFalse(CursorService.IsBusy);
        }

        #endregion


    }
}
