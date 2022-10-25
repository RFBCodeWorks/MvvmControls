﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class XmlBooleanSetterTests
    {
        [TestMethod()]
        public void CreateBinarySetterTest()
        {
            var obj = XmlBooleanSetter.CreateBinarySetter(new XElementWrapper("test"));
            Assert.AreSame(obj.Converter, BooleanConverter.StoreAsBinary);

            obj.Value = true;
            Assert.AreEqual("1", obj.XValueProvider.Value);
            obj.Value = false;
            Assert.AreEqual("0", obj.XValueProvider.Value);

            obj.Toggle();
            Assert.IsTrue(obj.Value);
            Assert.AreEqual("1", obj.XValueProvider.Value);
            obj.Toggle();
            Assert.IsFalse(obj.Value);
            Assert.AreEqual("0", obj.XValueProvider.Value);
        }

        [TestMethod()]
        public void CreateStandardStringSetterTest()
        {
            var obj = XmlBooleanSetter.CreateStandardStringSetter(new XElementWrapper("test"));
            Assert.AreSame(obj.Converter, BooleanConverter.StoreAsString);

            obj.Value = true;
            Assert.AreEqual("true", obj.XValueProvider.Value);
            obj.Value = false;
            Assert.AreEqual("false", obj.XValueProvider.Value);

            obj.Toggle();
            Assert.IsTrue(obj.Value);
            Assert.AreEqual("true", obj.XValueProvider.Value);
            obj.Toggle();
            Assert.IsFalse(obj.Value);
            Assert.AreEqual("false", obj.XValueProvider.Value);
        }


        [TestMethod()]
        public void TestXElementProdiver()
        {
            XDocumentWrapper doc = new XDocumentWrapper(new XElement("RootElement"));
            XElementProvider ParentProvider = new XElementProvider("TestParent", doc);
            TextIXValueProvider(new XElementProvider("TestElement", ParentProvider), false);
        }

        [TestMethod()]
        public void TestXAttributeProvider()
        {
            XDocumentWrapper doc = new XDocumentWrapper(new XElement("RootElement"));
            XElementProvider ParentProvider = new XElementProvider("TestParent", doc);
            TextIXValueProvider(new XAttributeRetriever("TestAttribute", ParentProvider), false);
        }

        [TestMethod()]
        public void TestXElementWrapper()
        {
            XDocumentWrapper doc = new XDocumentWrapper(new XElement("RootElement"));
            XElementProvider ParentProvider = new XElementProvider("TestParent", doc);
            var el = new XElementWrapper("TestElement", ParentProvider);
            ParentProvider.CreateXElement().Add(el);
            TextIXValueProvider(el, true);
        }

        //[TestMethod()]
        //public void TestXAttributeWrapper()
        //{
        //    XDocumentWrapper doc = new XDocumentWrapper(new XElement("RootElement"));
        //    XElementProvider ParentProvider = new XElementProvider("TestParent", doc);
        //    var el = new XAttributeWrapper("TestElement", ParentProvider);
        //    ParentProvider.XElement.Add(el);
        //    TextIXValueProvider(el, true);
        //}

        private void TextIXValueProvider(IXValueObject provider, bool isWrapper)
        {
            var valueObj = XmlBooleanSetter.CreateBinarySetter(provider);
            bool isAttributeProvider = provider is IXAttributeProvider;
            void AssertNodeExists(bool shouldExist)
            {
                string errText() => !shouldExist ? "\nNode exists when it shouldn't" : "\nNode doesn't exist when it should";
                if (isAttributeProvider)
                {
                    Assert.AreEqual(shouldExist, provider.Parent?.XObject?.Attribute(provider.Name) != null, errText());
                }
                else
                {
                    Assert.AreEqual(shouldExist, provider.Parent?.XObject?.Element(provider.Name) != null, errText());
                }
            }
            void setValue1() 
            {
                var val = true;
                valueObj.Value = val;
                AssertNodeExists(true);
                Assert.AreEqual(val, valueObj.Value);
            }
            void setValue2()
            {
                var val = false;
                valueObj.Value = val;
                AssertNodeExists(true);
                Assert.AreEqual(val, valueObj.Value);
            }

            AssertNodeExists(isWrapper);
            Assert.IsFalse(valueObj.Value);
            provider.Parent.CreateXElement();

            if (provider is IXAttributeProvider attr)
            {
                provider.Parent.XObject.SetAttributeValue(attr.Name, true);
                setValue1();
                setValue2();
                provider.Remove();
                AssertNodeExists(false);
            }
            else if (provider is IXElementProvider el)
            {
                el.CreateXElement();
                setValue1();
                setValue2();
                provider.Remove();
                AssertNodeExists(false);
            }
        }
    }
}