using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void Test_SupportsMultiSelect()
        {
            var wrapper = GetSelector(typeof(GeneratorInputs.Listbox_SupportsMultiSelect));
            var selector = Assert.IsInstanceOfType<RefreshableListBoxDefinition<int, int[], object>>(wrapper.Selector);
                
            Assert.IsFalse(wrapper.GetProperty<bool>("PartialMethodCalled"));
            Assert.IsNotEmpty(selector.Items);
            selector.SelectedItems = [selector.Items[0], selector.Items[2]];
            Assert.HasCount(2, selector.SelectedItems);
            Assert.IsTrue(wrapper.GetProperty<bool>("PartialMethodCalled"));
        }
    }
}
