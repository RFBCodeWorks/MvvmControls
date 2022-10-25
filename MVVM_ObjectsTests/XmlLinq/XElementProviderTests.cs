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
    public class XElementProviderTests
    {
        [TestMethod()]
        public void XElementProviderTest()
        {
            var p = new XElementWrapper("parent");
            Assert.IsNotNull(new XElementProvider("name", p));
            Assert.ThrowsException<ArgumentNullException>(() => new XElementProvider("name", null));
            Assert.ThrowsException<ArgumentException>(() => new XElementProvider("", p));
        }

        [TestMethod()]
        public void XElementProviderTest1()
        {
            var p = new XElementWrapper("parent");
            XElement GetElement(string s)=> new XElement(s);
            Assert.IsNotNull(new XElementProvider("name", p, GetElement));
            Assert.ThrowsException<ArgumentNullException>(() => new XElementProvider("name", null, GetElement));
            Assert.ThrowsException<ArgumentNullException>(() => new XElementProvider("name", p, null));
            Assert.ThrowsException<ArgumentException>(() => new XElementProvider("", p, null));
        }



        [TestMethod()]
        public void RefreshTest()
        {
            var p = new XElementWrapper("parent");
            var e = new XElementProvider("name", p);
            Assert.IsNotNull(p);
            Assert.IsNull(e.XElement);
            var n1 = new XElement(e.Name);
            p.Add(n1, new XElement(e.Name), new XElement(e.Name));
            Assert.IsNotNull(e.XElement);
            Assert.AreSame(n1, e.XElement);
            n1.Remove();
            Assert.AreNotSame(n1, e.XElement);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            var p = new XElementWrapper("parent");
            var e = new XElementProvider("name", p);
            Assert.IsNotNull(p);
            Assert.IsNull(e.XElement);
            e.CreateXElement();
            Assert.IsNotNull(e.XElement);
            e.Remove();
            Assert.IsNull(e.XElement);
        }

        [TestMethod()]
        public void CreateXElementTest()
        {
            var p = new XElementWrapper("parent");
            var e = new XElementProvider("name", p);
            Assert.IsNotNull(p);
            Assert.IsNull(e.XElement);
            e.CreateXElement();
            Assert.IsNotNull(e.XElement);
        }
    }
}