using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using RFBCodeWorks.Mvvm.XmlLinq;
using RFBCodeWorks.Mvvm.XmlLinq.ValueSetters;
using RFBCodeWorks.Mvvm.XmlLinq.Controls;

namespace ExampleWPF.Models
{
    class XmlLinqModel : RFBCodeWorks.Mvvm.ViewModelBase
    {
        public XmlLinqModel()
        {
            var root = new System.Xml.Linq.XElement("Root");
            XDoc = new XDocumentWrapper(new System.Xml.Linq.XDocument(root));
            StringNode = new XElementProvider("StringNode", XDoc);
            IntNode = new XElementProvider("IntNode", XDoc);
            root.Add(StringNode, IntNode);

            NumericBox = new(IntNode);
            Textbox = new XTextBox(StringNode);
        }

        public XDocumentWrapper XDoc { get; }
        public readonly XElementProvider StringNode;
        public readonly XElementProvider IntNode;

        public XNumericBox NumericBox { get; }
        public XTextBox Textbox { get; }
    }
}
