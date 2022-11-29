using System;

namespace RFBCodeWorks.MvvmControls.XmlLinq.ValueSetters
{
    /// <summary>
    /// Provides a way to interact with an <see cref="IXValueObject"/> to set it to an <see cref="int"/> value
    /// </summary>
    /// <inheritdoc/>
    public class XIntegerSetter : XNumericSetterBase<int>
    {
        /// <inheritdoc/>
        public XIntegerSetter(IXValueObject xValueProvider) : base(xValueProvider) { Maximum = int.MaxValue; }
        /// <inheritdoc/>
        protected override bool IsWithinRange(int value) => value <= Maximum && value >= Minimum;
        /// <inheritdoc/>
        protected override bool TryParse(string text, out int result) => int.TryParse(text, out result);
    }

    /// <summary>
    /// Provides a way to interact with an <see cref="IXValueObject"/> to set it to a <see cref="double"/> value
    /// </summary>
    /// <inheritdoc/>
    public class XDoubleSetter : XNumericSetterBase<double>
    {
        /// <inheritdoc/>
        public XDoubleSetter(IXValueObject xValueProvider) : base(xValueProvider) { Maximum = double.MaxValue; }
        /// <inheritdoc/>
        protected override bool IsWithinRange(double value) => value <= Maximum && value >= Minimum;
        /// <inheritdoc/>
        protected override bool TryParse(string text, out double result) => double.TryParse(text, out result);
    }


    /// <remarks><br/> - Explicitly implements <see cref="IRangeControl"/> </remarks>
    public abstract class XNumericSetterBase<T> : ValueSetterBase<T>
    where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Create the XML Numeric Setter
        /// </summary>
        /// <param name="xValueProvider">The object that will provide the node whose value needs to be set</param>
        public XNumericSetterBase(IXValueObject xValueProvider) : base(xValueProvider) { }

        /// <summary>
        /// Occurs when an invalid value has been submitted
        /// </summary>
        public event EventHandler<ValueEventArgs<T>> InvalidValueSubmitted;

        /// <summary>
        /// The integer value of the node. 
        /// </summary>
        /// <remarks>
        /// Get => Attempts to parse the value from the node. If unable to, returns the default. <br/>
        /// Set => If <see langword="IsEnabled"/>, store the value to the XValue.
        /// </remarks>
        public override T Value
        {
            get
            {
                return XValueProvider.IsNodeAvailable && TryParse(XValueProvider.Value, out T result) ? result : default;
            }
            set
            {
                if (ValueField != null &&  ValueField.Equals(value)) return;
                base.IsSettingValue = true;
                if (IsWithinRange(value))
                {
                    XValueProvider.Value = ConvertToString(value);
                    ValueField = value;
                    OnPropertyChanged(nameof(Value));
                    OnValueChanged();
                }else
                {
                    InvalidValueSubmitted?.Invoke(this, new(value));
                }
                base.IsSettingValue = false;
            }
        }
        private T? ValueField;

        /// <summary>
        /// The minimum value to accept
        /// </summary>
        public virtual T Minimum { get; set; }

        /// <summary>
        /// The Maximum value to accept
        /// </summary>
        public virtual T Maximum { get; set; }

        /// <summary>
        /// Converts the value to a string for saving to the XObject
        /// </summary>
        /// <param name="value">the value to convert</param>
        /// <returns>a string representation of the value.</returns>
        protected virtual string ConvertToString(T value) => value.ToString();

        /// <summary>
        /// Try to parse the string into the struct value, ex: int.TryParse
        /// </summary>
        /// <inheritdoc cref="int.TryParse(string, out int)"/>
        protected abstract bool TryParse(string text, out T result);

        /// <summary>
        /// Check if the value is between the Min and Max values
        /// </summary>
        /// <param name="value">the value to check</param>
        /// <returns>TRUE if the value is within the range of acceptable values, otherwise false</returns>
        protected abstract bool IsWithinRange(T value);

        /// <inheritdoc/>
        protected override void XValueProvider_ValueChanged()
        {
            ValueField = Value;
            OnValueChanged();
        }

        /// <inheritdoc/>
        protected override void XValueProvider_Removed()
        {
            ValueField = null;
            OnValueChanged();
        }

        /// <inheritdoc/>
        protected override void XValueProvider_Added()
        {
            XValueProvider_ValueChanged();
        }
    }

}