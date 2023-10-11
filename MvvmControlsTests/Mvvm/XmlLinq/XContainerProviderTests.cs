using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace RFBCodeWorks.Mvvm.XmlLinq.Tests
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


        [TestMethod]
        public void LoadXDocumentTest()
        {
            // Write a document to disk
            XDocument doc = new XDocument(new XElement("DocRoot"));
            string group = nameof(group);
            string name = nameof(name);
            var x = new XElement(group); x.SetAttributeValue(name, "Element1");
            var y = new XElement(group); y.SetAttributeValue(name, "Element2");
            var z = new XElement(group); z.SetAttributeValue(name, "Element3");
            var val = new XElement("value"); val.Value = "100";
            doc.Root.Add(x);
            x.Add(y);
            y.Add(z);
            z.Add(val);
            string fp = System.IO.Path.Combine(AppContext.BaseDirectory, "TestXml.xml");
            doc.Save(fp);

            // Create providers
            XContainerProvider root = new XContainerProvider(new XDocument(new XElement("Sample")));
            XElementProvider el1 = root.CreateChildProvider(group, name, "Element1");
            XElementProvider el2 = el1.CreateChildProvider(group, name, "Element2");
            XElementProvider el3 = el2.CreateChildProvider(group, name, "Element3");

            // Test
            Assert.IsTrue(el3.CanBeCreated);
            Assert.IsInstanceOfType(el3.CreateXElement(), typeof(XElement));

            root.XContainer = null;
            Assert.IsFalse(el3.CanBeCreated, "Should not be able to be created when no XDocument is provided.");
            Assert.IsNull(el3.CreateXElement());

            root.XContainer = XDocument.Load(fp);
            Assert.IsTrue(el3.CanBeCreated, "Cannot be created after loading XDocument");
            Assert.IsInstanceOfType(el3.CreateXElement(), typeof(XElement));
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