using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace RFBCodeWorks.MvvmControls.XmlLinq.Tests
{

    [TestClass()]
    public class XWrapperTests
    {
        internal const string XDocRootName = "XDocRoot";
        internal static XDocumentWrapper GetXDoc() => new XDocumentWrapper(new XElementWrapper(XDocRootName));

        internal const string XElementName = "TestElement";
        internal static XElementWrapper GetXElement => new XElementWrapper(XElementName);

        internal const string AttributeName = "TestAttribute";
        //internal static XAttributeWrapper GetXAttribute => new XAttributeWrapper(AttributeName, false);
        internal static XAttribute GetXAttribute => new XAttribute(AttributeName, false);



        [TestMethod()]
        public void TestWrappers()
        {
            bool XDocEvent = false;
            bool XElementEvent = false;
            bool XAttributeEvent = false;

            var doc = GetXDoc();
            doc.PropertyChanged += Doc_PropertyChanged;
            var xElement = GetXElement;
            xElement.PropertyChanged += XElement_PropertyChanged;
            var xAttr = GetXAttribute;
            //xAttr.PropertyChanged += XAttr_PropertyChanged;

            doc.Root.Add(xElement);
            Assert.IsTrue(XDocEvent); XDocEvent = false;
            
            xElement.Add(xAttr);
            Assert.IsTrue(XDocEvent); XDocEvent = false;
            Assert.IsFalse(XElementEvent); XElementEvent = false;

            xAttr.Value = "true";
            Assert.IsTrue(XDocEvent); XDocEvent = false;
            Assert.IsFalse(XElementEvent); XElementEvent = false;
            //Assert.IsTrue(XAttributeEvent); XAttributeEvent = false;

            void XAttr_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                XAttributeEvent = true;
            }

            void XElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                XElementEvent = true;
            }

            void Doc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                XDocEvent = true;
            }
        }

        private static XElementWrapper XElFunc => new XElementWrapper("ElementWrapper");
        //private static XAttributeWrapper XAttrFunc => new XAttributeWrapper("AttributeWrapper", "AttributeValue");


        [TestMethod()]
        [DataRow(data1: true, DisplayName = "Test Object: " + nameof(XElementWrapper))]
        //[DataRow(data1: false, DisplayName = "Test Object: " + nameof(XAttributeWrapper))]
        [DataRow(data1: null, DisplayName = "Test Object: " + nameof(XDocumentWrapper))]
        public void TestXElementWrapper(bool? isElement)
        {
            //Added and Removed are only fired by LINQ provider objects, and as such are not expected to fire here.
            //Descendant changed SHOULD fire for XElement.
            //ValueChanged should fire for both, and so should Changed.

            IXObjectProvider provider = isElement is null ? GetXDoc() : (bool)isElement ? (IXObjectProvider)XElFunc : null;// XAttrFunc;

            Assert.IsNotNull(provider);
            Assert.IsNotNull(provider.XObject);
            Assert.IsInstanceOfType(provider.GetName(), typeof(string));

            bool addedFired = false;
            bool removedFired = false;
            bool descendantFired = false;
            bool valueFired = false;

            void Provider_Added(object sender, EventArgs e) => addedFired = true;
            void Provider_Removed(object sender, EventArgs e) => removedFired = true;
            void Provider_DescendantChanged(object sender, EventArgs e) => descendantFired = true;
            void Provider_ValueChanged(object sender, EventArgs e) => valueFired = true;

            provider.Added += Provider_Added;
            provider.Removed += Provider_Removed;
            if (provider is IXElementProvider XProvider)
                XProvider.DescendantChanged += Provider_DescendantChanged;
            XDocument XDoc;
            XElement el = new XElement("Level1Element");
            XElement el2 = new XElement("Element2");
            if (isElement != null)
            {
                XDoc = new XDocument(new XElement("RootElement", el, new XElement("Level1Element")));
                el = XDoc.Root.Element("Level1Element");
            }else
            {
                XDoc = (XDocument)provider;
                XDoc.Root.Add(el);
            }


            if (isElement != null) el.Add(provider); else el.Add(el2);
            Assert.IsFalse(addedFired);
            Assert.IsFalse(removedFired);
            Assert.AreEqual(provider == XDoc, descendantFired);
            Assert.IsFalse(valueFired);

            if (isElement != null) provider.Remove(); else el2.Remove(); 
            Assert.IsFalse(addedFired);
            Assert.IsFalse(removedFired);
            Assert.AreEqual(provider == XDoc, descendantFired);
            Assert.IsFalse(valueFired);

            if (isElement != null)
            {
                //Indicate that the value has changed, but added/removed/descendant changed are not fired
                // XElement objects send their 'value' changes as 'XTEST sender, and 'Add/Remove' types
                var valueObj = (IXValueObject)provider;
                valueObj.ValueChanged += Provider_ValueChanged;
                valueObj.XObject.Changed += (o, e) => { Console.WriteLine($"\nValue Changed Test:\n Sender Type: {o.GetType()}\nEventArgs: {e.ObjectChange}"); };
                Console.WriteLine($"\nSetting value from: '{valueObj.Value}' to 'UpdateValue_1'");
                valueObj.Value = "UpdateValue_1";
                Console.WriteLine($"\nSetting value from: '{valueObj.Value}' to 'UpdateValue_2'");
                valueObj.Value = "UpdateValue_2";
                Assert.IsTrue(valueFired);
                Assert.IsFalse(addedFired);
                Assert.IsFalse(removedFired);
                Assert.IsFalse(descendantFired);
            }
            if (isElement ?? true)
            {
                var x = provider.XObject as XElement;
                x?.Add(new XElement("TestAdd"));
                Assert.IsTrue(descendantFired);
            }
            else
            {

            }


        }

        public void IXElementProvider()
        {
            var doc = GetXDoc() as IXElementProvider;
            
        }

        
    }
}
