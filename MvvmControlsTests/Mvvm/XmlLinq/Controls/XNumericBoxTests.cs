using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.XmlLinq.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RFBCodeWorks.Mvvm.XmlLinq.Controls.Tests
{
    [TestClass()]
    public class XNumericBoxTests
    {
        private XElementProvider GetXElement()
        {
            XDocumentWrapper doc = new XDocumentWrapper(new XElement("RootElement"));
            XElementProvider ParentProvider = new XElementProvider("TestParent", doc);
            var node = new XElementProvider("ValueNode", ParentProvider);
            node.Value = "0";
            return node;
        }

        [TestMethod()]
        public void TestControlDefinition()
        {
            Assert.IsNotNull(new XNumericBox(GetXElement()) as IControlDefinition);
        }

        [TestMethod()]
        public void TestValueUpdate()
        {
            var node = GetXElement();
            var box = new XNumericBox(node)
            {
                Minimum = -10,
                Maximum = 10,
                SmallChange = 1,
                LargeChange = 5
            };

            bool test = false;
            int assertNum = 0;
            box.PropertyChanged += Box_PropertyChanged;

            //Check updating from the node
            Assert.AreEqual(0, box.Value);
            node.Value = "10";
            AssertPropertyChanged(10);

            node.Value = "-10";
            AssertPropertyChanged(-10);

            //Check updating from the box
            box.Value = 0;
            Assert.AreEqual("0", node.Value);
            AssertPropertyChanged(0);
            
            box.Value = 9;
            Assert.AreEqual("9", node.Value);
            AssertPropertyChanged(9);
            
            box.Value = -9;
            Assert.AreEqual("-9", node.Value);
            AssertPropertyChanged(-9);

            //Check value outside range
            
            box.Value = 20;
            Assert.AreEqual("-9", node.Value); // Value does not update, since its outside the range
            Assert.AreEqual(-9, box.Value);
            Assert.IsFalse(test);
            //AssertPropertyChanged(7);


            void Box_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                test = true;
            }

            void AssertPropertyChanged(int value)
            {
                assertNum++;
                Assert.AreEqual(value, box.Value);
                Assert.IsTrue(test, $"\n-- OnPropertyChanged did not fire (UI will not update to reflect unchanged value) -- expected value: {value}  | Assert# {assertNum}");
                test = false;
            }
        }

        
    }
}