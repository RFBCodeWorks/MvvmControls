using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq;

namespace RFBCodeWorks.MVVMObjects.XmlLinq
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
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new(propertyName));
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
                        ValueChanged?.Invoke(this, new());
                    }
                }
                else
                {
                    base.Value = value;
                }
            }
        }

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
                    ValueChanged?.Invoke(this, new());
                    OnPropertyChanged(nameof(Value));
                    break;
                case XObjectChangedEventEvaluation.ChangeType.DescendantAdded:
                case XObjectChangedEventEvaluation.ChangeType.DescendantRemoved:
                    DescendantChanged?.Invoke(this, new());
                    break;
            }
        }
    }
}
