using System;
using System.Text.RegularExpressions;

namespace RFBCodeWorks.MvvmControls.XmlLinq.ValueSetters
{
    /// <summary>
    /// Provides a way to interact with an <see cref="IXValueObject"/> to set it to a string value
    /// </summary>
    public class XStringSetter : ValueSetterBase<string>
    {   
        /// <summary>
        /// Create the XML Integer Setter
        /// </summary>
        /// <param name="xValueProvider">The object that will provide the node whose value needs to be set</param>
        public XStringSetter(IXValueObject xValueProvider) : base(xValueProvider) { }

        /// <summary>
        /// Occurs when an invalid value has been submitted
        /// </summary>
        public event EventHandler<ValueEventArgs<string>> InvalidValueSubmitted;
        
        /// <summary>
        /// Gets and Sets the value of the <see cref="IXValueObject"/>
        /// </summary>
        public override string Value
        {
            get { return XValueProvider.IsNodeAvailable ? XValueProvider.Value : null; }
            set
            {
                if (Value != value)
                {
                    if (ValidationRegex?.IsMatch(value) ?? true)
                    {
                        base.IsSettingValue = true;
                        XValueProvider.Value = value;
                        OnValueChanged();
                        base.IsSettingValue = false;
                    }
                    else
                    {
                        InvalidValueSubmitted?.Invoke(this, new(value));
                    }
                }
            }
        }

        /// <summary>
        /// Regex used to Validate the string prior to storing it to the node.
        /// <br/> If not supplied, any string will be accepted.
        /// </summary>
        public Regex ValidationRegex { get; set; }

        /// <inheritdoc/>
        protected override void XValueProvider_ValueChanged()
        {
            OnValueChanged();
        }

        /// <inheritdoc/>
        protected override void XValueProvider_Removed()
        {
            XValueProvider_ValueChanged();
        }

        /// <inheritdoc/>
        protected override void XValueProvider_Added()
        {
            XValueProvider_ValueChanged();
        }
    }
}
