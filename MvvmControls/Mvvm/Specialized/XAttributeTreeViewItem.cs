using RFBCodeWorks.Mvvm.XmlLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RFBCodeWorks.Mvvm.Specialized
{
    /// <summary>
    /// 
    /// </summary>
    public class XAttributeTreeViewItem : XNodeTreeViewItem<XAttributeRetriever>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="parent"></param>
        public XAttributeTreeViewItem(XAttribute attribute, IXElementProvider parent) 
            : base(new XAttributeRetriever(attribute?.Name?.LocalName ?? throw new ArgumentNullException(nameof(attribute)), parent))
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="parent"></param>
        public XAttributeTreeViewItem(string attributeName, IXElementProvider parent)
        : base(new XAttributeRetriever(attributeName, parent))
        { }

        /// <inheritdoc/>
        public override string Name => Item.Name;

        /// <inheritdoc cref="XAttributeRetriever.Value"/>
        public string Value
        {
            get { return Item.Value; }
            set
            {
                Item.Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}
