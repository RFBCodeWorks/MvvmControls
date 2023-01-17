using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Tests;
using System.Windows.Controls;

namespace RFBCodeWorks.Mvvm.XmlLinq.Controls.Tests
{
    [TestClass()]
    public class XCheckBoxElementProviderTests : XCheckBoxElementWrapperTests
    {

        public XCheckBoxElementProviderTests() : base(new XCheckBox(new XElementProvider("TestAttr", new XElementWrapper("Parent")), BooleanConverter.StoreAsBinary)) { }
    }


    [TestClass()]
    public class XCheckBoxAttributeTests : XCheckBoxElementWrapperTests
    {

        public XCheckBoxAttributeTests() : base(new XCheckBox(new XAttributeRetriever("TestAttr", new XElementWrapper("Parent")), BooleanConverter.StoreAsBinary)) { }
    }
    
    [TestClass()]
    public class XCheckBoxElementWrapperTests : CheckBoxDefinitionTest
    {

        public XCheckBoxElementWrapperTests() : this(new XCheckBox(new XElementWrapper("TestElement"), BooleanConverter.StoreAsBinary)) { }
        protected XCheckBoxElementWrapperTests(XCheckBox btn) : base(btn) { ControlDefinition = btn; }

        new protected XCheckBox ControlDefinition { get; }

        protected override void TestControlInteraction(Control cntrl)
        {
            
            base.TestControlInteraction(cntrl);

            ControlDefinition.IsThreeState = false;
            ControlDefinition.IsChecked = true;
            Assert.IsTrue(ControlDefinition.NodeValueSetter.Value);
            Assert.AreEqual("1", ControlDefinition.NodeValueSetter.XValueProvider.Value);
            ControlDefinition.IsChecked = false;
            Assert.IsFalse(ControlDefinition.NodeValueSetter.Value);
            Assert.AreEqual("0", ControlDefinition.NodeValueSetter.XValueProvider.Value);
            
            ControlDefinition.IsThreeState = true;
            Assert.IsTrue(ControlDefinition.NodeValueSetter.IsThreeState);
            ControlDefinition.IsChecked = null;
            Assert.IsNull(ControlDefinition.NodeValueSetter.Value);

            ControlDefinition.NodeValueSetter.XValueProvider.Value = "1";
            Assert.IsTrue(ControlDefinition.NodeValueSetter.Value);
            Assert.IsTrue(ControlDefinition.IsChecked);
            ControlDefinition.NodeValueSetter.XValueProvider.Value = "0";
            Assert.IsFalse(ControlDefinition.NodeValueSetter.Value);
            Assert.IsFalse(ControlDefinition.IsChecked);
        }
    }
}