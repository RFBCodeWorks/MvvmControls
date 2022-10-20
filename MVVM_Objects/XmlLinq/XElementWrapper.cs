using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RFBCodeWorks.MVVMObjects.XmlLinq
{
    /// <summary>
    /// Directly wraps an XElement to implement INotifyPropertyChanged
    /// </summary>
    public class XElementWrapper : XElement, INotifyPropertyChanged, IXElementProvider
    {
        /// <inheritdoc cref="XElement.XElement(XElement)"/>
        public XElementWrapper(XElement other) : base(other)
        {
            SubScribe();
        }

        /// <inheritdoc cref="XElement.XElement(XName)"/>
        public XElementWrapper(XName name) : base(name)
        {
            SubScribe();
        }

        /// <inheritdoc cref="XElement.XElement(XName, object)"/>
        public XElementWrapper(XName name, object content) : base(name, content)
        {
            SubScribe();
        }

        /// <inheritdoc cref="XElement.XElement(XName, object[])"/>
        public XElementWrapper(XName name, params object[] content) : base(name, content)
        {
            SubScribe();
        }

        /// <inheritdoc cref="XElement.XElement(XStreamingElement)"/>
        public XElementWrapper(XStreamingElement other) : base(other)
        {
            SubScribe();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

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

        event EventHandler IXElementProvider.XElementChanged
        {
            add { }
            remove { }
        }

        XElement IXElementProvider.GetXElement()
        {
            return this;
        }
    }
}
