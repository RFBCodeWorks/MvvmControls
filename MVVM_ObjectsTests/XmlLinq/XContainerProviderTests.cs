using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.MVVMObjects.XmlLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RFBCodeWorks.MVVMObjects.XmlLinq.Tests
{
    [TestClass()]
    public class XContainerProviderTests
    {
        [TestMethod()]
        public void XContainerProviderTest()
        {
            var obj = new XContainerProvider();
            var iObj = obj as IXElementProvider;

            bool wasAdded = false;
            bool wasRemoved = false;
            bool descChanged = false;
            void AddedHandler(object sender, EventArgs e) => wasAdded = true;
            void RemovedHandler(object sender, EventArgs e) => wasRemoved= true;
            void DescHandler(object sender, EventArgs e) => descChanged= true;

            obj.Added += AddedHandler;
            obj.Removed += RemovedHandler;
            obj.DescendantChanged += DescHandler;

            Assert.IsNull(obj.XContainer);
            Assert.IsFalse(obj.IsNodeAvailable);
            Assert.IsFalse(iObj.CanBeCreated);
            Assert.IsFalse(iObj.IsNodeAvailable);

            XElement root = new("rootElement");
            XDocument doc = new XDocument(root);
            obj.XContainer = doc;

            //Test adding an XDocument
            Assert.AreSame(root, obj.ProvidedElement);
            Assert.IsTrue(obj.IsNodeAvailable);
            Assert.IsTrue(iObj.CanBeCreated);
            Assert.IsTrue(iObj.IsNodeAvailable);
            Assert.IsTrue(wasAdded, " -- Added event never fired!"); wasAdded = false;
            
            Assert.IsFalse(descChanged, " -- DescendantChanged fired unexpectedly!");
            root.Add(new XElement("AddTest1"));
            Assert.IsTrue(descChanged, " -- DescendantChanged event never fired!"); descChanged= false;

            //Remove the XDocument
            obj.XContainer = null;
            Assert.IsNull(obj.XContainer);
            Assert.IsFalse(obj.IsNodeAvailable);
            Assert.IsFalse(iObj.CanBeCreated);
            Assert.IsFalse(iObj.IsNodeAvailable);
            Assert.IsTrue(wasRemoved, " -- Removed event never fired!"); wasRemoved = false;
            Assert.IsFalse(descChanged);
            root.Add(new XElement("AddTest2"));
            Assert.IsFalse(descChanged, " -- DescendantChanged fired unexpectedly!");

            //Using an XElement
            obj.XContainer = root;
            Assert.IsTrue(obj.IsNodeAvailable);
            Assert.IsTrue(iObj.CanBeCreated);
            Assert.IsTrue(iObj.IsNodeAvailable);
            Assert.IsTrue(wasAdded, " -- Added event never fired!"); wasAdded = false;

            Assert.IsFalse(descChanged, " -- DescendantChanged fired unexpectedly!");
            root.Add(new XElement("AddTest3")); 
            Assert.IsTrue(descChanged, " -- DescendantChanged event never fired!"); descChanged = false;
            
            //Remove the XElement
            obj.XContainer = null;
            Assert.IsNull(obj.ProvidedElement);
            Assert.IsFalse(obj.IsNodeAvailable);
            Assert.IsFalse(iObj.CanBeCreated);
            Assert.IsFalse(iObj.IsNodeAvailable);
            Assert.IsTrue(wasRemoved, " -- Removed event never fired!"); wasRemoved = false;
            Assert.IsFalse(descChanged);

            root.Add(new XElement("AddTest4"));
            Assert.IsFalse(descChanged, " -- DescendantChanged fired unexpectedly!");
        }

        [TestMethod()]
        public void XContainerProviderTest1()
        {
            var obj = new XContainerProvider();
            Assert.IsFalse(obj.IsNodeAvailable);

            obj = new XContainerProvider(new XDocument());
            Assert.IsFalse(obj.IsNodeAvailable);

            obj = new XContainerProvider(new XDocument(new XElement("RootNode")));
            Assert.IsTrue(obj.IsNodeAvailable);

            obj = new XContainerProvider(new XElement("RootNode"));
            Assert.IsTrue(obj.IsNodeAvailable);
        }

        [TestMethod()]
        public void CreateXElementTest()
        {
            XElement root = new("rootElement");
            XDocument doc = new XDocument(root);
            var obj = new XContainerProvider(doc);

            var node2 = obj.CreateChildProvider("level2");
            var node3 = obj.CreateChildProvider("level3");
            var node4 = obj.CreateChildProvider("level4");
            Assert.IsTrue(node4.CanBeCreated);
            node4.Value = "Test";
            Assert.IsTrue(node4.IsNodeAvailable);

        }

        [TestMethod()]
        public void RemoveTest()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                var obj = new XContainerProvider(new XDocument(new XElement("root")));
                obj.Remove();
            });
        }
    }
}