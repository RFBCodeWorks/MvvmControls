using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RFBCodeWorks.MVVMObjects.XmlLinq
{
    /// <summary>
    /// Wraps an XAttribute to implement INotifyPropertyChanged
    /// </summary>
    public class XAttributetWrapper : XAttribute, INotifyPropertyChanged, IXAttributeProvider
    {
        /// <inheritdoc cref="XAttribute.XAttribute(XName, object)"/>
        public XAttributetWrapper(XName name, object content) : base(name, content)
        {
            SubScribe();
        }

        /// <inheritdoc cref="XAttribute.XAttribute(XAttribute)"/>
        public XAttributetWrapper(XAttribute other) : base(other)
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

        event EventHandler IXAttributeProvider.XAttributeChanged
        {
            add { }
            remove { }
        }

        XAttribute IXAttributeProvider.GetXAttribute()
        {
            return this;
        }

        string IXValueProvider.XmlValue { get => base.Value; set => base.Value = value; }
    }
}
