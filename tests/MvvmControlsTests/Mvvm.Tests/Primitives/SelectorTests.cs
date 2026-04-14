using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Tests;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace RFBCodeWorks.Mvvm.Primitives.Tests
{
    [TestClass]
    public abstract class SelectorTests : ItemSourceTests
    {

        public SelectorTests(ISelector<SelectorTestItem> definition)
        : this((SelectorDefinition<SelectorTestItem, SelectorTestItem[], string>)definition) 
        { }

        /// <summary>
        /// Set the ItemSourceDefinitionTests for the test methods
        /// </summary>
        /// <param name="definition"></param>
        protected SelectorTests(SelectorDefinition<SelectorTestItem, SelectorTestItem[], string> definition) : base(definition)
        {
            ControlDefinition = definition;
        }

        new protected SelectorDefinition<SelectorTestItem, SelectorTestItem[], string>  ControlDefinition { get; }

        /// <summary>
        /// Returns a <see cref="Selector"/> control
        /// </summary>
        protected abstract System.Windows.Controls.Primitives.Selector GetSelector();

        /// <summary>Returns the result of <see cref="GetSelector"/></summary>
        /// <returns>Some type of <see cref="Selector"/></returns>
        protected sealed override ItemsControl GetItemsControl() => GetSelector();

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            ControlDefinition.SelectedValuePath = nameof(SelectorTestItem.Name);
        }

        [TestMethod]
        public void ModelTest_SelectionChanged()
        {
            ControlDefinition.Items = new SelectorTestItem[] { SelectorTestItem.Zero, SelectorTestItem.One, SelectorTestItem.Two, SelectorTestItem.Three, SelectorTestItem.Four, SelectorTestItem.Five};

            int propChanging = 0;
            int propChanged = 0;
            int selectedItemChanged = 0;
            List<string> propNameChanged = new();
            List<string> propNameChanging = new();
            ControlDefinition.PropertyChanging += (o, e) =>
            {
                Assert.IsNotNull(e?.PropertyName);
                propChanging++;
                propNameChanging.Add(e.PropertyName);
            };
            ControlDefinition.PropertyChanged += (o, e) =>
            {
                Assert.IsNotNull(e?.PropertyName);
                propChanged++;
                propNameChanged.Add(e.PropertyName);
            };
            ControlDefinition.SelectedItemChanged += (o, e) =>
            {
                selectedItemChanged++;
            };

            
            Assert.AreEqual(null, ControlDefinition.SelectedItem);
            Assert.AreEqual(-1, ControlDefinition.SelectedIndex);

            ControlDefinition.SelectedItem = 4;
            Assert.AreEqual(1, selectedItemChanged, "SelectedItemChanged event raised incorrect number of times");
            Assert.AreEqual(((IList<SelectorTestItem>)ControlDefinition.Items).IndexOf(ControlDefinition.SelectedItem), ControlDefinition.SelectedIndex, "\nSelectedIndex was not properly updated after setting the SelectedItem");
            Assert.Contains(nameof(ControlDefinition.SelectedIndex), propNameChanging, "\nPropertyChanging does not contain SelectedIndex");
            Assert.Contains(nameof(ControlDefinition.SelectedIndex), propNameChanged, "\nPropertyChanged does not contain SelectedIndex");
            Assert.Contains(nameof(ControlDefinition.SelectedItem), propNameChanging, "\nPropertyChanging does not contain SelectedItem");
            Assert.Contains(nameof(ControlDefinition.SelectedItem), propNameChanged, "\nPropertyChanged does not contain SelectedItem");
            Assert.Contains(nameof(ControlDefinition.SelectedValue), propNameChanging, "\nPropertyChanging does not contain SelectedItem");
            Assert.Contains(nameof(ControlDefinition.SelectedValue), propNameChanged, "\nPropertyChanged does not contain SelectedItem");
            Assert.AreEqual(3, propChanging, "\nPropertyChanging event raised incorrect number of times");
            Assert.AreEqual(3, propChanged, "\nPropertyChanged event raised incorrect number of times");
            propNameChanged.Clear();
            propNameChanging.Clear();

            ControlDefinition.SelectedIndex = 2;
            Assert.AreEqual(2, selectedItemChanged, "SelectedItemChanged event raised incorrect number of times");
            Assert.AreEqual(ControlDefinition.Items[ControlDefinition.SelectedIndex], ControlDefinition.SelectedItem, "\nSelectedItem was not properly updated after setting the SelectedIndex");
            Assert.Contains(nameof(ControlDefinition.SelectedIndex), propNameChanging, "\nPropertyChanging does not contain SelectedIndex");
            Assert.Contains(nameof(ControlDefinition.SelectedIndex), propNameChanged, "\nPropertyChanged does not contain SelectedIndex");
            Assert.Contains(nameof(ControlDefinition.SelectedItem), propNameChanging, "\nPropertyChanging does not contain SelectedItem");
            Assert.Contains(nameof(ControlDefinition.SelectedItem), propNameChanged, "\nPropertyChanged does not contain SelectedItem");
            Assert.Contains(nameof(ControlDefinition.SelectedValue), propNameChanging, "\nPropertyChanging does not contain SelectedItem");
            Assert.Contains(nameof(ControlDefinition.SelectedValue), propNameChanged, "\nPropertyChanged does not contain SelectedItem");
            Assert.AreEqual(6, propChanging, "\nPropertyChanging event raised incorrect number of times");
            Assert.AreEqual(6, propChanged, "\nPropertyChanged event raised incorrect number of times");
            propNameChanged.Clear();
            propNameChanging.Clear();

        }

        [STATestMethod]
        public void ControlTest_SelectedValuePath()
        {
            var selector = GetSelector();
            Assert.AreEqual(selector.SelectedValuePath, ControlDefinition.SelectedValuePath);

            ControlDefinition.SelectedValue = "One";
            Assert.AreEqual(ControlDefinition.SelectedItem, selector.SelectedItem);
            Assert.AreEqual(ControlDefinition.SelectedIndex, selector.SelectedIndex);
            Assert.IsNotNull(ControlDefinition.SelectedItem);
            Assert.AreEqual(ControlDefinition.SelectedItem.Name, selector.SelectedValue);
            Assert.AreEqual(ControlDefinition.SelectedValue, selector.SelectedValue);
        }

        [STATestMethod]
        public void ControlTest_SelectedIndex()
        {
            var selector = GetSelector() as System.Windows.Controls.Primitives.Selector;
            Assert.IsNotNull(selector);

            // Adjust the Index of the MVVM object, which then updates the 
            ControlDefinition.SelectedIndex = 0;
            Assert.AreEqual(ControlDefinition.SelectedItem, selector.SelectedItem);
            
            Assert.AreEqual(ControlDefinition.SelectedIndex, selector.SelectedIndex);
            Assert.IsNotNull(ControlDefinition.SelectedItem);
            Assert.AreEqual(ControlDefinition.SelectedItem.Name, selector.SelectedValue);
            Assert.AreEqual(ControlDefinition.SelectedValue, selector.SelectedValue);

            ControlDefinition.SelectedIndex = 7;
            Assert.AreEqual(ControlDefinition.SelectedItem, selector.SelectedItem);
            Assert.AreEqual(ControlDefinition.SelectedIndex, selector.SelectedIndex);
            Assert.AreEqual(ControlDefinition.SelectedItem.Name, selector.SelectedValue);
            Assert.AreEqual(ControlDefinition.SelectedValue, selector.SelectedValue);
        }

        [STATestMethod]
        public void ControlTest_SelectedItem()
        {
            var selector = GetSelector() as System.Windows.Controls.Primitives.Selector;
            Assert.IsNotNull(selector);

            // Adjust the Index of the MVVM object, which then updates the 
            ControlDefinition.SelectedItem = ControlDefinition.Items[1];
            Assert.AreEqual(ControlDefinition.SelectedItem, selector.SelectedItem);
            Assert.AreEqual(ControlDefinition.SelectedIndex, selector.SelectedIndex);
            Assert.AreEqual(ControlDefinition.SelectedItem.Name, selector.SelectedValue);
            Assert.AreEqual(ControlDefinition.SelectedValue, selector.SelectedValue);

            ControlDefinition.SelectedItem = ControlDefinition.Items[5];
            Assert.AreEqual(ControlDefinition.SelectedItem, selector.SelectedItem);
            Assert.AreEqual(ControlDefinition.SelectedIndex, selector.SelectedIndex);
            Assert.AreEqual(ControlDefinition.SelectedItem.Name, selector.SelectedValue);
            Assert.AreEqual(ControlDefinition.SelectedValue, selector.SelectedValue);
        }
    }
}