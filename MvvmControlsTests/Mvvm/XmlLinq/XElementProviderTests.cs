﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace RFBCodeWorks.Mvvm.XmlLinq.Tests
{
    [TestClass()]
    public class XElementProviderTests
    {
        [TestMethod]
        public void XElementProviderTest()
        {
            var p = new XElementWrapper("parent");
            Assert.IsNotNull(new XElementProvider("name", p));
            Assert.ThrowsException<ArgumentNullException>(() => new XElementProvider("name", null));
            Assert.ThrowsException<ArgumentException>(() => new XElementProvider("", p));
            Assert.IsNotNull(new XElementProvider("name", p, null, null));
        }

        [TestMethod()]
        [DataRow(data1: true)]
        [DataRow(data1: false)]
        public void TreeCreationTest(bool discriminate)
        {
            var p = new XContainerProvider() ;
            IXElementProvider Create10(IXElementProvider root)
            {
                IXElementProvider prov = root; int i = 1;
                while (i <= 10)
                {
                    prov = new XElementProvider($"L{i}", prov) { DiscriminateDescendantChanged = discriminate };
                    i++;
                }
                return prov;
            }

            var lastNode = Create10(p);
            Assert.IsFalse(lastNode.CanBeCreated);
            p.XContainer = new XElement("RootNode");
            Assert.IsTrue(lastNode.CanBeCreated);

            
            bool wasAdded = false;
            bool descRaised = false;
            p.DescendantChanged += (o, e) => descRaised = true;
            lastNode.Added += (o, e) => wasAdded = true;

            Assert.IsNull(lastNode.XObject);
            ((IXValueObject)lastNode).Value = "SetValue!";
            Assert.IsTrue(descRaised);
            Assert.IsTrue(wasAdded);
            Assert.IsNotNull(lastNode.XObject);

            Console.WriteLine(p.XContainer);
        }

        [TestMethod()]
        [DataRow(data1: true)]
        [DataRow(data1: false)]
        public void RefreshTest(bool discriminate)
        {
            var p = new XElementWrapper("parent") { DiscriminateDescendantChanged = discriminate };
            var e = new XElementProvider("name", p) { DiscriminateDescendantChanged = discriminate };
            Assert.IsNotNull(p);
            Assert.IsNull(e.XElement);
            var n1 = new XElement(e.Name);
            var n2 = new XElement(e.Name);
            Assert.AreNotSame(n1, n2);
            p.Add(n1, n2, new XElement(e.Name));
            Assert.IsNotNull(e.XElement);
            Assert.AreSame(n1, e.XElement, "Expected the XElementProvider to provide n1.");
            n1.Remove();
            Assert.AreNotSame(n1, e.XElement, "Expected the XElementProvider to provide n2, since n1 was removed.");
        }

        [TestMethod()]
        [DataRow(data1: true)]
        [DataRow(data1: false)]
        public void RemoveTest(bool discriminate)
        {
            var p = new XElementWrapper("parent") { DiscriminateDescendantChanged = discriminate };
            var e = new XElementProvider("name", p) { DiscriminateDescendantChanged = discriminate };
            Assert.IsNotNull(p);
            Assert.IsNull(e.XElement);
            e.CreateXElement();
            Assert.IsNotNull(e.XElement);
            e.Remove();
            Assert.IsNull(e.XElement);
        }

        [TestMethod()]
        [DataRow(data1: true)]
        [DataRow(data1: false)]
        public void CreateXElementTest(bool discriminate)
        {
            var p = new XElementWrapper("parent") { DiscriminateDescendantChanged = discriminate };
            var e = new XElementProvider("name", p) { DiscriminateDescendantChanged = discriminate };
            Assert.IsNotNull(p);
            Assert.IsNull(e.XElement);
            e.CreateXElement();
            Assert.IsNotNull(e.XElement);
        }

        [TestMethod()]
        [DataRow(data1: true)]
        [DataRow(data1: false)]
        public void ValueSetTest(bool discriminate)
        {
            var p = new XElementWrapper("parent") { DiscriminateDescendantChanged = discriminate };
            var e = new XElementProvider("name", p) { DiscriminateDescendantChanged = discriminate };
            var v = new XElementProvider("ValueNode", e) { DiscriminateDescendantChanged = discriminate };
            Assert.IsNotNull(p);
            Assert.IsNull(e.XElement);
            Assert.IsNull(v.XElement);
            v.Value = "TestValue";
            Assert.AreEqual("TestValue", v.Value);
        }

        [TestMethod()]
        public void CanBeCreatedTest()
        {
            var p = new XContainerProvider();
            var e = new XElementProvider("name", p);
            var v = new XElementProvider("ValueNode", e);
            var t = new XElementProvider("TestNode", v);
            Assert.IsNotNull(p);
            Assert.IsNull(e.XElement);
            Assert.IsNull(v.XElement);
            Assert.IsNull(t.XElement);

            Assert.IsFalse(t.CanBeCreated);
            p.XContainer = new XElement("RootNode");
            Assert.IsTrue(t.CanBeCreated);
            e.CanBeCreated = false;
            Assert.IsFalse(t.CanBeCreated);
        }
    }
}