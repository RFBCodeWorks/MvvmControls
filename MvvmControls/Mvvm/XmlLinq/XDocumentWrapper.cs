using System;
using System.Xml.Linq;
using System.ComponentModel;

namespace RFBCodeWorks.Mvvvm.XmlLinq
{
    /// <summary>
    /// Wraps an XDocument to implement INotifyPropertyChanged
    /// </summary>
    public class XDocumentWrapper : XDocument, INotifyPropertyChanged, IXElementProvider, IXObjectProvider
    {
        /// <inheritdoc cref="XDocument.XDocument()"/>
        public XDocumentWrapper() : base()
        {
            SubScribe();
            base.Changed += XObjectChanged;
        }

        /// <inheritdoc cref="XDocument.XDocument(XDocument)"/>
        public XDocumentWrapper(XDocument other) : base(other)
        {
            SubScribe();
            base.Changed += XObjectChanged;
        }

        /// <inheritdoc cref="XDocument.XDocument(object[])"/>
        public XDocumentWrapper(params object[] content) : base(content)
        {
            SubScribe();
            base.Changed += XObjectChanged;
        }

        /// <inheritdoc cref="XDocument.XDocument(XDeclaration, object[])"/>
        public XDocumentWrapper(XDeclaration declaration, params object[] content) : base(declaration, content)
        {
            SubScribe();
            base.Changed += XObjectChanged;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <inheritdoc/>
        public event EventHandler DescendantChanged;

        private event EventHandler IXRemoved;
        private event EventHandler IXAdded;

        event EventHandler IXObjectProvider.Removed { add { IXRemoved += value; } remove { IXRemoved -= value; } }
        event EventHandler IXObjectProvider.Added { add { IXAdded += value; } remove { IXAdded -= value; } }
        IXElementProvider IXObjectProvider.Parent => null;
        string IXElementProvider.Name => Root?.Name.LocalName;
        XObject IXObjectProvider.XObject => base.Root;
        XElement IXElementProvider.XObject => base.Root;

        /// <summary>
        /// Determine if a Root XElement object exists
        /// </summary>
        public bool IsNodeAvailable => base.Root != null;

        bool IXObjectProvider.CanBeCreated { get => IsNodeAvailable; set { } }

        private void XObjectChanged(object sender, XObjectChangeEventArgs e)
        {
            if (sender == this | sender == this.Root)
            {
                switch (e.ObjectChange)
                {
                    case XObjectChange.Value:
                    case XObjectChange.Name:
                        OnPropertyChanged("");
                        break;   
                        
                    case XObjectChange.Add:
                        IXAdded?.Invoke(sender, new());
                        break;

                    case XObjectChange.Remove:
                        IXRemoved?.Invoke(sender, new());
                        break;
                }
            }
            else if (e.ObjectChange == XObjectChange.Remove || e.ObjectChange == XObjectChange.Add)
            {
                DescendantChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raise the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        private void SubScribe()
        {
            base.Changed += (o, e) => OnPropertyChanged();
        }

        XElement IXElementProvider.CreateXElement()
        {
            return Root;
        }
    }
}
