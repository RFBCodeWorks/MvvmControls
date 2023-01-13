using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Tests;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RFBCodeWorks.Mvvm.Primitives.Tests
{
    [TestClass()]
    public class SelectorTests : ItemSourceTests
    {
        public SelectorTests() :this(new()) { }

        /// <summary>
        /// Set the ItemSourceDefinitionTests for the test methods
        /// </summary>
        /// <param name="definition"></param>
        public SelectorTests(SelectorDefinition<SelectorTestItem, SelectorTestItem[], string> definition) : base(definition)
        {
            ControlDefinition = definition;
        }

        new protected SelectorDefinition<SelectorTestItem, SelectorTestItem[], string>  ControlDefinition { get; }


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            ControlDefinition.SelectedValuePath = nameof(SelectorTestItem.Name);
        }

        [TestMethod]
        public void TestSelectionChanged()
        {
            ControlDefinition.Items = new SelectorTestItem[] { SelectorTestItem.Zero, SelectorTestItem.One, SelectorTestItem.Two, SelectorTestItem.Three, SelectorTestItem.Four, SelectorTestItem.Five};

            int propChanging = 0;
            int propChanged = 0;
            int selectedItemChanged = 0;
            List<string> propNameChanged = new();
            List<string> propNameChanging = new();
            ControlDefinition.PropertyChanging += (o, e) =>
            {
                propChanging++;
                propNameChanging.Add(e.PropertyName);
            };
            ControlDefinition.PropertyChanged += (o, e) =>
            {
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
            Assert.IsTrue(propNameChanging.Contains(nameof(ControlDefinition.SelectedIndex)), "\nPropertyChanging does not contain SelectedIndex");
            Assert.IsTrue(propNameChanged.Contains(nameof(ControlDefinition.SelectedIndex)), "\nPropertyChanged does not contain SelectedIndex");
            Assert.IsTrue(propNameChanging.Contains(nameof(ControlDefinition.SelectedItem)), "\nPropertyChanging does not contain SelectedItem");
            Assert.IsTrue(propNameChanged.Contains(nameof(ControlDefinition.SelectedItem)), "\nPropertyChanged does not contain SelectedItem");
            Assert.AreEqual(2, propChanging, "\nPropertyChanging event raised incorrect number of times");
            Assert.AreEqual(2, propChanged, "\nPropertyChanged event raised incorrect number of times");
            propNameChanged.Clear();
            propNameChanging.Clear();

            ControlDefinition.SelectedIndex = 2;
            Assert.AreEqual(2, selectedItemChanged, "SelectedItemChanged event raised incorrect number of times");
            Assert.AreEqual(ControlDefinition.Items[ControlDefinition.SelectedIndex], ControlDefinition.SelectedItem, "\nSelectedItem was not properly updated after setting the SelectedIndex");
            Assert.IsTrue(propNameChanging.Contains(nameof(ControlDefinition.SelectedIndex)), "\nPropertyChanging does not contain SelectedIndex");
            Assert.IsTrue(propNameChanged.Contains(nameof(ControlDefinition.SelectedIndex)), "\nPropertyChanged does not contain SelectedIndex");
            Assert.IsTrue(propNameChanging.Contains(nameof(ControlDefinition.SelectedItem)), "\nPropertyChanging does not contain SelectedItem");
            Assert.IsTrue(propNameChanged.Contains(nameof(ControlDefinition.SelectedItem)), "\nPropertyChanged does not contain SelectedItem");
            Assert.AreEqual(4, propChanging, "\nPropertyChanging event raised incorrect number of times");
            Assert.AreEqual(4, propChanged, "\nPropertyChanged event raised incorrect number of times");
            propNameChanged.Clear();
            propNameChanging.Clear();

        }

        /// <summary>
        /// Test a <see cref="SelectorDefinition{T, E, V}"/> against a Selector control
        /// </summary>
        protected override void TestControlInteraction(Control cntrl)
        {
            base.TestControlInteraction(cntrl);
            Assert.IsTrue(cntrl is System.Windows.Controls.Primitives.Selector );
            var selector = cntrl as System.Windows.Controls.Primitives.Selector;

            Assert.AreEqual(selector.SelectedValuePath, ControlDefinition.SelectedValuePath);

            ControlDefinition.SelectedIndex = 0;
            Assert.AreEqual(ControlDefinition.SelectedItem, selector.SelectedItem);
            Assert.AreEqual(ControlDefinition.SelectedIndex, selector.SelectedIndex);
            Assert.AreEqual(ControlDefinition.SelectedItem.Name, selector.SelectedValue);
            Assert.AreEqual(ControlDefinition.SelectedValue, selector.SelectedValue);

            ControlDefinition.SelectedIndex = 3;
            Assert.AreEqual(ControlDefinition.SelectedItem, selector.SelectedItem);
            Assert.AreEqual(ControlDefinition.SelectedIndex, selector.SelectedIndex);
            Assert.AreEqual(ControlDefinition.SelectedItem.Name, selector.SelectedValue);
            Assert.AreEqual(ControlDefinition.SelectedValue, selector.SelectedValue);
        }
    }
}