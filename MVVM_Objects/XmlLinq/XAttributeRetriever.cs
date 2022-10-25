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
    public class XAttributeRetriever : ObservableObject, IXAttributeProvider, IXValueObject, IXObjectProvider
    {
        /// <summary>
        /// Create a new XAttributeRetriever that uses LINQ to get an XAttribute of the specified <paramref name="attributeName"/> from the <paramref name="parent"/>
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="parent">The parent that provides the XElement the attribute resides within</param>
        public XAttributeRetriever(string attributeName, IXElementProvider parent)
        {
            AttrName = attributeName.IsNotEmpty() ? attributeName.Trim() : throw new ArgumentException("attributeName is empty");
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            
            Parent.DescendantChanged += Parent_DescendantChanged;
            Parent.Added += Parent_Added; 
            Parent.Removed += Parent_Removed;
            Refresh();
        }

        /// <inheritdoc/>
        public event EventHandler ValueChanged;
        /// <inheritdoc/>
        public event EventHandler Added;
        /// <inheritdoc/>
        public event EventHandler Removed;

        private XAttribute xAttrField;
        private readonly string AttrName;
        private XElement ParentElement => Parent.XObject;

        /// <inheritdoc/>
        public IXElementProvider Parent { get; }

        /// <summary>
        /// Use Xml.Linq to retrieve the first XAttribute with a matching <see cref="Name"/> from the parent node
        /// </summary>
        public XAttribute XAttribute
        {
            get => xAttrField;
            private set
            {
                if (XAttribute.ReferenceEquals(xAttrField, value)) return;
                if (xAttrField != null) //removing the reference
                {

                    xAttrField.Changed -= XAttrField_Changed;
                    xAttrField = null;
                    Removed?.Invoke(this, new());
                }
                if (value != null)
                {
                    xAttrField = value;
                    xAttrField.Changed += XAttrField_Changed;
                    Added?.Invoke(this, new());
                }
                OnValueChange();
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// Gets the Name of the Attribute
        /// </summary>
        public string Name => XAttribute?.Name?.LocalName ?? AttrName;
        
        /// <summary>
        /// Set TRUE to create the parent element if it is missing when setting the value
        /// </summary>
        public bool CreateParentIfMissing { get; set; }

        /// <summary>
        /// Get => Gets the current value of the Attribute, if the attribute does not exist returns null.
        /// <br/> Set => Sets the value of the attribute.
        /// <br/> ---> If the parent XElement exists, it will add the attribute if necessary.
        /// <br/> ---> If the parent XElement does not exist, will create it if '<see cref="CreateParentIfMissing"/>' is true
        /// </summary>
        public virtual string Value
        {
            get => XAttribute?.Value;
            set
            {
                if (value == Value) return;
                if (value is null)
                {
                    //Removing the attribute
                    if (ParentElement is null)
                        return;
                    else
                        XAttribute?.Remove();
                }
                else if (ParentElement is null)
                {
                    if (CreateParentIfMissing)
                        Parent.CreateXElement().SetAttributeValue(AttrName, value);
                }
                else
                {
                    ParentElement.SetAttributeValue(AttrName, value);
                }
            }
        }

        XAttribute IXAttributeProvider.XObject => this.XAttribute;
        XObject IXObjectProvider.XObject => this.XAttribute;

        /// <inheritdoc/>
        public void Refresh()
        {
            XAttribute = ParentElement?.Attribute(AttrName);
        }

        /// <inheritdoc/>
        public void Remove()
        {
            XAttribute?.Remove();
        }

        /// <summary>
        /// Raise ValueChanged and INotifyPropertyChanged('Value')
        /// </summary>
        protected virtual void OnValueChange()
        {
            ValueChanged?.Invoke(this, new());
            OnPropertyChanged(nameof(Value));
        }

        private void XAttrField_Changed(object sender, XObjectChangeEventArgs e)
        {
            //validate - this will throw if not a valid result
            _ = XObjectChangedEventEvaluation.XAttributeChanged(sender, e);
            OnValueChange();
        }

        private void Parent_Removed(object sender, EventArgs e)
        {
            XAttribute = null;
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
    }
}
