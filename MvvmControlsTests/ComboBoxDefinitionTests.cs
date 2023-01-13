using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using RFBCodeWorks.Mvvm.Primitives;
using RFBCodeWorks.Mvvm.Primitives.Tests;
using Behaviors = RFBCodeWorks.WPFBehaviors;

namespace RFBCodeWorks.Mvvm.Tests
{
    [TestClass()]
    public class ComboBoxDefinitionTests : SelectorTests
    {
        public ComboBoxDefinitionTests() : this(new()) { }

        /// <summary>
        /// Set the ItemSourceDefinitionTests for the test methods
        /// </summary>
        /// <param name="definition"></param>
        public ComboBoxDefinitionTests(ComboBoxDefinition<SelectorTestItem, SelectorTestItem[], string> definition) : base(definition)
        {
            ControlDefinition = definition;
        }

        new protected ComboBoxDefinition<SelectorTestItem, SelectorTestItem[], string> ControlDefinition { get; }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestMethod]
        public void ComboBoxBehaviorBindingTest()
        {
            System.Windows.Controls.ComboBox cmb = new();
            cmb.SetValue(Behaviors.ControlDefinitions.ComboBoxDefinitionProperty, ControlDefinition);
            TestControlInteraction(cmb);
        }

        protected override void TestControlInteraction(Control cntrl)
        {
            base.TestControlInteraction(cntrl);
            Assert.IsTrue(cntrl is ComboBox);
            var cmb = cntrl as ComboBox;

            cmb.IsDropDownOpen = true;
            Assert.IsTrue(ControlDefinition.IsDropDownOpen);
            ControlDefinition.IsDropDownOpen = false;
            Assert.IsFalse(cmb.IsDropDownOpen);
        }

    }

    [TestClass()]
    public class ListBoxDefinitionTests : SelectorTests
    {
        public ListBoxDefinitionTests() : this(new()) { }

        /// <summary>
        /// Set the ItemSourceDefinitionTests for the test methods
        /// </summary>
        /// <param name="definition"></param>
        public ListBoxDefinitionTests(ListBoxDefinition<SelectorTestItem, SelectorTestItem[], string> definition) : base(definition)
        {
            ControlDefinition = definition;
        }

        new protected ListBoxDefinition<SelectorTestItem, SelectorTestItem[], string> ControlDefinition { get; }

        [TestMethod]
        public void ListBoxBehaviorBindingTest()
        {
            System.Windows.Controls.ListBox listbox = new();
            listbox.SetValue(Behaviors.ControlDefinitions.ListBoxDefinitionProperty, ControlDefinition);
            TestControlInteraction(listbox);
        }

        protected override void TestControlInteraction(Control cntrl)
        {
            base.TestControlInteraction(cntrl);

            Assert.IsTrue(cntrl is ListBox); // ListView derives from ListBox
            var listbox = cntrl as ListBox;

            //SelectionMode is ONE-WAY BOUND by the behavior - ControlModel -> Control, not the other way around
            ControlDefinition.SelectionMode = SelectionMode.Single;
            Assert.AreEqual(SelectionMode.Single, listbox.SelectionMode);
            ControlDefinition.SelectionMode = SelectionMode.Multiple;
            Assert.AreEqual(SelectionMode.Multiple, listbox.SelectionMode);
            ControlDefinition.SelectionMode = SelectionMode.Extended;
            Assert.AreEqual(SelectionMode.Extended, listbox.SelectionMode);
            
            
        }
    }
}