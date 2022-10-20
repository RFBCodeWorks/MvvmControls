using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RFBCodeWorks.MVVMObjects.XmlLinq
{
    /// <summary>
    /// Class that implements <see cref="IXElementProvider"/> <br/>
    /// Can be set up to use LINQ to get the first matching element from the parent IXElementProvider.
    /// </summary>
    /// <remarks>Explicitly implements <see cref="IXValueProvider"/></remarks>
    public class XElementProvider : ObservableObject, IXElementProvider, IXValueProvider
    {
        /// <summary>
        /// Create a new XElementProvider that represents a singular XElement object
        /// </summary>
        /// <param name="element">The element object that will be provided</param>
        public XElementProvider(XElement element)
        {
            SearchName = element.Name.LocalName;
            Element = element;
            Element.Changed += (_, _) => OnPropertyChanged("");
        }

        /// <summary>
        /// Create a new XElementProvider that will get an XElement that is a descendant of the XElement provided by the <paramref name="parent"/>
        /// </summary>
        /// <param name="elementName">The name of the element to search for</param>
        /// <param name="parent">The object that provides the parent XElement to this object's XElement</param>
        /// <param name="getElement">
        /// A function to get an XElement from the <paramref name="parent"/> <br/>
        /// If null, it will get the first element from the parent that has a matching <paramref name="elementName"/>
        /// </param>
        public XElementProvider(string elementName, IXElementProvider parent, Func<string, XElement> getElement = default)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            SearchName = elementName.IsNotEmpty() ? elementName : throw new ArgumentException("elementName is Empty!");
            GetElementFunc = getElement ?? GetElementFromParent;
            Element = GetElementFunc.Invoke(SearchName);
            SubscribeToParent();
        }

        /// <summary>
        /// Create a new XElementProvider that will the first child with a matching <paramref name="elementName"/> from the <paramref name="parent"/>
        /// </summary>
        /// <inheritdoc cref="XElementProvider.XElementProvider(string, IXElementProvider, Func{string, XElement})"/>
        public XElementProvider(string elementName, IXElementProvider parent) : this(elementName, parent, null)
        {
            //Setup performed in other constructor
        }

        /// <inheritdoc cref="XElementProvider.XElementProvider(string, IXElementProvider, Func{string, XElement})"/>
        public XElementProvider(string elementName, Func<string, XElement> getElement)
        {
            GetElementFunc = getElement ?? throw new ArgumentNullException(nameof(getElement));
            SearchName = elementName.IsNotEmpty() ? elementName : throw new ArgumentException("elementName is Empty!");
            Element = GetElementFunc.Invoke(SearchName);
        }

        private XElement Element;
        private readonly string SearchName;
        private readonly Func<string, XElement> GetElementFunc;
        
        private XElement GetElementFromParent(string searchTerm) => Parent.GetXElement()?.Element(searchTerm);

        /// <inheritdoc/>
        public event EventHandler XElementChanged;

        private event EventHandler NodeChangedEvent;
        event EventHandler IXValueProvider.XNodeChanged
        {
            add
            {
                NodeChangedEvent += value;
            }

            remove
            {
                NodeChangedEvent -= value;
            }
        }

        /// <summary>
        /// Raise the <see cref="XElementChanged"/> event, and raise PropertyChanged XElement
        /// </summary>
        protected virtual void OnXElementChanged()
        {
            XElementChanged?.Invoke(this, new());
            NodeChangedEvent?.Invoke(this, new());
            OnPropertyChanged("");
        }


        /// <summary>
        /// The IXElementProvider object that was passed into the constructor, if any
        /// </summary>
        protected IXElementProvider Parent { get; }

        /// <summary>
        /// The name of the XElement
        /// </summary>
        protected virtual string ElementName => Element?.Name?.LocalName ?? SearchName;

        /// <summary>
        /// Use Xml.Linq to retrieve the first element with a matching <see cref="ElementName"/> from the parent node
        /// </summary>
        public virtual XElement XElement => Element;

        string IXValueProvider.XmlValue
        {
            get => XElement?.Value;
            set
            {
                if (XElement is null) return;
                XElement.Value = value;
            }
        }

        #nullable enable
        XElement? IXElementProvider.GetXElement() => XElement;
        #nullable disable

        /// <summary>
        /// Subscribe the base object to the parent's XElementChanged event, which is used to update the base <see cref="XElement"/> property when the parent XElement notifies a change
        /// </summary>
        protected void SubscribeToParent() => Parent.XElementChanged += Parent_XElementChanged;
        
        /// <summary>
        /// Unsubscribe the base object from the parent's XElementChanged event, which is used to update the base <see cref="XElement"/> property
        /// </summary>
        protected void UnSubscribeFromParent() => Parent.XElementChanged -= Parent_XElementChanged;

        private void Parent_XElementChanged(object sender, EventArgs e)
        {
            var el = GetElementFunc?.Invoke(SearchName);
            if (Element != el)
            {
                Element = el;
                OnXElementChanged();
            }
        }

    }

}
