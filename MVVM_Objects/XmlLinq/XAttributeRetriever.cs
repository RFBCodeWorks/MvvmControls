using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RFBCodeWorks.MVVMObjects.XmlLinq
{
    /// <summary>
    /// Provides a way to present an XAttribute into a ViewModel via XML.Linq
    /// </summary>
    public class XAttributeRetriever : ObservableObject, IXAttributeProvider
    {
        /// <summary>
        /// Create a new XAttributeRetriever that represents some XAttribute object
        /// </summary>
        /// <param name="xAttribute">the XAttribute</param>
        public XAttributeRetriever(XAttribute xAttribute)
        {
            if (xAttribute is XAttributetWrapper wrapper)
                XAttr = wrapper;
            else
                XAttr =  new(xAttribute);
            
            AttrName = XAttr.Name.LocalName;
        }

        /// <summary>
        /// Create a new XAttributeRetriever that uses LINQ to get an XAttribute of the specified <paramref name="attributeName"/> from the <paramref name="parent"/>
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="parent">The parent that provides the XElement the attribute resides within</param>
        public XAttributeRetriever(string attributeName, IXElementProvider parent)
        {
            XElementProvider = parent ?? throw new ArgumentNullException(nameof(parent));
            AttrName = attributeName.IsNotEmpty() ? attributeName : throw new ArgumentException("attributeName is empty");
            parent.XElementChanged += RefreshxAttr;
        }

        /// <inheritdoc/>
        public event EventHandler XAttributeChanged;
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
        
        private XAttributetWrapper xAttrField;
        private readonly string AttrName;
        private readonly IXElementProvider XElementProvider;

        private XAttributetWrapper XAttr {
            get => xAttrField;
            set
            {
                if (xAttrField != value)
                {
                    xAttrField.PropertyChanged += XAttrField_PropertyChanged;
                    xAttrField = value;
                    if (value != null) value.PropertyChanged += XAttrField_PropertyChanged;
                    OnXAttributeChanged();
                }
            }
        }

        /// <summary>
        /// Gets the Name of the Attribute
        /// </summary>
        public string AttributeName => XAttr?.Name?.LocalName ?? AttrName;
        
        /// <summary>
        /// Get => Gets the current value of the Attribute, if the attribute does not exist returns null.
        /// <br/> Set => Sets the value of the attribute, adding it if necessary.
        /// </summary>
        public virtual string XmlValue
        {
            get => XAttr?.Value;
            set
            {
                if (XAttr is null && value.IsNotEmpty())
                {
                    XElementProvider.GetXElement()?.SetAttributeValue(AttributeName, value);
                }
                else if (XAttr != null)
                {
                    XAttr.Value = value;
                }
                OnPropertyChanged(nameof(XmlValue));
            }
        }

        /// <inheritdoc/>
        public XAttribute GetXAttribute()
        {
            return XAttr;
        }

        private void RefreshxAttr(object sender, EventArgs e)
        {
            var val = XElementProvider?.GetXElement()?.Attribute(this.AttrName);
            if (val is null)
                XAttr = null;
            else if (XAttr is XAttributetWrapper wrapper)
                XAttr = wrapper;
            else
                XAttr = new(val);
        }

        private void XAttrField_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged("");
        }

        /// <summary>
        /// Raises XAttributeChanged
        /// </summary>
        protected virtual void OnXAttributeChanged()
        {
            XAttributeChanged?.Invoke(this, new());
            NodeChangedEvent?.Invoke(this, new());
            OnPropertyChanged("");
        }
    }
}
