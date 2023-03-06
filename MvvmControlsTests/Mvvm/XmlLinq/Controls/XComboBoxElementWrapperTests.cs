using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Tests;
using RFBCodeWorks.Mvvm.Tests.Helpers;
using System.Windows.Controls;

namespace RFBCodeWorks.Mvvm.XmlLinq.Controls.Tests
{
    [TestClass()]
    public class XComboBoxElementProviderTests : XComboBoxElementWrapperTests
    {
        public XComboBoxElementProviderTests() : base(new XElementProvider("TestAttr", new XElementWrapper("Parent"))) { }
    }


    [TestClass()]
    public class XComboBoxAttributeTests : XComboBoxElementWrapperTests
    {
        public XComboBoxAttributeTests() : base(new XAttributeRetriever("TestAttr", new XElementWrapper("Parent"))) { }
    }

    [TestClass()]
    public class XComboBoxElementWrapperTests : ComboBoxDefinitionTests
    {
        public XComboBoxElementWrapperTests() : this(new XElementWrapper("TestElement")) { }

        protected XComboBoxElementWrapperTests(IXValueObject provider) : this(
            new XComboBox<SelectorTestItem, SelectorTestItem[], string>(provider)
            { ItemConverter = ItemConverter }
            )
        { }

        private XComboBoxElementWrapperTests(XComboBox<SelectorTestItem, SelectorTestItem[], string> cmb) : base(cmb)
        {
            ControlDefinition = cmb;
        }

        new protected XComboBox<SelectorTestItem, SelectorTestItem[], string> ControlDefinition { get; }

        protected override void TestControlInteraction(Control cntrl)
        {
            base.TestControlInteraction(cntrl);

            ControlDefinition.StoreValueAsItemIndex = true;
            ControlDefinition.SelectedIndex = 2;
            Assert.AreEqual("2", ControlDefinition.NodeValueSetter.Value);
            ControlDefinition.SelectedIndex = 1;
            Assert.AreEqual("1", ControlDefinition.NodeValueSetter.Value);

            ControlDefinition.StoreValueAsItemIndex = false;
            ControlDefinition.SelectedIndex = 4;
            Assert.AreEqual("four", ControlDefinition.NodeValueSetter.Value);
            ControlDefinition.SelectedIndex = 1;
            Assert.AreEqual("one", ControlDefinition.NodeValueSetter.Value);
        }

        private static string ItemConverter(SelectorTestItem item)
        {
            return item.Name.ToLower();
        }
    }
}