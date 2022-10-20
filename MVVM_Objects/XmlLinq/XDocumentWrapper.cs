using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RFBCodeWorks.MVVMObjects.XmlLinq
{
    /// <summary>
    /// Wraps an XDocument to implement INotifyPropertyChanged
    /// </summary>
    public class XDocumentWrapper : XDocument, INotifyPropertyChanged, IXElementProvider
    {
        /// <inheritdoc cref="XDocument.XDocument()"/>
        public XDocumentWrapper() : base()
        {
            SubScribe();
            Changed += RaiseIXelement;
        }

        /// <inheritdoc cref="XDocument.XDocument(XDocument)"/>
        public XDocumentWrapper(XDocument other) : base(other)
        {
            SubScribe();
            Changed += RaiseIXelement;
        }

        /// <inheritdoc cref="XDocument.XDocument(object[])"/>
        public XDocumentWrapper(params object[] content) : base(content)
        {
            SubScribe();
            Changed += RaiseIXelement;
        }

        /// <inheritdoc cref="XDocument.XDocument(XDeclaration, object[])"/>
        public XDocumentWrapper(XDeclaration declaration, params object[] content) : base(declaration, content)
        {
            SubScribe();
            Changed += RaiseIXelement;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        event EventHandler IXElementProvider.XElementChanged
        {
            add
            {
                IXelementHandlers ??= new();
                IXelementHandlers.Add(value);
            }

            remove
            {
                IXelementHandlers?.Remove(value);
            }
        }

        private List<EventHandler> IXelementHandlers;
        private void RaiseIXelement(object sender, EventArgs e)
        {
            if (IXelementHandlers is null) return;
            foreach(var handler in IXelementHandlers)
            {
                handler?.Invoke(this, new());
            }
        }

        /// <summary>
        /// Returns the root element of the XDocument
        /// </summary>
        /// <returns>The document's root element</returns>
        public virtual XElement GetXElement()
        {
            return base.Root;
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
    }
}
