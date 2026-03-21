using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;
using RFBCodeWorks.Mvvm.Tests;


namespace RFBCodeWorks.Mvvm.XmlLinq.Tests
{

    [TestClass]
    public class XWrapperTests
    {
        internal const string XDocRootName = "XDocRoot";
        internal static XDocumentWrapper GetXDoc() => new XDocumentWrapper(new XElementWrapper(XDocRootName));

        internal const string XElementName = "TestElement";
        internal static XElementWrapper GetXElement => new XElementWrapper(XElementName);

        internal const string AttributeName = "TestAttribute";
        //internal static XAttributeWrapper GetXAttribute => new XAttributeWrapper(AttributeName, false);
        internal static XAttribute GetXAttribute => new XAttribute(AttributeName, false);



        [TestMethod]
        public void ModelTest_Wrappers()
        {
            bool XDocEvent = false;
            bool XElementEvent = false;
            //bool XAttributeEvent = false;

            var doc = GetXDoc();
            doc.PropertyChanged += Doc_PropertyChanged;
            var xElement = GetXElement;
            xElement.PropertyChanged += XElement_PropertyChanged;
            var xAttr = GetXAttribute;
            //xAttr.PropertyChanged += XAttr_PropertyChanged;

            doc.Root!.Add(xElement);
            Assert.IsTrue(XDocEvent); XDocEvent = false;

            xElement.Add(xAttr);
            Assert.IsTrue(XDocEvent); XDocEvent = false;
            Assert.IsFalse(XElementEvent); XElementEvent = false;

            xAttr.Value = "true";
            Assert.IsTrue(XDocEvent); XDocEvent = false;
            Assert.IsFalse(XElementEvent); XElementEvent = false;

            void XElement_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                XElementEvent = true;
            }

            void Doc_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                XDocEvent = true;
            }
        }

        [TestMethod]
        public void XElementWrapperTest_XDocumentWrapper()
        {
            var doc = GetXDoc();
            doc.Root!.Add(new XElement(Level1Element));
            IXObjectProviderTest(doc, doc, new XElement("Test"));
        }

        [TestMethod]
        public void XElementWrapperTest_XElementWrapper()
        {
            var provider = new XElementWrapper(Level1Element);
            var xdoc = new XDocument(new XElement("RootElement", provider));

            IXObjectProviderTest(
                xDocument: xdoc,
                provider: provider,
                elementToAddOrRemove: new XElement("Test")
                );
        }

        const string Level1Element = nameof(Level1Element);
        const string Level2Element = nameof(Level2Element);

        /// <param name="provider">The provider to test</param>
        /// <param name="xDocument"></param>
        private static void IXObjectProviderTest(IXObjectProvider provider, XDocument xDocument, XElement elementToAddOrRemove)
        {
            //Added and Removed are only fired by LINQ provider objects, and as such are not expected to fire here.
            //Descendant changed SHOULD fire for XElement.
            //ValueChanged should fire for both, and so should Changed.

            #region < Test Setup >

            Assert.IsNotNull(provider?.XObject);
            Assert.IsNotNull(xDocument);
            Assert.IsNotNull(elementToAddOrRemove);

            bool raisedDescendantChanged = false;
            bool raisedValueChanged = false;

            void Reset()
            {
                raisedDescendantChanged = false;
                raisedValueChanged = false;
            }

            void Provider_DescendantChanged(object? sender, EventArgs e) => raisedDescendantChanged = true;
            void Provider_ValueChanged(object? sender, EventArgs e) => raisedValueChanged = true;

            if (provider is IXElementProvider xp) { xp.DescendantChanged += Provider_DescendantChanged; }
            if (provider is IXValueObject vp) { vp.ValueChanged += Provider_ValueChanged; }

            #endregion < Test Setup >

            XElement documentElement = xDocument.Root!.Element(Level1Element).AssertIsOfType<XElement>();

            // Test 1 - Add the new element to the provider document element
            try
            {
                Reset();
                documentElement.Add(elementToAddOrRemove);
                Assert.IsFalse(raisedValueChanged);
                Assert.IsTrue(raisedDescendantChanged);
            }
            catch (Exception e)
            {
                throw new AssertFailedException($"Failed Test 1 - (Adding sub-element)\n{e.Message}");
            }

            // Test 2 - Remove the new element
            try
            {
                Reset();
                elementToAddOrRemove.Remove();
                Assert.IsFalse(raisedValueChanged);
                Assert.IsTrue(raisedDescendantChanged);
            }
            catch (Exception e)
            {
                throw new AssertFailedException($"Failed Test 2 - (Removing sub-element)\n{e.Message}");
            }

            /// Test 3 - Updating the value
            /// This test does not apply to the <see cref="XDocumentWrapper">
            /// Only applies to the <see cref="XElementWrapper">
            if (provider is IXValueObject valueProvider)
            {
                //Indicate that the value has changed, but added/removed/descendant changed are not fired
                // XElement objects send their 'value' changes as 'XTEST sender, and 'Add/Remove' types
                try
                {
                    valueProvider.Value = "UpdateValue_1";
                    Reset();
                    valueProvider.Value = "UpdateValue_2";
                    Assert.IsTrue(raisedValueChanged);
                    Assert.IsFalse(raisedDescendantChanged);
                }
                catch (Exception e)
                {
                    throw new AssertFailedException($"Failed Test 3 - ({nameof(IXValueObject)}.{nameof(IXValueObject.ValueChanged)} Test)\n{e.Message}");
                }
            }
        }  // IXObjectProviderTest



    }
}
