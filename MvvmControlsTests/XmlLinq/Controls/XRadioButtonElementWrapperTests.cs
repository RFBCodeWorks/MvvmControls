using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.MvvmControls.XmlLinq.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFBCodeWorks.MvvmControls.Tests;
using System.Windows.Controls;

namespace RFBCodeWorks.MvvmControls.XmlLinq.Controls.Tests
{
    [TestClass()]
    public class XRadioButtonElementProviderTests : XRadioButtonElementWrapperTests
    {

        public XRadioButtonElementProviderTests() : base(new XRadioButton(new XElementProvider("TestAttr", new XElementWrapper("Parent")), BooleanConverter.StoreAsBinary)) { }
    }


    [TestClass()]
    public class XRadioButtonAttributeTests : XRadioButtonElementWrapperTests
    {

        public XRadioButtonAttributeTests() : base(new XRadioButton(new XAttributeRetriever("TestAttr", new XElementWrapper("Parent")), BooleanConverter.StoreAsBinary)) { }
    }

    [TestClass()]
    public class XRadioButtonElementWrapperTests : RadioButtonDefinitionTest
    {
        public XRadioButtonElementWrapperTests(): this(new XRadioButton(new XElementWrapper("TestElement"), BooleanConverter.StoreAsBinary)) { }
        protected XRadioButtonElementWrapperTests(XRadioButton btn) : base(btn) { ControlDefinition = btn; }

        new protected XRadioButton ControlDefinition { get; }

        protected override void TestControlInteraction(Control cntrl)
        {
            base.TestControlInteraction(cntrl);

            ControlDefinition.IsChecked = true;
            Assert.IsTrue(ControlDefinition.NodeValueSetter.Value);
            Assert.AreEqual("1", ControlDefinition.NodeValueSetter.XValueProvider.Value);
            ControlDefinition.IsChecked = false;
            Assert.IsFalse(ControlDefinition.NodeValueSetter.Value);
            Assert.AreEqual("0", ControlDefinition.NodeValueSetter.XValueProvider.Value);
        }
    }
}