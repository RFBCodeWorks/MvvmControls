using System;
using System.Xml.Linq;
using System.ComponentModel;

namespace RFBCodeWorks.Mvvm.XmlLinq
{
    /// <summary>
    /// Wraps an XDocument to implement INotifyPropertyChanged
    /// </summary>
    public class XDocumentWrapper : XDocument, INotifyPropertyChanged, IXElementProvider, IXObjectProvider
    {
        /// <inheritdoc cref="XDocument.XDocument()"/>
        public XDocumentWrapper() : base()
        {
            base.Changed += XObjectChanged;
        }

        /// <inheritdoc cref="XDocument.XDocument(XDocument)"/>
        public XDocumentWrapper(XDocument other) : base(other)
        {
            base.Changed += XObjectChanged;
        }

        /// <inheritdoc cref="XDocument.XDocument(object[])"/>
        public XDocumentWrapper(params object[] content) : base(content)
        {
            base.Changed += XObjectChanged;
        }

        /// <inheritdoc cref="XDocument.XDocument(XDeclaration, object[])"/>
        public XDocumentWrapper(XDeclaration declaration, params object[] content) : base(declaration, content)
        {
            base.Changed += XObjectChanged;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <inheritdoc/>
        public event EventHandler DescendantChanged;

        event EventHandler IXObjectProvider.Added { add { } remove { } }
        event EventHandler IXObjectProvider.Removed { add { } remove { } }
        
        IXElementProvider IXObjectProvider.Parent => null;
        string IXElementProvider.Name => Root?.Name.LocalName;
        XObject IXObjectProvider.XObject => base.Root;
        XElement IXElementProvider.XObject => base.Root;

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

        /// <summary>
        /// Determine if a Root XElement object exists
        /// </summary>
        public bool IsNodeAvailable => base.Root != null;

        bool IXObjectProvider.CanBeCreated { get => IsNodeAvailable; set { } }

        /// <inheritdoc/>
        public bool CanRaiseAddedOrRemovedEvents => false;

        private void XObjectChanged(object sender, XObjectChangeEventArgs e)
        {
            switch (e.ObjectChange)
            {
                case XObjectChange.Value:
                case XObjectChange.Name:
                    OnPropertyChanged(INotifyArgs.Empty);
                    break;

                case XObjectChange.Add:
                case XObjectChange.Remove:
                    DescendantChanged?.Invoke(this, EventArgs.Empty);
                    OnPropertyChanged(nameof(Root));
                    break;
            }
        }

        /// <summary>
        /// Raise the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName = "")
        {
            OnPropertyChanged(string.IsNullOrWhiteSpace(propertyName) ? INotifyArgs.Empty : new(propertyName));
        }

        /// <inheritdoc cref="OnPropertyChanged(string)"/>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e ?? INotifyArgs.Empty);
        }

        XElement IXElementProvider.CreateXElement()
        {
            return Root;
        }
    }
}
