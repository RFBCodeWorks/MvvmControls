using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace RFBCodeWorks.Mvvm.XmlLinq
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
        /// <param name="createElement">
        /// <inheritdoc cref="CreateElementFunc" path="*"/>
        /// </param>
        public XElementProvider(string elementName, IXElementProvider parent, Func<string, XElement> getElement, Func<XElement> createElement = null)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            SearchName = elementName.IsNotEmpty() ? elementName.Trim() : throw new ArgumentException("elementName is Empty!");
            GetElementFunc = getElement ?? throw new ArgumentNullException(nameof(getElement));
            CreateElementFunc = createElement;
            CanBeCreated = createElement != null;

            Parent.DescendantChanged += Parent_DescendantChanged;
            Parent.Added += Parent_Added;
            Parent.Removed += Parent_Removed;
            Refresh();
        }

        /// <summary>
        /// Create a new XElementProvider that will get the first child with a matching <paramref name="elementName"/> from the <paramref name="parent"/>
        /// </summary>
        /// <inheritdoc cref="XElementProvider.XElementProvider(string, IXElementProvider, Func{string, XElement}, Func{XElement})"/>
        public XElementProvider(string elementName, IXElementProvider parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            SearchName = elementName.IsNotEmpty() ? elementName.Trim() : throw new ArgumentException("elementName is Empty!");
            GetElementFunc = GetElementFromParent;
            CreateElementFunc = DefaultCreateNew;
            CanBeCreated = true;
            
            Parent.DescendantChanged += Parent_DescendantChanged;
            Parent.Added += Parent_Added;
            Parent.Removed += Parent_Removed;
            Refresh();
        }

        private XElement XElementField;
        private readonly string SearchName;
        private XElement ParentElement => Parent.XObject;

        /// <summary>
        /// The function to get the XElement from the parent node
        /// </summary>
        private readonly Func<string, XElement> GetElementFunc;
        
        /// <summary>
        /// Function that can be used to generate the XElement object within the tree. <br/>
        /// The returned XElement object will be added as a child of the parent node.
        /// </summary>
        private readonly Func<XElement> CreateElementFunc;

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
        public string Name => XElementField?.Name?.LocalName ?? SearchName;

        /// <inheritdoc/>
        public bool IsNodeAvailable => this.XElement != null;

        /// <inheritdoc/>
        /// <remarks>The XElementProvider also takes its parent nodes into consideration</remarks>
        public bool CanBeCreated
        {
            get => IsNodeAvailable || (CanBeCreatedField && CreateElementFunc != null && (Parent.IsNodeAvailable || Parent.CanBeCreated));
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


        private XElement GetElementFromParent(string searchTerm) => Parent.XObject?.Element(searchTerm);
        XElement IXElementProvider.XObject => XElement;
        XObject IXObjectProvider.XObject => XElement;

        /// <summary>
        /// Raise ValueChanged and INotifyPropertyChanged('Value')
        /// </summary>
        protected virtual void OnValueChange()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
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
            if (!CanBeCreated) return null;
            var x = CreateElementFunc();
            Parent.CreateXElement()?.Add(x);
            return XElement;
        }

        private XElement DefaultCreateNew() => new XElement(SearchName);

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
        /// Create a new XElementProvider object that provide an XElement of a specified name that has a specific attribute value.
        /// <br/> Generates the requires function to get the XElement from the parent node, and also the method to create the node as a child of the parent.
        /// </summary>
        /// <inheritdoc cref="XElementProvider.XElementProvider(string, IXElementProvider, Func{string, XElement}, Func{XElement})"/>
        /// <param name="attributeName">The name of the required attribute</param>
        /// <param name="attributeValue">The value of the required attribute</param>
        /// <param name="parent"/> <param name="elementName"/>
        /// <returns>A new XElement Provider</returns>
        public static XElementProvider GetXElementProvider(IXElementProvider parent, string elementName, string attributeName, string attributeValue)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));
            if (elementName.IsNullOrEmpty()) throw new ArgumentException(nameof(elementName) + " Cannot be empty!");
            if (attributeName.IsNullOrEmpty()) throw new ArgumentException(nameof(attributeName) + " Cannot be empty!");
            if (attributeValue.IsNullOrEmpty()) throw new ArgumentException(nameof(attributeValue) + " Cannot be empty!");

            XElement GetElement(string name) => parent.XObject?.Elements(elementName).SingleOrDefault(n => n.Attribute(attributeName).Value == attributeValue);
            XElement CreateElement() => CreateXElement(elementName, attributeName, attributeValue);
            return new XElementProvider(elementName, parent, GetElement, CreateElement);
        }

        private static XElement CreateXElement(string elementName, string attributeName, string attributeValue)
        {
            //if (elementName.IsNullOrEmpty()) throw new ArgumentException(nameof(elementName) + " Cannot be empty!");
            //if (attributeName.IsNullOrEmpty()) throw new ArgumentException(nameof(attributeName) + " Cannot be empty!");
            //if (attributeValue.IsNullOrEmpty()) throw new ArgumentException(nameof(attributeValue) + " Cannot be empty!");
            var x = new XElement(elementName);
            x.SetAttributeValue(attributeName, attributeValue);
            return x;
        }

        /// <summary>
        /// Write out the contents of the <see cref="XElement"/>
        /// </summary>
        /// <inheritdoc/>
        public override string ToString()
        {
            return XElement?.ToString() ?? string.Empty;
        }
    }

}
