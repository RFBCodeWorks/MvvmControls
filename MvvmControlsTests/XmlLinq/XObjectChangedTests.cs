using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml.Linq;

namespace RFBCodeWorks.MvvmControls.XmlLinq.Tests
{
    /// <summary>
    /// ShowCase how the 'Changed' event propogates onto an XElement object
    /// </summary>
    [TestClass()]
    public class XObjectChangedTests
    {

        private static XElement getElement()
        {
            var x = new XElement("TestElement");
            x.Changed += (o, e) =>
            {
                Console.WriteLine(
                    $"\nSender Type: {o.GetType()}" +
                    $"\nEventArgs: {e.ObjectChange}" +
                    $"\n Sender: {o}");
            };
            return x;
        }

        [TestMethod()]
        public void TestAddChildElement()
        {
            var x = getElement();
            x.Add(new XElement("Child_1"));
            Console.WriteLine("\n--------------------------  Remove the first child  ------------------------------");
            x.Element("Child_1").Remove();
        }

        [TestMethod()]
        public void TestAddChildElements()
        {
            var x = getElement();
            x.Add(new XElement("Child_1"), new XElement("Child_2"), new XElement("Child_3"));
            Console.WriteLine("\n--------------------------  Remove the first child  ------------------------------");
            x.Element("Child_1").Remove();
        }

        [TestMethod()]
        public void TestAttribute()
        {
            var x = getElement();
            string attrName = "Attr";
            var a = new XAttribute(attrName, "TestAttr");
            bool aRaised = false;
            a.Changed += (o, e) => aRaised = true;
            Console.WriteLine("--------------------------------------------------------\nAdding an XAttribute");
            x.ReplaceAttributes(a);
            Assert.IsFalse(aRaised);
            Console.WriteLine("--------------------------------------------------------\nReplacing Value");
            x.SetAttributeValue(attrName, "NewValue2");
            Console.WriteLine("--------------------------------------------------------\nSetting Empty Value");
            x.SetAttributeValue(attrName, string.Empty);
            Console.WriteLine("--------------------------------------------------------\nRemoving an XAttribute");
            x.SetAttributeValue(attrName, null);
        }

        [TestMethod()]
        public void TestSetValue()
        {
            var x = getElement();
            Console.WriteLine("Setting XElement.Value to a string value\n--------------------------------------------------------\n");
            x.Value = "SetValue";
            Console.WriteLine("--------------------------------------------------------\n Replacing the value\n");
            x.Value = "ReplaceValue";
            Console.WriteLine("--------------------------------------------------------\n Removing the value\n");
            x.Value = "";
        }
        [TestMethod()]
        public void MicrosoftExample_TextNodes()
        {
            XElement xmlTree = new XElement("Root", "Content");

            Console.WriteLine("# of text nodes: " + xmlTree.Nodes().OfType<XText>().Count());
            Console.WriteLine(xmlTree);
            Console.WriteLine("");
            Console.WriteLine("");

            // this doesn't add a new text node
            xmlTree.Add("new content");
            Console.WriteLine("# of text nodes: " + xmlTree.Nodes().OfType<XText>().Count());
            Console.WriteLine(xmlTree);
            Console.WriteLine("");
            Console.WriteLine("");

            // this does add a new, adjacent text node
            xmlTree.Add(new XText("more text"));
            Console.WriteLine("# of text nodes: " + xmlTree.Nodes().OfType<XText>().Count());
            Console.WriteLine(xmlTree);
        }
    }
}
