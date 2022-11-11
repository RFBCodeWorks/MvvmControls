using System;
using System.Xml.Linq;

namespace RFBCodeWorks.MvvmControls.XmlLinq
{
    /// <summary>
    /// Class that is doesn't diffentiate between an XDocument and an XElement, and allows updating the reference to a new xml tree on the fly
    /// </summary>
    /// <remarks>Implements <see cref="IXElementProvider"/></remarks>
    public class XContainerProvider : ObservableObject, IXElementProvider, IXObjectProvider
    {
        /// <summary>
        /// Instantiate the object with a null container
        /// </summary>
        public XContainerProvider()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xContainer"></param>
        public XContainerProvider(XContainer xContainer) : this()
        {
            XContainer = xContainer;
        }

        /// <inheritdoc/>
        public event EventHandler Added;
        /// <inheritdoc/>
        public event EventHandler Removed;
        /// <inheritdoc/>
        public event EventHandler DescendantChanged;

        /// <summary>
        /// The XContainer object to provide
        /// </summary>
        public XContainer XContainer
        {
            get { return XContainerField; }
            set {
                if (XContainer.ReferenceEquals(XContainerField, value)) return;
                //Updating the value
                if (XContainer != null)
                {
                    XContainer.Changed -= XContainer_Changed;
                    XContainerField = null;
                    Removed?.Invoke(this, EventArgs.Empty);
                }
                if (value != null)
                {
                    XContainerField = value;
                    XContainer.Changed += XContainer_Changed;
                    Added?.Invoke(this, EventArgs.Empty);
                }
                OnPropertyChanged(nameof(XContainer));
            }
        }

        /// <summary>
        /// <inheritdoc cref="XObjectChangedEventEvaluation.XElementChanged(object, XObjectChangeEventArgs, XElement, bool)" path="/param[@name='discriminateDescendants']" />
        /// </summary>
        public bool DiscriminateDescendantChanged { get; set; }

        private void XContainer_Changed(object sender, XObjectChangeEventArgs e)
        {
            var result = XObjectChangedEventEvaluation.XElementChanged(sender, e, ProvidedElement, DiscriminateDescendantChanged);
            if (result == XObjectChangedEventEvaluation.ChangeType.DescendantAdded | result == XObjectChangedEventEvaluation.ChangeType.DescendantRemoved)
                DescendantChanged?.Invoke(this, EventArgs.Empty);
        }

        private XContainer XContainerField;

        /// <summary>
        /// The XElement provided by this XContainerProvider.
        /// <br/> - If an XDocument is the XContainer, then this will be the root node.
        /// <br/> - If an XElement is the XContainer, then this will be the XElement.
        /// <br/> - If this is a custom type derived from XContainer, this will throw a <see cref="NotImplementedException"/>
        /// </summary>
        public XElement ProvidedElement
        {
            get
            {
                if (XContainer is null) return null;
                if (XContainer is XDocument doc) return doc.Root;
                if (XContainer is XElement el) return el;
                throw new NotImplementedException("Unknown XContainer Type");
            }
        }

        IXElementProvider IXObjectProvider.Parent => null;
        XObject IXObjectProvider.XObject => ProvidedElement;
        XElement IXElementProvider.XObject => ProvidedElement;


        /// <summary>
        /// The name of the provided XElement object
        /// </summary>
        public string Name => ProvidedElement?.Name?.LocalName;

        /// <inheritdoc/>
        public bool IsNodeAvailable => this.ProvidedElement != null;

        bool IXObjectProvider.CanBeCreated { get => IsNodeAvailable; set { } }

        /// <inheritdoc/>
        public XElement CreateXElement()
        {
            return ProvidedElement;
        }

        /// <summary>
        /// This functionality is not implemented
        /// </summary>
        /// <exception cref="NotImplementedException"/>
        public void Remove()
        {
            throw new InvalidOperationException("'XContainerProvider.Remove' functionality is not implemented since this is designed as a root provider for other IXObjectProvider objects");
        }

        /// <summary>
        /// Write out the contents of the <see cref="XContainer"/>
        /// </summary>
        /// <inheritdoc/>
        public override string ToString()
        {
            return XContainer?.ToString() ?? string.Empty;
        }
    }
}
