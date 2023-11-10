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
    public class XElementTreeViewItem : XNodeTreeViewItem<XElementWrapper>, ITreeViewItem<XNodeTreeViewItem>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public XElementTreeViewItem(XElement element)
            : this(new XElementWrapper(element ?? throw new ArgumentNullException(nameof(element))))
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xElementWrapper"></param>
        public XElementTreeViewItem(XElementWrapper xElementWrapper)
            : base(xElementWrapper ?? throw new ArgumentNullException(nameof(xElementWrapper)))
        {
            Item.DescendantChanged += Element_DescendantChanged;
            LazyCollection = GetChildren();
        }

        private Lazy<XNodeTreeViewItem[]> LazyCollection;

        /// <inheritdoc/>
        public IEnumerable<XNodeTreeViewItem> Children => LazyCollection.Value;

        /// <summary>
        /// Child Elements
        /// </summary>
        public IEnumerable<XElementTreeViewItem> Elements => Children.SelectWhere<XElementTreeViewItem>();

        /// <summary>
        /// Child Attributes
        /// </summary>
        public IEnumerable<XAttributeTreeViewItem> Attributes => Children.SelectWhere<XAttributeTreeViewItem>();

        /// <inheritdoc/>
        protected override IEnumerable<ITreeViewItem> ITreeViewChildren => Children;

        /// <inheritdoc/>
        public override string Name => Item.Name.LocalName;

        /// <summary>
        /// The string value of the XElement. If this item has child elements, returns String.Empty
        /// </summary>
        public string Value => Item.HasElements ? string.Empty : Item.Value ?? string.Empty;
        
        private void Element_DescendantChanged(object sender, EventArgs e)
        {
            LazyCollection = GetChildren();
            OnPropertyChanged(nameof(Children));
        }

        private Lazy<XNodeTreeViewItem[]> GetChildren()
        {
            if (LazyCollection?.IsValueCreated ?? false)
            {
                var oldCollection = LazyCollection.Value;
                return new Lazy<XNodeTreeViewItem[]>(Refresh);

                XNodeTreeViewItem[] Refresh()
                {
                    var oldElements = oldCollection.Where(x => x is XElementTreeViewItem).Select(x => (XElementTreeViewItem)x).ToArray();
                    var oldAttributes = oldCollection.Where(y => y is XAttributeTreeViewItem).Select(y => (XAttributeTreeViewItem)y).ToArray();
                    return ProcessElements(oldElements).Concat(ProcessAttributes(oldAttributes)).ToArray();
                }
            }
            else if (LazyCollection is null)
            {
                return new Lazy<XNodeTreeViewItem[]>(FirstRetrieval);

                XNodeTreeViewItem[] FirstRetrieval()
                {
                    IEnumerable<XNodeTreeViewItem> children = Item.Elements().Select(ProcessElement);
                    return children.Concat(Item.Attributes().Select(ProcessAttribute)).ToArray();
                }
            }
            return LazyCollection; // is not null and also have not been populated.
        }

        private IEnumerable<XNodeTreeViewItem> ProcessElements(IEnumerable<XElementTreeViewItem> oldElements)
        {
            List<XNodeTreeViewItem> elements = new List<XNodeTreeViewItem>();
            foreach (var x in Item.Elements())
            {
                if (oldElements.FirstOrDefault(o => o.Item.IsSameElement(x)) is XNodeTreeViewItem item)
                    elements.Add(item);
                else
                    elements.Add(ProcessElement(x));
            }
            return elements;
        }

        /// <summary>
        /// Process the discovered <paramref name="element"/> into a new <see cref="XNodeTreeViewItem"/> object
        /// </summary>
        /// <returns>A new <see cref="XElementTreeViewItem"/></returns>
        protected virtual XNodeTreeViewItem ProcessElement(XElement element)
        {
            XElementTreeViewItem child = new XElementTreeViewItem(element) { Parent = this };
            SubscribeToChild(child);
            return child;
        }

        private IEnumerable<XNodeTreeViewItem> ProcessAttributes(IEnumerable<XAttributeTreeViewItem> oldAttributes)
        {
            List<XNodeTreeViewItem> attributes = new List<XNodeTreeViewItem>();
            foreach (var x in Item.Attributes())
            {
                if (oldAttributes.FirstOrDefault(o => o.Item.Name == x.Name) is XNodeTreeViewItem item)
                    attributes.Add(item);
                else
                    attributes.Add(ProcessAttribute(x));
            }
            return attributes;
        }

        /// <summary>
        /// Process the discovered <paramref name="attribute"/> into a new <see cref="XNodeTreeViewItem"/> object
        /// </summary>
        /// <returns>A new <see cref="XAttributeTreeViewItem"/></returns>
        protected virtual XNodeTreeViewItem ProcessAttribute(XAttribute attribute)
        {
            XAttributeTreeViewItem child = new XAttributeTreeViewItem(attribute, Item) { Parent = this };
            SubscribeToChild(child);
            return child;
        }
    }
}
