using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace RFBCodeWorks.Mvvm.XmlLinq
{

    /// <summary>
    /// A function that searches up the tree for the appropriate XElement node to be provided by the provider.
    /// </summary>
    /// <remarks>Func&lt;<see cref="IXElementProvider"/>, <see cref="string"/>, <see cref="XElement"/>&gt;</remarks>
    /// <param name="parent">The object that provides the parent node within the XML Tree.</param>
    /// <param name="elementName">The element name to search for. This should be a child of the element provided by the parent.</param>
    /// <returns>An XElement node located within the tree. <br/> If an element was not located, return null.</returns>
    public delegate XElement XElementLocatorDelegate(IXElementProvider parent, string elementName);

    /// <summary>
    /// A function that creates a new XElement object with the specified name. <br/>
    /// This will be used to generate the XElement when <see cref="XElementProvider.CreateXElement()"/> is called.
    /// </summary>
    /// <remarks>Func&lt;<see cref="string"/>, <see cref="XElement"/>&gt;</remarks>
    /// <param name="elementName">The name of the XElement node</param>
    /// <returns>A new XElement object to be added to the tree.</returns>
    public delegate XElement CreateElementDelegate(string elementName);

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
        /// If not provided, uses <see cref="LocateByName(IXElementProvider, string)"/>
        /// </param>
        /// <param name="createElement">
        /// A function that creates an XElement object to be added to the tree.
        /// If not provided, uses <see cref="CreateXElement(string)"/>
        /// </param>
        public XElementProvider(string elementName, IXElementProvider parent, XElementLocatorDelegate getElement = default, CreateElementDelegate createElement = default)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            NameField = elementName.IsNotEmpty() ? elementName.Trim() : throw new ArgumentException("elementName is Empty!");
            GetElementFunc = getElement ?? LocateByName;
            CreateElementFunc = createElement ?? CreateXElement;
            CanBeCreated = true;

            Parent.DescendantChanged += Parent_DescendantChanged;
            Parent.Added += Parent_Added;
            Parent.Removed += Parent_Removed;
            Refresh();
        }

        /// <summary>
        /// Create a new XElementProvider that will get the first child with a matching <paramref name="elementName"/> from the <paramref name="parent"/>
        /// </summary>
        /// <inheritdoc cref="XElementProvider.XElementProvider(string, IXElementProvider, XElementLocatorDelegate, CreateElementDelegate)"/>
        public XElementProvider(string elementName, IXElementProvider parent)
            : this(elementName, parent, LocateByName, CreateXElement) { }


        private XElement XElementField;
        private readonly string NameField;
        /// <inheritdoc cref="XElementLocatorDelegate"/>
        private readonly XElementLocatorDelegate GetElementFunc;
        /// <inheritdoc cref="CreateElementDelegate"/>
        private readonly CreateElementDelegate CreateElementFunc;        
        
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
        public bool DiscriminateDescendantChanged { get; set; } = true;

        /// <summary>
        /// The name of the XElement
        /// </summary>
        public string Name => XElementField?.Name?.LocalName ?? NameField;

        /// <inheritdoc/>
        public bool IsNodeAvailable => this.XElement != null;

        /// <inheritdoc/>
        /// <remarks>The XElementProvider also takes its parent nodes into consideration</remarks>
        public bool CanBeCreated
        {
            get => IsNodeAvailable || (CanBeCreatedField && (Parent.IsNodeAvailable || Parent.CanBeCreated));
            set => CanBeCreatedField = value;
        }
        private bool CanBeCreatedField;

        /// <summary>
        /// Use Xml.Linq to retrieve the first element with a matching <see cref="Name"/> from the parent node
        /// </summary>
        public XElement XElement
        {
            get => XElementField;
            private set
            {
                if (ReferenceEquals(XElementField, value)) return;
                if (XElementField != null) //removing the reference
                {

                    XElementField.Changed -= XElementChanged;
                    XElementField = null;
                    Removed?.Invoke(this, EventArgs.Empty);
                }
                if (value != null)
                {
                    XElementField = value;
                    XElementField.Changed += XElementChanged;
                    Added?.Invoke(this, EventArgs.Empty);
                    
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
                    if (!CanBeCreated || value is null)
                    {
                        return;
                    }
                    else
                    {
                        _ = CreateXElement();
                        if (XElement is null)
                            throw new Exception("Failed to create the XElement within the tree");
                    }
                }
                if (value is null)
                {
                    //Remove text nodes
                    var nodes = XElement.Nodes().OfType<XText>();
                    if (nodes.Any())
                    {
                        nodes.Remove();
                        OnValueChange();
                    }                    
                }
                else
                {
                    XElement.Value = value;
                }
            }
        }

        XElement IXElementProvider.XObject => XElement;
        XObject IXObjectProvider.XObject => XElement;

        /// <summary>
        /// Raise ValueChanged and INotifyPropertyChanged('Value')
        /// </summary>
        protected virtual void OnValueChange()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(EventArgSingletons.ValueChangedArgs);
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
            XElement = GetElementFunc(Parent, NameField);
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
            if (!CanBeCreated) return null;
            var x = CreateElementFunc(NameField);
            Parent.CreateXElement()?.Add(x);
            return XElement;
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
                    DescendantChanged?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        /// <summary>
        /// Write out the contents of the <see cref="XElement"/>
        /// </summary>
        /// <inheritdoc/>
        public override string ToString()
        {
            return XElement?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Create a new XElementProvider using the default locator and creator delegators.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static XElementProvider GetXElementProvider(IXElementProvider parent, string elementName)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(elementName)) throw new ArgumentException(nameof(elementName) + " Cannot be empty!");
            return new XElementProvider(elementName, parent, LocateByName, CreateXElement);
        }

        /// <summary>
        /// Create a new XElementProvider object that provide an XElement of a specified name that has a specific attribute value.
        /// <br/> Generates the requires function to get the XElement from the parent node, and also the method to create the node as a child of the parent.
        /// </summary>
        /// <inheritdoc cref="XElementProvider.XElementProvider(string, IXElementProvider, XElementLocatorDelegate, CreateElementDelegate)"/>
        /// <inheritdoc cref="CreateWithAttribute(string, string)"/>
        /// <returns>A new XElement Provider</returns>
        public static XElementProvider GetXElementProvider(IXElementProvider parent, string elementName, string attributeName, string attributeValue)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrWhiteSpace(elementName)) throw new ArgumentException(nameof(elementName) + " Cannot be empty!");
            if (string.IsNullOrWhiteSpace(attributeName)) throw new ArgumentException(nameof(attributeName) + " Cannot be empty!");
            if (string.IsNullOrWhiteSpace(attributeValue)) throw new ArgumentException(nameof(attributeValue) + " Cannot be empty!");
            return new XElementProvider(elementName, parent, LocateByAttribute(attributeName, attributeValue), CreateWithAttribute(attributeName, attributeValue));
        }

        /// <summary> 
        /// Search the <paramref name="parent"/> node and retrieve the first XElement with a matching <paramref name="elementName"/></summary>
        /// <remarks>The default <see cref="XElementLocatorDelegate"/>.<br/></remarks>
        /// <inheritdoc cref="XElementLocatorDelegate"/>
        public static XElement LocateByName(IXElementProvider parent, string elementName) => parent?.XObject?.Element(elementName);

        /// <summary>
        /// Create a new <see cref="XElementLocatorDelegate"/> delegate that searches for an XElement of a specified name that has a specific attribute value.
        /// </summary>
        /// <param name="attributeName">The name of the required attribute</param>
        /// <param name="attributeValue">The value of the required attribute</param>
        /// <returns>A new <see cref="XElementLocatorDelegate"/></returns>
        /// <exception cref="ArgumentException"/>
        public static XElementLocatorDelegate LocateByAttribute(string attributeName, string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeName)) throw new ArgumentException(nameof(attributeName) + " can not be empty.", nameof(attributeName));
            attributeValue ??= string.Empty;
            XElement GetElement(IXElementProvider parent, string elementName) => parent.XObject?.Elements(elementName).FirstOrDefault(n => n.Attribute(attributeName).Value == attributeValue);
            return GetElement;
        }

        /// <summary>
        /// The default function used to create an XElement for the <see cref="XElementProvider"/>
        /// </summary>
        /// <remarks>Default <see cref="CreateElementDelegate"/></remarks>
        /// <param name="elementName">the name of the element to create</param>
        /// <returns>A new XElement object</returns>
        public static XElement CreateXElement(string elementName) => new XElement(elementName);

        /// <summary>
        /// Create a new <see cref="CreateElementDelegate"/> that generates a new XElement with the specified name, <paramref name="attributeName"/>, and <paramref name="attributeValue"/>.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to apply</param>
        /// <param name="attributeValue">The value of the attribute to apply</param>
        /// <returns>A new <see cref="CreateElementDelegate"/></returns>
        public static CreateElementDelegate CreateWithAttribute(string attributeName, string attributeValue)
        {
            if (string.IsNullOrWhiteSpace(attributeName)) throw new ArgumentException(nameof(attributeName) + " can not be empty.", nameof(attributeName));
            XElement Create(string elementName)
            {
                var x = new XElement(elementName);
                x.SetAttributeValue(attributeName, attributeValue ?? string.Empty);
                return x;
            }
            return Create;
        }
    }

}
