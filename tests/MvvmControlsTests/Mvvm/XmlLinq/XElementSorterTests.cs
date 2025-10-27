using RFBCodeWorks.Mvvm.XmlLinq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RFBCodeWorks.Mvvm.XmlLinq.Tests
{
    [TestClass()]
    public class XElementSorterTests
    {
        [TestMethod()]
        public void SetAttributeValueTest()
        {
            var root = new XContainerProvider(new XElement("root"));
            var attr = new XAttributeRetriever("TestValue", root);
            var attr2 = new XAttributeRetriever("AlphaValue", root);
            root.ChildSorter = new AlphaNumericSorter();
            attr.Value = "SomeValue";
            Assert.IsTrue(attr.IsNodeAvailable);

            attr2.Value = "SomeValue";
            Assert.IsTrue(attr2.IsNodeAvailable);
            Assert.IsTrue(root.ProvidedElement.FirstAttribute == attr2.XAttribute);
        }

        [TestMethod()]
        public void AddChildTest()
        {
            var root = new XContainerProvider(new XElement("root"));
            var el = new XElementProvider("BetaNode", root);
            var el2 = new XElementProvider("AlphaNode", root);
            root.ChildSorter = new AlphaNumericSorter();

            _ = el.CreateXElement();
            Assert.IsTrue(el.IsNodeAvailable, "BetaNode was not added.");

            _ = el2.CreateXElement();
            Assert.IsTrue(el2.IsNodeAvailable, "AlphaNode was not added.");
            Assert.IsTrue(root.ProvidedElement.FirstNode == el2.XElement, "Element 2 is not the first element.");

            var el2_1 = new XElementProvider("Child_2_1", el2);
            var el2_1_1 = new XElementProvider("Child_2_1_1", el2_1);
            var el2_2 = new XElementProvider("Child_2_2", el2);

            _ = el2_2.CreateXElement();
            _ = el2_1_1.CreateXElement();
            Assert.IsTrue(el2.XElement.FirstNode == el2_1.XElement, "Child Nodes were not sorted");
        }

        [TestMethod()]
        public void SortChildTest()
        {
            var doc = new XDocumentWrapper(new XElement("RootNode"));
            doc.Root.Add(
                new XElement("B"),
                new XElement("C"),
                new XElement("Z"),
                new XElement("X"),
                new XElement("A"),
                new XElement("D")
                );

            doc.ChildSorter.SortChildren(doc.Root);

            Assert.IsTrue(doc.Root.FirstNode.GetName() == "A");
            Assert.IsTrue(doc.Root.LastNode.GetName() == "Z");
        }


        class AlphaNumericSorter : XElementSorter
        {
            public override void SetAttributeValue(XElement parent, XName name, string value)
            {
                base.SetAttributeValue(parent, name, value);
                SortAttributes(parent);
            }

            public override void AddChild(XElement parent, XElement childToAdd)
            {
                base.AddChild(parent, childToAdd);
                SortChildren(parent);
            }
        }
    }
}
