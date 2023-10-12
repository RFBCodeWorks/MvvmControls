using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.ComponentModel;
using System.Linq;

namespace RFBCodeWorks.Mvvm.XmlLinq
{
    /// <summary>
    /// Directly wraps an XElement to implement INotifyPropertyChanged
    /// </summary>
    public class XElementWrapper : XElement, INotifyPropertyChanged, IXElementProvider, IXObjectProvider, IXValueObject
    {
        /// <inheritdoc cref="XElement.XElement(XElement)"/>
        public XElementWrapper(XElement other) : base(other)
        {
            base.Changed += XElementChanged;
        }

        /// <inheritdoc cref="XElement.XElement(XName)"/>
        public XElementWrapper(XName name) : this(new XElement(name)) { }

        /// <inheritdoc cref="XElement.XElement(XName, object)"/>
        public XElementWrapper(XName name, object content) : this(new XElement(name, content)) { }

        /// <inheritdoc cref="XElement.XElement(XName, object[])"/>
        public XElementWrapper(XName name, params object[] content) : this(new XElement(name, content)) { }

        /// <inheritdoc cref="XElement.XElement(XStreamingElement)"/>
        public XElementWrapper(XStreamingElement other) : this(new XElement(other)) { }

        // These will never be raised since this object will never be notified of the change
        event EventHandler IXObjectProvider.Added { add { } remove { } }
        event EventHandler IXObjectProvider.Removed { add { } remove { } }
        IXElementProvider IXObjectProvider.Parent => this.Parent != null ? new XElementWrapper(this.Parent) : null;

        /// <inheritdoc/>
        public event EventHandler DescendantChanged;
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// <inheritdoc cref="XObjectChangedEventEvaluation.XElementChanged(object, XObjectChangeEventArgs, XElement, bool)" path="/param[@name='discriminateDescendants']" />
        /// </summary>
        public bool DiscriminateDescendantChanged { get; set; } = true;

        /// <summary>
        /// Raise the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">name of the property that changed</param>
        protected void OnPropertyChanged(string propertyName = "")
        {
            OnPropertyChanged(string.IsNullOrWhiteSpace(propertyName) ? ObservableObject.INotifyAllProperties : new(propertyName));
        }

        /// <inheritdoc cref="OnPropertyChanged(string)"/>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e ?? Mvvm.ObservableObject.INotifyAllProperties);
        }

        /// <inheritdoc cref="XElement.Value"/>
        new public string Value { 
            get => base.Value; 
            set
            {
                if (value is null)
                {
                    var nodes = base.Nodes().OfType<XText>();
                    if (nodes.Any())
                    {
                        nodes.Remove();
                        ValueChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    base.Value = value;
                }
            }
        }

        /// <summary>
        /// An <see cref="IXElementSorter"/> to be used when adding children to this node.
        /// </summary>
        /// <remarks>If not specified, uses the default implementation of <see cref="XElementSorter"/></remarks>
        public IXElementSorter ChildSorter
        {
            get => ChildSorterField ?? XElementSorter.DefaultSorter;
            set => ChildSorterField = value;
        }
        private IXElementSorter ChildSorterField;

        string IXElementProvider.Name => this.Name.LocalName;
        string IXValueObject.Name => this.Name.LocalName;
        XElement IXElementProvider.XObject => this;
        XObject IXObjectProvider.XObject => this;

        bool IXObjectProvider.IsNodeAvailable => true;

        bool IXObjectProvider.CanBeCreated { get => true; set { } }

        XElement IXElementProvider.CreateXElement()
        {
            return this;
        }


        private void XElementChanged(object sender, XObjectChangeEventArgs e)
        {
            var result = XObjectChangedEventEvaluation.XElementChanged(sender, e, this, DiscriminateDescendantChanged);
            switch (result)
            {
                case XObjectChangedEventEvaluation.ChangeType.None: break;
                case XObjectChangedEventEvaluation.ChangeType.NameChanged:
                    OnPropertyChanged(nameof(Name));
                    break;
                case XObjectChangedEventEvaluation.ChangeType.ValueChanged:
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    OnPropertyChanged(EventArgSingletons.ValueChangedArgs);
                    break;
                case XObjectChangedEventEvaluation.ChangeType.DescendantAdded:
                case XObjectChangedEventEvaluation.ChangeType.DescendantRemoved:
                    DescendantChanged?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }
    }
}
