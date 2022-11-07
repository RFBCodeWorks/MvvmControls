using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.MVVMObjects.XmlLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects.XmlLinq.Tests
{
    [TestClass()]
    public class IXValueObjectTests
    {
        [TestInitialize]
        public void TestInit() 
        {
            WasValueChanged = false;
        }

        private bool WasValueChanged = false;

        private void X_ValueChanged(object sender, EventArgs e)
        {
            WasValueChanged = true;
        }

        private void AssertValueChanged(bool expected)
        {
            Assert.AreEqual(expected, WasValueChanged, expected ? "ValueChanged Event was not raised!" : "ValueChanged Event was raised unexpectedly!");
            WasValueChanged = false;
        }

        private void RemoveNode(IXValueObject node)
        {
            bool isAvailable = node.IsNodeAvailable;
            node.Remove();
            Assert.IsFalse(node.IsNodeAvailable, "Node was expected to be removed");
            if (isAvailable)
            {
                AssertValueChanged(true);
            }
        }

        [TestMethod()]
        public void XAttributeRetrieverTest()
        {
            var p = new XElementWrapper("parent");
            var n = new XAttributeRetriever("Value", p);
            IXValueObject x = n;
            x.ValueChanged += X_ValueChanged;

            n.Value = "TestValue";
            AssertValueChanged(true);
            RemoveNode(n);
            
            Console.WriteLine("\n\n-------- NULL TEST - Do Not Create Value--------");
            x.Value = null;
            Console.WriteLine(p);
            Assert.IsFalse(n.IsNodeAvailable, "Node exists when not expected");
            AssertValueChanged(false);

            Console.WriteLine("\n\n-------- Empty Value TEST --------");
            x.Value = "";
            Console.WriteLine(p);
            Assert.IsTrue(n.IsNodeAvailable, "Failed to create the value node");
            AssertValueChanged(true);
            RemoveNode(n);

            Console.WriteLine("\n\n-------- Apply Value TEST --------");
            x.Value = "ApplyValue";
            Console.WriteLine(p);
            Assert.IsTrue(n.IsNodeAvailable, "Failed to create the value node");
            AssertValueChanged(true);

            Console.WriteLine("\n\n-------- NULL TEST --------");
            x.Value = null;
            Console.WriteLine(p);
            Assert.IsFalse(n.IsNodeAvailable, "Node exists when not expected");
            AssertValueChanged(true);
        }

        [TestMethod()]
        public void XElementProviderTest()
        {
            var p = new XElementWrapper("parent");
            var n = new XElementProvider("Value", p);
            IXValueObject x = n;
            x.ValueChanged += X_ValueChanged;

            n.Value = "TestValue";
            AssertValueChanged(true);
            RemoveNode(n);

            Console.WriteLine("\n\n-------- NULL TEST - Do Not Create Value--------");
            x.Value = null;
            Console.WriteLine(p);
            Assert.IsFalse(n.IsNodeAvailable, "Node exists when not expected");
            AssertValueChanged(false);

            Console.WriteLine("\n\n-------- Empty Value TEST --------");
            x.Value = "";
            Console.WriteLine(p);
            AssertValueChanged(true);
            Assert.IsTrue(n.IsNodeAvailable, "Failed to create the value node");
            Assert.IsFalse(n.XElement.Nodes().Any(), "ValueNode should not have child XText nodes");
            RemoveNode(n);

            Console.WriteLine("\n\n-------- Apply Value TEST --------");
            x.Value = "ApplyValue";
            Console.WriteLine(p);
            Assert.IsTrue(n.IsNodeAvailable, "Failed to create the value node");
            Assert.IsTrue(n.XElement.Nodes().Any(), "ValueNode should have child XText nodes");
            AssertValueChanged(true);
            

            Console.WriteLine("\n\n-------- NULL TEST --------");
            x.Value = null;
            Console.WriteLine(p);
            AssertValueChanged(true);
            Assert.IsTrue(n.IsNodeAvailable, "Value Node Unexpectedly Removed");
            Assert.IsFalse(n.XElement.Nodes().Any(), "ValueNode should not have child XText nodes");
            
        }

        [TestMethod()]
        public void XElementWrapperTest()
        {
            var n = new XElementWrapper("ValueNode");
            IXValueObject x = n;
            Console.WriteLine(n);
            x.ValueChanged += X_ValueChanged;

            //n.Value = "TestValue";
            //AssertValueChanged(true);
            //x.Remove();
            //Assert.IsFalse(x.IsNodeAvailable, "Node was expected to be removed");

            Console.WriteLine("\n\n-------- NULL TEST - Do Not Create Value--------");
            x.Value = null;
            Console.WriteLine(n);
            Assert.IsFalse(n.Nodes().Any(), "ValueNode should not have child XText nodes");
            AssertValueChanged(false);
            

            Console.WriteLine("\n\n-------- Empty Value TEST --------");
            x.Value = "";
            Console.WriteLine(n);
            Assert.IsFalse(n.Nodes().Any(), "ValueNode should not have child XText nodes");
            AssertValueChanged(true);

            Console.WriteLine("\n\n-------- Apply Value TEST --------");
            x.Value = "ApplyValue";
            Console.WriteLine(n);
            Assert.IsTrue(n.Nodes().Any(), "ValueNode should have child XText nodes");
            AssertValueChanged(true);
            
            Console.WriteLine("\n\n-------- NULL TEST --------");
            x.Value = null;
            Console.WriteLine(n);
            Assert.IsFalse(n.Nodes().Any(), "ValueNode should not have child XText nodes");
            AssertValueChanged(true);
        }

    }
}