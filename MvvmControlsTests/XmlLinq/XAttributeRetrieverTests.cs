using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.MvvmControls.XmlLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MvvmControls.XmlLinq.Tests
{
    [TestClass()]
    public class XAttributeRetrieverTests
    {
        [TestMethod()]
        public void XAttributeRetrieverTest()
        {
            var p = new XElementWrapper("parent");
            Assert.IsNotNull(new XAttributeRetriever("name", p));
            Assert.ThrowsException<ArgumentNullException>(() => new XElementProvider("name", null));
            Assert.ThrowsException<ArgumentException>(() => new XElementProvider("", p));
        }

        [TestMethod()]
        public void RefreshTest()
        {
            var p = new XElementWrapper("parent");
            var a = new XAttributeRetriever("name", p);
            Assert.IsNull(a.XAttribute);
            p.SetAttributeValue(a.Name, "true");
            Assert.IsNotNull(a.XAttribute);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            var p = new XElementWrapper("parent");
            var a = new XAttributeRetriever("name", p);
            p.SetAttributeValue(a.Name, "true");
            Assert.IsNotNull(a.XAttribute);
            a.Remove();
            Assert.IsNull(a.XAttribute);
        }

        [TestMethod()]
        public void ValueSetTest()
        {
            var p = new XElementWrapper("parent");
            var e = new XElementProvider("name", p);
            var v = new XAttributeRetriever("ValueNode", e);
            Assert.IsNotNull(p);
            Assert.IsNull(e.XElement);
            Assert.IsNull(v.XAttribute);
            v.Value = "TestValue";
            Assert.AreEqual("TestValue", v.Value);
        }

    }
}