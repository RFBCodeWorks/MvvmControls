using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace RFBCodeWorks.MVVMObjects.XmlLinq.Tests
{

    [TestClass()]
    public class XProviderTests
    {
        internal IXElementProvider GetXElementProvider(IXElementProvider parent) => new XElementProvider(XWrapperTests.XElementName, parent);
        internal IXAttributeProvider GetXAttributeProvider(IXElementProvider parent) => new XAttributeRetriever(XWrapperTests.AttributeName, parent);

        [TestMethod()]
        public void TestIXElementProvider()
        {
            var doc = XWrapperTests.GetXDoc();
            var element = GetXElementProvider(doc);
            var attr = GetXAttributeProvider(element);
            var element2 = GetXElementProvider(element);
            var attr2 = GetXAttributeProvider(element2);

            Assert.IsNotNull(((IXElementProvider)doc).XObject);
            Assert.IsNull(element.XObject);
            Assert.IsNull(attr.XObject);

            Console.WriteLine("\n------------------------------------------- With Element");
            doc.Root.Add(XWrapperTests.GetXElement);
            Assert.IsNotNull(element.XObject);
            Console.WriteLine(doc.ToString());

            element.XObject.Remove();
            Assert.IsNull(element.XObject);

            element.CreateXElement();
            Assert.IsNotNull(element.XObject);

            Console.WriteLine("\n------------------------------------------- With Attribute");
            Assert.IsNull(attr.XObject);
            attr.Value = "true";
            Assert.IsNotNull(attr.XObject);
            Assert.IsTrue(attr.Value == "true");
            Console.WriteLine(doc.ToString());

            Console.WriteLine("\n------------------------------------------- Remove Node");
            element.XObject.Remove();
            Assert.IsNull(element.XObject);
            Assert.IsNull(attr.XObject);
            Console.WriteLine(doc.ToString());

            Console.WriteLine("\n------------------------------------------- Create Element 2");
            element2.CreateXElement();
            Assert.IsNotNull(element2.XObject);
            Console.WriteLine(doc.ToString());

            Console.WriteLine("\n------------------------------------------- Remove Element Tree");
            element.CreateXElement().Remove();
            Assert.IsNull(element2.XObject);
            Console.WriteLine(doc.ToString());

            Console.WriteLine("\n------------------------------------------- Attempt to write attribute 2 without element");
            attr2.Value = "test";
            Assert.IsNull(attr.XObject);
            ((XAttributeRetriever)attr2).CreateParentIfMissing = true;
            attr2.Value = "test";
            Assert.IsNotNull(attr2.XObject);
            Console.WriteLine(doc.ToString());
        }


        



        
        /// <summary>
        /// ShowCase how the 'Changed' event propogates through an XML tree
        /// </summary>
        [TestMethod()]
        public void TestXObjectCHanged()
        {
            
            bool rootChanged = false, Raised_Root = false;
            bool LevelChange_1 = false, Raised_1 = false;
            bool LevelChange_2 = false, Raised_2 = false;
            bool LevelChange_3 = false, Raised_3 = false;
            bool LevelChange_4 = false, Raised_4 = false;
            bool AttrChanged = false, Raised_Attr = false;

            object Sender = null;
            void EvalSender(object sender, EventArgs e)
            {
                Sender = sender;
                Console.WriteLine($"Sender Node: {((XObject)sender)}");
            }

            var Root = new XElement("Root");
            var Level1 = new XElement("Level1");
            var Level2 = new XElement("Level2");
            var Level3 = new XElement("Level3");
            var Level4 = new XElement("Level4");
            var Level3Attr = new XAttribute("Attr", "false");

            void rootChange(object sender, EventArgs e) { rootChanged = sender == Root; Raised_Root = true; }
            void level1Change(object sender, EventArgs e) {LevelChange_1 = sender == Level1; Raised_1 = true; }
            void level2Change(object sender, EventArgs e) {LevelChange_2 = sender == Level2; Raised_2 = true; }
            void level3Change(object sender, EventArgs e) {LevelChange_3 = sender == Level3; Raised_3 = true; }
            void level4Change(object sender, EventArgs e) {LevelChange_4 = sender == Level4; Raised_4 = true; }
            void AttrChange(object sender, EventArgs e) { AttrChanged = sender == Level3Attr; Raised_Attr = true; }

            void reset()
            {
                Raised_1 = false;
                Raised_2 = false;
                Raised_3 = false;
                Raised_4 = false;
                Raised_Attr = false;
            }

            Root.Changed += EvalSender;
            Root.Changed += rootChange;
            Level1.Changed += level1Change;
            Level2.Changed += level2Change;
            Level3.Changed += level3Change;
            Level4.Changed += level4Change;
            Level3Attr.Changed += AttrChange;

            //When adding a node, the sender is the node that is added, but the added node itself does not raise the event, the node's parent does.
            Console.WriteLine("Adding Level 1 to root");
            reset();
            Root.Add(Level1);
            Assert.IsFalse(rootChanged);
            Assert.IsFalse(LevelChange_1);
            Assert.AreEqual(Level1, Sender);

            Console.WriteLine("Adding Level 2");
            reset();
            Level1.Add(Level2);
            Assert.IsFalse(rootChanged);
            Assert.IsFalse(LevelChange_1);
            Assert.IsFalse(LevelChange_2);
            Assert.AreEqual(Level2, Sender);

            Console.WriteLine("Adding Level 3");
            reset();
            Level2.Add(Level3);
            Assert.IsFalse(rootChanged);
            Assert.IsFalse(LevelChange_1);
            Assert.IsFalse(LevelChange_2);
            Assert.IsFalse(LevelChange_3);
            Assert.AreEqual(Level3, Sender);

            Console.WriteLine("Adding Level 4");
            reset();
            Level3.Add(Level4);
            Assert.IsFalse(rootChanged);
            Assert.IsFalse(LevelChange_1);
            Assert.IsFalse(LevelChange_2);
            Assert.IsFalse(LevelChange_3);
            Assert.IsFalse(LevelChange_4);
            Assert.AreEqual(Level4, Sender);

            //when adding an attribute, the attribute node is the sender, but event is raised by its parent
            Console.WriteLine("Adding Level 3 attribute");
            reset();
            Level3.Add(Level3Attr);
            Assert.IsFalse(LevelChange_3);
            Assert.IsFalse(Raised_4);
            Assert.IsFalse(Raised_Attr);
            Assert.IsTrue(Raised_3);
            Assert.IsFalse(AttrChanged);
            Level3Attr.Value = "NEW VALUE";
            Assert.IsFalse(LevelChange_3);
            Assert.IsTrue(AttrChanged);
            Assert.IsFalse(Raised_4);
            Assert.IsTrue(Raised_Attr);
            Assert.IsTrue(Raised_3);

            //Removing an element - Sender is node that is removed (level 3), but only events on parents are raised
            Console.WriteLine("Removing Level3 node");
            reset();
            Level3.Remove();
            Assert.AreEqual(Level3, Sender);
            Assert.IsFalse(LevelChange_3);
            Assert.IsFalse(Raised_3);
            Assert.IsFalse(Raised_Attr);
            Assert.IsFalse(Raised_4);
            Assert.IsTrue(Raised_2);


            Console.WriteLine("");
            Console.WriteLine(Root);
        }
    }
}
