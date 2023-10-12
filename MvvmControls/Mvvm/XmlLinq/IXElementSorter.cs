using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RFBCodeWorks.Mvvm.XmlLinq
{
    /// <summary>
    /// Interface that provides methods for sorting / adding children in a specified manner to an XElement
    /// </summary>
    public interface IXElementSorter : IComparer<XElement>, IComparer<XAttribute>
    {
        /// <summary>
        /// Add a child node to the <paramref name="parent"/> XElement.
        /// </summary>
        /// <param name="parent">The parent element to which the child will be added.</param>
        /// <param name="childToAdd">The XElement to add to the tree.</param>
        void AddChild(XElement parent, XElement childToAdd);

        /// <param name="parent">The element to add the value to.</param>
        /// <inheritdoc cref="XElement.SetAttributeValue(XName, object)"/>
        /// <param name="name"/><param name="value"/>
        void SetAttributeValue(XElement parent, XName name, string value);

        /// <summary>
        /// Sort the child nodes
        /// </summary>
        /// <param name="parent">The element whose children should be sorted.</param>
        void SortChildren(XElement parent);

        /// <summary>
        /// Sort the attributes of the specified element
        /// </summary>
        /// <param name="parent">The element whose children should be sorted.</param>
        void SortAttributes(XElement parent);
    }

    /// <summary>
    /// Class with virtual methods that implements <see cref="IXElementSorter"/>
    /// </summary>
    public class XElementSorter : IXElementSorter
    {
        /// <summary>
        /// The default <see cref="XElementSorter"/> implementation
        /// </summary>
        public static XElementSorter DefaultSorter { get; } = new XElementSorter();

        /// <inheritdoc/>
        public virtual void SetAttributeValue(XElement parent, XName name, string value)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));
            parent.SetAttributeValue(name, value);
        }

        /// <remarks>If not overridden, adds to the bottom of the tree via <see cref="XContainer.Add(object?)"/></remarks>
        /// <inheritdoc/>
        public virtual void AddChild(XElement parent, XElement childToAdd)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));
            parent.Add(childToAdd);
        }

        /// <summary>
        /// Determine the order which the elements should appear within the tree.
        /// <br/> Base functionality sorts alphabetically by <see cref="XElement.Name"/>
        /// </summary>
        /// <returns>
        /// -1 : When <paramref name="x"/> should appear before <paramref name="y"/> <br/>
        ///  0 : When the order doesn't matter (no change) <br/>
        ///  1 : When <paramref name="x"/> should appear after <paramref name="y"/> <br/>
        /// </returns>
        /// <inheritdoc/>
        public virtual int Compare(XElement x, XElement y)
        {
            return x?.Name?.LocalName?.CompareTo(y?.Name?.LocalName) ?? (y is null ? 0 : 1);
        }

        /// <summary>
        /// Determine the order which the attributes should appear within the parent node.
        /// <br/> Base functionality sorts alphabetically by <see cref="XAttribute.Name"/>
        /// </summary>
        /// <returns>
        /// -1 : When <paramref name="x"/> should appear before <paramref name="y"/> <br/>
        ///  0 : When the order doesn't matter (no change) <br/>
        ///  1 : When <paramref name="x"/> should appear after <paramref name="y"/> <br/>
        /// </returns>
        /// <inheritdoc/>
        public virtual int Compare(XAttribute x, XAttribute y)
        {
            return x?.Name?.LocalName?.CompareTo(y?.Name?.LocalName) ?? (y is null ? 0 : 1);
        }

        /// <summary>
        /// Sort the child nodes.
        /// </summary>
        /// <remarks>
        /// Base functionality:
        /// <br/> - Removes all child XElements via <see cref="XContainer.RemoveNodes"/>
        /// <br/> - Sorts them via <see cref="SortElements(IEnumerable{XElement})"/>
        /// <br/> - Re-adds children to <paramref name="parent"/>
        /// </remarks>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public virtual void SortChildren(XElement parent)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));
            if (parent.Elements().Any())
            {
                XElement[] children = SortElements(parent.Elements());
                parent.RemoveNodes();
                parent.Add(children);
            }
        }

        /// <summary>
        /// Sort the elements via <see cref="Compare(XElement, XElement)"/>
        /// </summary>
        /// <returns>The sorted elements.</returns>
        public virtual XElement[] SortElements(IEnumerable<XElement> elements)
        {
            if (!elements.Any()) return Array.Empty<XElement>();
            List<XElement> nodes = elements.ToList();
            nodes.Sort(this);
            return nodes.ToArray();
        }


        /// <summary>
        /// Sort the child nodes.
        /// </summary>
        /// <remarks>
        /// Base functionality:
        /// <br/> - Removes all child XElements via <see cref="XElement.RemoveAttributes"/>
        /// <br/> - Sorts them via <see cref="SortAttributes(IEnumerable{XAttribute})"/>
        /// <br/> - Re-adds the attributes to <paramref name="parent"/>
        /// </remarks>
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"/>
        public virtual void SortAttributes(XElement parent)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));
            if (parent.Attributes().Any())
            {
                XAttribute[] attributes = SortAttributes(parent.Attributes());
                parent.RemoveAttributes();
                parent.Add(attributes);
            }
        }

        /// <summary>
        /// Sort the XAttributes via <see cref="Compare(XAttribute, XAttribute)"/>
        /// </summary>
        /// <returns>The sorted XAttributes.</returns>
        public virtual XAttribute[] SortAttributes(IEnumerable<XAttribute> attributes)
        {
            if (!attributes.Any()) return Array.Empty<XAttribute>();
            List<XAttribute> nodes = attributes.ToList();
            nodes.Sort(this);
            return nodes.ToArray();
        }
    }
}
