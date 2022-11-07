﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.MvvmControls.XmlLinq;
using RFBCodeWorks.MvvmControls.XmlLinq.ValueSetters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace RFBCodeWorks.MvvmControls.XmlLinq.Tests
{
    [TestClass()]
    public class XmlStringSetterTests
    {
        [TestMethod()]
        public void TestXElementProdiver()
        {
            XDocumentWrapper doc = new XDocumentWrapper(new XElement("RootElement"));
            XElementProvider ParentProvider = new XElementProvider("TestParent", doc);
            TextIXValueProvider(new XElementProvider("TestElement", ParentProvider), false, false);
        }

        [TestMethod()]
        public void TestXAttributeProvider()
        {
            XDocumentWrapper doc = new XDocumentWrapper(new XElement("RootElement"));
            XElementProvider ParentProvider = new XElementProvider("TestParent", doc);
            TextIXValueProvider(new XElementProvider("TestElement", ParentProvider), false, true);
        }

        [TestMethod()]
        public void TestXElementWrapper()
        {
            XDocumentWrapper doc = new XDocumentWrapper(new XElement("RootElement"));
            XElementProvider ParentProvider = new XElementProvider("TestParent", doc);
            var el = new XElementWrapper("TestElement", ParentProvider);
            ParentProvider.CreateXElement().Add(el);
            TextIXValueProvider(el, true, false);
        }

        //[TestMethod()]
        //public void TestXAttributeWrapper()
        //{
        //    XDocumentWrapper doc = new XDocumentWrapper(new XElement("RootElement"));
        //    XElementProvider ParentProvider = new XElementProvider("TestParent", doc);
        //    var el = new XAttributeWrapper("TestElement", ParentProvider);
        //    ParentProvider.XElement.Add(el);
        //    TextIXValueProvider(el, true,true);
        //}

        private void TextIXValueProvider(IXValueObject provider, bool isWrapper, bool isAttribute)
        {
            var valueObj = new XStringSetter(provider);
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
                var val = "true";
                valueObj.Value = val;
                AssertNodeExists(true);
                Assert.AreEqual(val, valueObj.Value);
            }
            void setValue2()
            {
                var val = "false";
                valueObj.Value = val;
                AssertNodeExists(true);
                Assert.AreEqual(val, valueObj.Value);
            }

            AssertNodeExists(isWrapper);
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