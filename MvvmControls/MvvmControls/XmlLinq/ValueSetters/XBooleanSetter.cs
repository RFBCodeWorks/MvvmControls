using System;

namespace RFBCodeWorks.MvvmControls.XmlLinq.ValueSetters
{
    /// <summary>
    /// Provides a way to interact with an <see cref="IXValueObject"/> to set it to a boolean value
    /// </summary>
    public class XBooleanSetter : ValueSetters.ValueSetterBase<bool?>
    { 
        /// <summary>
        /// Create the XML Boolean Setter
        /// </summary>
        /// <param name="converter">Provider a converter to convert between the boolean value and the string value</param>
        /// <param name="xValueProvider">The object that will provide the node whose value needs to be set</param>
        public XBooleanSetter(IBooleanConverter converter, IXValueObject xValueProvider) : base(xValueProvider)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        /// <inheritdoc cref="XBooleanSetter.XBooleanSetter(DynamicBooleanConverter, IXValueObject, bool)"/>
        public XBooleanSetter(DynamicBooleanConverter converter, IXValueObject xValueProvider) : this(converter, xValueProvider, true) { }

        /// <summary>
        /// Create a new XBoolean setter object that utilizes a DynamicBooleanConverter, and subscribe to the <paramref name="xValueProvider"/>'s event to automatically update the 'ReturnAs' type
        /// </summary>
        /// <inheritdoc cref="XBooleanSetter.XBooleanSetter(IBooleanConverter, IXValueObject)"/>
        /// <param name="subscribe">if TRUE, subscribe to the value provider changing events. If false, don't subscribe.</param>
        /// <param name="converter"/><param name="xValueProvider"/>
        public XBooleanSetter(DynamicBooleanConverter converter, IXValueObject xValueProvider, bool subscribe) : this((IBooleanConverter)converter, xValueProvider)
        {
            if (subscribe)
            {
                void Configure(object sender, EventArgs e) => converter.CongifureConverter(XValueProvider?.Value);
                XValueProvider.Added += Configure;
                XValueProvider.Removed += Configure;
            }
        }

        /// <summary>
        /// Creates a new XmlBooleanSetter that uses <see cref="BooleanConverter.StoreAsBinary"/> as the converter
        /// </summary>
        /// <returns>a new XmlBooleanSetter that uses <see cref="BooleanConverter.StoreAsBinary"/> as the converter</returns>
        /// <inheritdoc cref="XBooleanSetter.XBooleanSetter(IBooleanConverter, IXValueObject)"/>
        public static XBooleanSetter CreateBinarySetter(IXValueObject xValue)
        {
            return new XBooleanSetter(BooleanConverter.StoreAsBinary, xValue);
        }

        /// <summary>
        /// Creates a new XmlBooleanSetter that uses <see cref="BooleanConverter.StoreAsString"/> as the converter
        /// </summary>
        /// <returns>a new XmlBooleanSetter that uses <see cref="BooleanConverter.StoreAsString"/> as the converter</returns>
        /// <inheritdoc cref="XBooleanSetter.XBooleanSetter(IBooleanConverter, IXValueObject)"/>
        public static XBooleanSetter CreateStandardStringSetter(IXValueObject xValue)
        {
            return new XBooleanSetter(BooleanConverter.StoreAsString, xValue);
        }

        /// <summary>
        /// The converter that converts between the string value and the boolean value
        /// </summary>
        public IBooleanConverter Converter { get; }

        /// <summary>
        /// The boolean value of the node. 
        /// </summary>
        /// <remarks>
        /// Evaluates the <see langword="IsEnabled"/> property prior to settings or retrieving the value. <br/>
        /// Get => Converts the boolean value from the <see langword="XValue"/> <br/>
        /// Set => Converts then stored the value to the <see langword="XValue"/> <br/>
        /// </remarks>
        public override bool? Value
        {
            get
            {
                if (XValueProvider.IsNodeAvailable && !String.IsNullOrWhiteSpace(XValueProvider.Value))
                    return Converter.Convert(XValueProvider.Value);
                else
                    return IsThreeState ? null : false;
            }
            set
            {
                base.IsSettingValue = true;
                if (value.HasValue)
                    XValueProvider.Value = Converter.Convert(value.Value);
                else if (IsThreeState)
                {
                    XValueProvider.Value = null;
                }
                base.IsSettingValue = false;
            }
        }
        private bool? ValueField;

        /// <summary>
        /// If TRUE, allows the Value to be set to null, which would allow the following to occur:
        /// <br/> - If this is an XAttribute, the XAttribute node will be removed.
        /// <br/> - If this is an XElement, the Value node (represented by an XText node) will be removed.
        /// </summary>
        public bool IsThreeState
        {
            get { return IsThreeStateField; }
            set { SetProperty(ref IsThreeStateField, value, nameof(IsThreeState)); }
        }
        private bool IsThreeStateField;

        /// <inheritdoc/>
        protected override void XValueProvider_Added()
        {
            ValueField = null;
            OnValueChanged();
        }
        /// <inheritdoc/>
        protected override void XValueProvider_Removed()
        {
            ValueField = null;
            OnValueChanged();
        }
        /// <inheritdoc/>
        protected override void XValueProvider_ValueChanged()
        {
            if (Value != ValueField)
            {
                ValueField = Value;
                OnValueChanged();
            }
        }
    }
}
