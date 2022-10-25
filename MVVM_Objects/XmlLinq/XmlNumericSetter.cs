using RFBCodeWorks.MVVMObjects.BaseControlDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace RFBCodeWorks.MVVMObjects.XmlLinq
{
    /// <summary>
    /// Provides a way to interact with an <see cref="IXValueObject"/> to set it to an <see cref="int"/> value
    /// </summary>
    /// <inheritdoc/>
    public class XmlIntegerSetter : XmlNumericSetterBase<int>
    {
        /// <inheritdoc/>
        public XmlIntegerSetter(IXValueObject xValueProvider) : base(xValueProvider) { Maximum = int.MaxValue; }
        /// <inheritdoc/>
        protected override bool IsWithinRange(int value) => value <= Maximum && value >= Minimum;
        /// <inheritdoc/>
        protected override bool TryParse(string text, out int result) => int.TryParse(text, out result);
    }

    /// <summary>
    /// Provides a way to interact with an <see cref="IXValueObject"/> to set it to a <see cref="double"/> value
    /// </summary>
    /// <inheritdoc/>
    public class XmlDoubleSetter : XmlNumericSetterBase<double>
    {
        /// <inheritdoc/>
        public XmlDoubleSetter(IXValueObject xValueProvider) : base(xValueProvider) { Maximum = double.MaxValue; }
        /// <inheritdoc/>
        protected override bool IsWithinRange(double value) => value <= Maximum && value >= Minimum;
        /// <inheritdoc/>
        protected override bool TryParse(string text, out double result) => double.TryParse(text, out result);
    }


    /// <remarks><br/> - Explicitly implements <see cref="ControlInterfaces.IRangeControlDefinition"/> </remarks>
    public abstract class XmlNumericSetterBase<T> : ExplicitControlDefinition, ControlInterfaces.IRangeControlDefinition, ControlInterfaces.IDisplayTextProvider
    where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Create the XML Numeric Setter
        /// </summary>
        /// <param name="xValueProvider">The object that will provide the node whose value needs to be set</param>
        public XmlNumericSetterBase(IXValueObject xValueProvider)
        {
            this.XValueProvider = xValueProvider ?? throw new ArgumentNullException(nameof(xValueProvider));
            this.XValueProvider.Removed += XValue_XNodeChanged;
            this.XValueProvider.Added += XValue_XNodeChanged;
            this.XValueProvider.ValueChanged += XValueProvider_ValueChanged;
        }

        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Raise the ValueChanged event
        /// </summary>
        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new());
        }


        /// <summary>
        /// The object that provides XNode whose value will be updated
        /// </summary>
        public IXValueObject XValueProvider { get; }

        /// <summary>
        /// The integer value of the node. 
        /// </summary>
        /// <remarks>
        /// Get => Attempts to parse the value from the node. If unable to, returns the default. <br/>
        /// Set => If <see langword="IsEnabled"/>, store the value to the XValue.
        /// </remarks>
        public virtual T Value
        {
            get
            {
                return IsEnabled && TryParse(XValueProvider.Value, out T result) ? result : default;
            }
            set
            {
                if (!IsEnabled || Value.Equals(value)) return;
                if (IsWithinRange(value))
                {
                    XValueProvider.Value = ConvertToString(value);
                    ValueField = value;
                    OnPropertyChanged(nameof(Value));
                    OnValueChanged();
                }
            }
        }
        private T? ValueField;

        /// <summary>
        /// The minimum value to accept
        /// </summary>
        public virtual T Minimum { get; init; }

        /// <summary>
        /// The Maximum value to accept
        /// </summary>
        public virtual T Maximum { get; init; }


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

        /// <summary>
        /// This is called whenever the XValue.XNodeChanged event is fired.
        /// </summary>
        /// <remarks>Raises OnPropertyChanged for IsEnabled and Value</remarks>
        protected virtual void XValue_XNodeChanged(object sender, EventArgs e)
        {
            ValueField = null;
            OnPropertyChanged(nameof(IsEnabled));
            OnValueChanged();
        }

        private void XValueProvider_ValueChanged(object sender, EventArgs e)
        {
            ValueField = IsEnabled ? Value : null;
            OnPropertyChanged(nameof(Value));
            OnValueChanged();
        }

        /// <summary>
        /// Determine if the value can be set.
        /// </summary>
        /// <remarks>Base implementation evaluates if the XObject provides by the XValue is null or not</remarks>
        public virtual bool IsEnabled => XValueProvider.XObject != null;

        double ControlInterfaces.IRangeControlDefinition.Minimum { get => (double)(object)Minimum; set { } }
        double ControlInterfaces.IRangeControlDefinition.Maximum { get => (double)(object)Maximum; set { } }
        double ControlInterfaces.IRangeControlDefinition.SmallChange { get => 1; set { } }
        double ControlInterfaces.IRangeControlDefinition.LargeChange { get => 1; set { } }
        double ControlInterfaces.IRangeControlDefinition.Value { get => (double)(object)Value; set => Value = (T)(object)value; }
        string ControlInterfaces.IToolTipProvider.ToolTip => $"Min: {ConvertToString(Minimum)} \\ Max: {ConvertToString(Maximum)}";
        string ControlInterfaces.IDisplayTextProvider.DisplayText => XValueProvider.GetName();
    }

}