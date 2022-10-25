using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RFBCodeWorks.MVVMObjects.XmlLinq
{
    /// <summary>
    /// Class that implements <see cref="IXElementProvider"/> <br/>
    /// Can be set up to use LINQ to get the first matching element from the parent IXElementProvider.
    /// </summary>
    /// <remarks>Explicitly implements <see cref="IXValueObject"/></remarks>
    public class XElementProvider : ObservableObject, IXElementProvider, IXValueObject, IXObjectProvider
    {
        /// <summary>
        /// Create a new XElementProvider that will get an XElement that is a descendant of the XElement provided by the <paramref name="parent"/>
        /// </summary>
        /// <param name="elementName">The name of the element to search for</param>
        /// <param name="parent">The object that provides the parent XElement to this object's XElement</param>
        /// <param name="getElement">
        /// A function to get an XElement from the <paramref name="parent"/> <br/>
        /// If null, it will get the first element from the parent that has a matching <paramref name="elementName"/>
        /// </param>
        public XElementProvider(string elementName, IXElementProvider parent, Func<string, XElement> getElement)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            SearchName = elementName.IsNotEmpty() ? elementName.Trim() : throw new ArgumentException("elementName is Empty!");
            GetElementFunc = getElement ?? throw new ArgumentNullException(nameof(getElement));

            Parent.DescendantChanged += Parent_DescendantChanged;
            Parent.Added += Parent_Added;
            Parent.Removed += Parent_Removed;
            Refresh();
        }

        /// <summary>
        /// Create a new XElementProvider that will get the first child with a matching <paramref name="elementName"/> from the <paramref name="parent"/>
        /// </summary>
        /// <inheritdoc cref="XElementProvider.XElementProvider(string, IXElementProvider, Func{string, XElement})"/>
        public XElementProvider(string elementName, IXElementProvider parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            SearchName = elementName.IsNotEmpty() ? elementName.Trim() : throw new ArgumentException("elementName is Empty!");
            GetElementFunc = GetElementFromParent;

            Parent.DescendantChanged += Parent_DescendantChanged;
            Parent.Added += Parent_Added;
            Parent.Removed += Parent_Removed;
            Refresh();
        }

        private XElement XElementField;
        private readonly string SearchName;
        private readonly Func<string, XElement> GetElementFunc;
        private XElement ParentElement => Parent.XObject;

        /// <inheritdoc/>
        public IXElementProvider Parent { get; }

        /// <inheritdoc/>
        public event EventHandler ValueChanged;
        /// <inheritdoc/>
        public event EventHandler Added;
        /// <inheritdoc/>
        public event EventHandler Removed;
        /// <inheritdoc/>
        public event EventHandler DescendantChanged;

        /// <summary>
        /// <inheritdoc cref="XObjectChangedEventEvaluation.XElementChanged(object, XObjectChangeEventArgs, XElement, bool)" path="/param[@name='discriminateDescendants']" />
        /// </summary>
        public bool DiscriminateDescendantChanged { get; set; }

        /// <summary>
        /// Set TRUE to create the parent element if it is missing when setting the value
        /// </summary>
        public bool CreateParentIfMissing { get; set; }

        /// <summary>
        /// The name of the XElement
        /// </summary>
        public string Name => XElementField?.Name?.LocalName ?? SearchName;

        /// <summary>
        /// Use Xml.Linq to retrieve the first element with a matching <see cref="Name"/> from the parent node
        /// </summary>
        public XElement XElement
        {
            get => XElementField;
            private set
            {
                if (XElement.ReferenceEquals(XElementField, value)) return;
                if (XElementField != null) //removing the reference
                {

                    XElementField.Changed -= XElementChanged;
                    XElementField = null;
                    Removed?.Invoke(this, new());
                }
                if (value != null)
                {
                    XElementField = value;
                    XElementField.Changed += XElementChanged;
                    Added?.Invoke(this, new());
                    
                }
                OnValueChange();
                OnPropertyChanged("");
            }
        }


        /// <inheritdoc/>
        public string Value
        { 
            get => XElement?.Value;
            set
            {
                if (value == Value) return;
                if (XElement is null)
                {
                    if (!CreateParentIfMissing)
                        return;
                    else
                        CreateXElement();
                    if (XElement is null) throw new Exception("Failed to create the XElement within the tree");
                }
                if (value is null)
                {
                    //Remove text nodes
                    XElement.Nodes().Where(n => n is XText).Remove();
                }
                else
                {
                    XElement.Value = value;
                }
            }
        }


        private XElement GetElementFromParent(string searchTerm) => Parent.XObject?.Element(searchTerm);
        XElement IXElementProvider.XObject => XElement;
        XObject IXObjectProvider.XObject => XElement;

        /// <summary>
        /// Raise ValueChanged and INotifyPropertyChanged('Value')
        /// </summary>
        protected virtual void OnValueChange()
        {
            ValueChanged?.Invoke(this, new());
            OnPropertyChanged(nameof(Value));
        }

        private void Parent_Removed(object sender, EventArgs e)
        {
            XElement = null;
            OnValueChange();
        }

        private void Parent_Added(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Parent_DescendantChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <inheritdoc/>
        public void Refresh()
        {
            XElement = GetElementFunc?.Invoke(SearchName);
        }

        /// <inheritdoc/>
        public void Remove()
        {
            XElement?.Remove();
        }

        /// <inheritdoc/>
        public XElement CreateXElement()
        {
            if (XElement != null) return XElement;
            var x = new XElementWrapper(SearchName);
            Parent.CreateXElement().Add(x);
            return x;
        }

        private void XElementChanged(object sender, XObjectChangeEventArgs e)
        {
            var result = XObjectChangedEventEvaluation.XElementChanged(sender, e, XElement, DiscriminateDescendantChanged);
            switch (result)
            {
                case XObjectChangedEventEvaluation.ChangeType.None: break;
                case XObjectChangedEventEvaluation.ChangeType.NameChanged:
                    OnPropertyChanged(nameof(Name));
                    break;
                case XObjectChangedEventEvaluation.ChangeType.ValueChanged:
                    OnValueChange();
                    break;
                case XObjectChangedEventEvaluation.ChangeType.DescendantAdded:
                case XObjectChangedEventEvaluation.ChangeType.DescendantRemoved:
                    DescendantChanged?.Invoke(this, new());
                    break;
            }
        }

    }

}
