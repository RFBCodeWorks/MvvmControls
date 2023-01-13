//using System;
//using System.Collections.Generic;
//using System.Xml.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.ComponentModel;

//namespace RFBCodeWorks.Mvvm.XmlLinq
//{
//    /// <summary>
//    /// Wraps an XAttribute to implement INotifyPropertyChanged
//    /// </summary>
//    public class XAttributeWrapper : XAttribute, INotifyPropertyChanged, IXAttributeProvider, IXObjectProvider, IXValueObject
//    {
//        /// <inheritdoc cref="XAttribute.XAttribute(XName, object)"/>
//        public XAttributeWrapper(XName name, object content) : this(new XAttribute(name, content)) { }

//        /// <inheritdoc cref="XAttribute.XAttribute(XAttribute)"/>
//        public XAttributeWrapper(XAttribute other) : base(other)
//        {
//            base.Changed += XAttributeChanged;
//        }

//        private void XAttributeChanged(object sender, XObjectChangeEventArgs e)
//        {
//            switch(e.ObjectChange)
//            {
//                case XObjectChange.Value:
//                    ValueChanged?.Invoke(this, new EventArgs());
//                    OnPropertyChanged(nameof(Value));
//                    break;
//                default:
//                    throw new NotImplementedException($"Unexepected XObjectChangeEventArg value of type: {e.ObjectChange}");
//            }
//        }

//        /// <inheritdoc/>
//        public event PropertyChangedEventHandler PropertyChanged;

//        /// <inheritdoc/>
//        public event EventHandler ValueChanged;

//        // this will never get raised
//        event EventHandler IXObjectProvider.Added { add { } remove { } }
//        event EventHandler IXObjectProvider.Removed { add { } remove { } }
//        IXElementProvider IXObjectProvider.Parent => this.Parent != null ? new XElementWrapper(this.Parent) : null;
        
//        /// <summary>
//        /// Raise the <see cref="PropertyChanged"/> event
//        /// </summary>
//        /// <param name="propertyName"></param>
//        protected virtual void OnPropertyChanged(string propertyName = "")
//        {
//            PropertyChanged?.Invoke(this, new(propertyName));
//        }

//        string IXValueObject.Name => this.Name.LocalName;
//        XAttribute IXAttributeProvider.XObject => this;
//        XObject IXObjectProvider.XObject => this;
//    }
//}
