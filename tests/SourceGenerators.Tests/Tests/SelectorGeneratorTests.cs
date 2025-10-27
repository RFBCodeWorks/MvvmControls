using CommunityToolkit.Mvvm.Input;
using RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests
{
    [TestClass]
    public class ComboBoxGeneratorTests : SelectorGeneratorTests 
    { 
        protected override ISelectorGen GetModel() => new ComboBoxGen();
        [TestMethod]
        public void Test_PropertyName()
        {
            Assert.IsNotNull(new ComboBoxGen().PropertyNameTest);
        }
    }

    [TestClass]
    public class ListBoxGeneratorTests : SelectorGeneratorTests { protected override ISelectorGen GetModel() => new ListBoxGen(); }

    [TestClass]
    public class SelectorGeneratorTests
    {
        protected virtual ISelectorGen GetModel() => new SelectorGen();

        private static void ResetModel(ISelectorGen model)
        {
            model.Reset();
            Assert.IsFalse(
                model.WasCommandNotifiedOnCollectionChange ||
                model.WasCommandNotifiedOnSelectionChange ||
                model.WasRefreshed ||
                model.WasSecondaryCollectionRefreshedOnSelectionChange ||
                model.WasSelectionChangeMethodRun
                );
        }

        private static void TestRefresh(ISelectorGen model, IRefreshableItemSource selector, IRelayCommand cmd)
        {
            if (cmd is IAsyncRelayCommand aCmd)
            {
                Assert.IsFalse(aCmd.IsRunning, "Refresh Task started unexpectedly");
            }
            Assert.IsNotNull(model);
            Assert.IsNotNull(selector);
            Assert.IsNotNull(cmd);
            model.IsRefreshable = false;
            Assert.AreEqual(model.IsRefreshable, cmd.CanExecute(null), "CanExecute does not match IsRefreshable [ 00 ]");
            // validate initial state
            Assert.IsFalse(model.WasRefreshed, "Synchronous - WasRefreshed set unexpectedly");
            Assert.IsFalse(model.WasCommandNotifiedOnCollectionChange, "Synchronous - CollectionChanged command was notified incorrectly");

            model.IsRefreshable = true;
            Assert.AreEqual(model.IsRefreshable, cmd.CanExecute(null), "CanExecute does not match IsRefreshable [ 01 ]");
            cmd.Execute(null);
            if (cmd is IAsyncRelayCommand bCmd)
            {
                bCmd.ExecutionTask!.Wait(3000);
                Assert.IsFalse(bCmd.IsRunning, "Waited for task completion, but task is reporting that it is still running!");
            }
            Assert.IsTrue(model.WasRefreshed, "Synchronous - WasRefreshed was not set true");
            Assert.IsTrue(model.WasCommandNotifiedOnCollectionChange, "Synchronous - CollectionChanged command was not notified");
        }

        private static async Task TestRefreshAsync(ISelectorGen model, IRefreshableItemSource selector, IAsyncRelayCommand cmd, bool isCancellable)
        {
            Assert.IsFalse(cmd.IsRunning, "Task is reporting that it is currently running!");
            Assert.IsNotNull(model);
            Assert.IsNotNull(selector);
            Assert.IsNotNull(cmd);
            model.IsRefreshable = false;
            Assert.IsFalse(cmd.CanExecute(null), "CanExecute does not match IsRefreshable [ 02 ]");
            // validate initial state
            Assert.IsFalse(model.WasRefreshed, "Async - WasRefreshed set unexpectedly");
            Assert.IsFalse(model.WasCommandNotifiedOnCollectionChange, "Async - CollectionChanged command was notified incorrectly");

            model.IsRefreshable = true;
            Assert.IsTrue(cmd.CanExecute(null), "CanExecute does not match IsRefreshable [ 03 ]");
            await cmd.ExecuteAsync(null);
            Assert.IsTrue(model.WasRefreshed, "Async - WasRefreshed was not set true");
            Assert.IsTrue(model.WasCommandNotifiedOnCollectionChange, "Async - CollectionChanged command was not notified");

            model.Reset();
            Task refreshTask = cmd.ExecuteAsync(null); // internal delay time of 350
            try
            {
                await Task.Delay(75);
                Assert.AreEqual(isCancellable, selector.CancelRefreshCommand.CanExecute(null));
                selector.CancelRefreshCommand.Execute(null);
                await refreshTask;
            }
            catch (OperationCanceledException) when (isCancellable)
            {

            }
            Assert.AreEqual(isCancellable, refreshTask.IsCanceled);
        }

        private static void TestSelection(ISelectorGen model, IRefreshableItemSource collection)
        {
            Assert.IsNotNull(model);
            var selector = collection as ISelector;
            Assert.IsNotNull(selector);
            Assert.IsFalse(model.WasSelectionChangeMethodRun, $"{nameof(ISelectorGen.WasSelectionChangeMethodRun)}  property has invalid starting state");
            Assert.IsFalse(model.WasCommandNotifiedOnSelectionChange, $"{nameof(ISelectorGen.WasCommandNotifiedOnSelectionChange)}  property has invalid starting state");
            Assert.IsFalse(model.WasSecondaryCollectionRefreshedOnSelectionChange, $"{nameof(ISelectorGen.WasSecondaryCollectionRefreshedOnSelectionChange)}  set unexpectedly");
            selector.SelectedIndex = 1;
            selector.SelectedIndex = 2;
            Assert.IsTrue(model.WasSelectionChangeMethodRun, $"{nameof(ISelectorGen.WasSelectionChangeMethodRun)}  was not set true");
            Assert.IsTrue(model.WasCommandNotifiedOnSelectionChange, $"{nameof(ISelectorGen.WasCommandNotifiedOnSelectionChange)} was not set true");
            Assert.IsTrue(model.WasSecondaryCollectionRefreshedOnSelectionChange, $"{nameof(ISelectorGen.WasSecondaryCollectionRefreshedOnSelectionChange)} was not set true");
        }

        [TestMethod]
        public void Test_Synchronous()
        {
            var model = GetModel();
            var selector = model.Selector_Synchronous;
            var cmd = selector.RefreshCommand;
            ResetModel(model);
            TestRefresh(model, selector, cmd);
            TestSelection(model, model.Selector_Synchronous);
        }

        [TestMethod]
        public async Task Test_Asynchronous()
        {
            var model = GetModel();
            var selector = model.Selector_Asynchronous;
            var cmd = selector.RefreshCommand;
            ResetModel(model);
            TestRefresh(model, selector, cmd);
            ResetModel(model);
            await TestRefreshAsync(model, selector, (IAsyncRelayCommand)cmd, false);
            TestSelection(model, selector);
        }

        [TestMethod]
        public async Task Test_AsynchronousCancellable()
        {
            var model = GetModel();
            var selector = model.Selector_AsynchronousCancellable;
            var cmd = selector.RefreshCommand;
            ResetModel(model);
            TestRefresh(model, selector, cmd);
            ResetModel(model);
            await TestRefreshAsync(model, selector, (IAsyncRelayCommand)cmd, true);
            TestSelection(model, selector);
        }


    }
}
