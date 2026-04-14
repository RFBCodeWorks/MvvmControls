using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests
{
    [TestClass]
    public class ListBoxGeneratorTests : SelectorGeneratorTests
    {
#if ROSLYN_311
        protected override string GetGeneratorName() => "ListBox";
#else
        protected override IIncrementalGenerator GetGenerator() => new RefreshableListBoxGeneratorRoslyn40();
#endif
        protected override void AssertImplementInterface(object instance) => Assert.IsInstanceOfType<IListBox>(instance);

        [TestMethod]
        public void Test_SupportsMultiSelectFalse()
        {
            var wrapper = GetSelector(typeof(GeneratorInputs.Listbox_SupportsMultiSelectFalse), getAdditionalText: GeneratorInputs.Listbox_SupportsMultiSelectFalse.GetPartialText);
            var selector = Assert.IsInstanceOfType<RefreshableListBoxDefinition<int, int[], object>>(wrapper.Selector);

            bool eventRaised = false;
            void Handler(object? s, EventArgs e) => eventRaised = true;

            // Validate initial state
            Assert.IsNotEmpty(selector.Items);
            Assert.IsFalse(selector.HasItemsSelected);
            Assert.IsNull(selector.SelectedItems);

            // Selection Mode single because not multi-select
            Assert.AreEqual(System.Windows.Controls.SelectionMode.Single, selector.SelectionMode);

            // select some items
            selector.SelectedItemsChanged += Handler;
            selector.SelectedItems = [selector.Items[0], selector.Items[2]];

            // verify proprties and events were raised
            Assert.HasCount(2, selector.SelectedItems);
            Assert.IsTrue(selector.HasItemsSelected);
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void Test_SupportsMultiSelect()
        {
            var wrapper = GetSelector(typeof(GeneratorInputs.Listbox_SupportsMultiSelect), getAdditionalText: GeneratorInputs.Listbox_SupportsMultiSelect.GetPartialText);
            var selector = Assert.IsInstanceOfType<RefreshableListBoxDefinition<int, int[], object>>(wrapper.Selector);

            bool eventRaised = false;
            void Handler(object? s, EventArgs e) => eventRaised = true;

            // Validate initial state
            Assert.IsNotEmpty(selector.Items);
            Assert.IsFalse(selector.HasItemsSelected);
            Assert.IsNull(selector.SelectedItems);
            Assert.IsFalse(wrapper.GetProperty<bool>("PartialMethodCalled"));

            // Selection Mode Multiple because multi-select enabled
            Assert.AreEqual(System.Windows.Controls.SelectionMode.Multiple, selector.SelectionMode);

            // select some items
            selector.SelectedItemsChanged += Handler;
            selector.SelectedItems = [selector.Items[0], selector.Items[2]];

            // verify proprties and events were raised
            Assert.HasCount(2, selector.SelectedItems);
            Assert.IsTrue(selector.HasItemsSelected);
            Assert.IsTrue(eventRaised);
            Assert.IsTrue(wrapper.GetProperty<bool>("PartialMethodCalled"));

        }
    }
}
