using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RFBCodeWorks.MVVMObjects.XmlLinq
{
    /// <summary>
    /// Provides a way to interact with an <see cref="IXValueObject"/> to set it to a boolean value
    /// </summary>
    /// <remarks>
    /// <br/> - Explicitly implements <see cref="ControlInterfaces.IToggleButtonDefinition"/> 
    /// <br/> - Explicitly implements <see cref="ControlInterfaces.ICheckBoxDefinition"/> 
    /// <br/> - Explicitly implements <see cref="ControlInterfaces.IRadioButtonDefinition"/> 
    /// </remarks>
    public class XmlBooleanSetter : BaseControlDefinitions.ExplicitControlDefinition, ControlInterfaces.IToggleButtonDefinition, ControlInterfaces.ICheckBoxDefinition, ControlInterfaces.IRadioButtonDefinition 
    {
        /// <summary>
        /// Create the XML Boolean Setter
        /// </summary>
        /// <param name="converter">Provider a converter to convert between the boolean value and the string value</param>
        /// <param name="xValueProvider">The object that will provide the node whose value needs to be set</param>
        public XmlBooleanSetter(IBooleanConverter converter, IXValueObject xValueProvider)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            this.XValueProvider = xValueProvider ?? throw new ArgumentNullException(nameof(xValueProvider));
            this.XValueProvider.Added += XValue_XNodeChanged;
            this.XValueProvider.Removed += XValue_XNodeChanged;
            this.XValueProvider.ValueChanged += XValueProvider_ValueChanged;
        }

        /// <summary>
        /// Creates a new XmlBooleanSetter that uses <see cref="BooleanConverter.StoreAsBinary"/> as the converter
        /// </summary>
        /// <returns>a new XmlBooleanSetter that uses <see cref="BooleanConverter.StoreAsBinary"/> as the converter</returns>
        /// <inheritdoc cref="XmlBooleanSetter.XmlBooleanSetter(IBooleanConverter, IXValueObject)"/>
        public static XmlBooleanSetter CreateBinarySetter(IXValueObject xValue)
        {
            return new XmlBooleanSetter(BooleanConverter.StoreAsBinary, xValue);
        }

        /// <summary>
        /// Creates a new XmlBooleanSetter that uses <see cref="BooleanConverter.StoreAsString"/> as the converter
        /// </summary>
        /// <returns>a new XmlBooleanSetter that uses <see cref="BooleanConverter.StoreAsString"/> as the converter</returns>
        /// <inheritdoc cref="XmlBooleanSetter.XmlBooleanSetter(IBooleanConverter, IXValueObject)"/>
        public static XmlBooleanSetter CreateStandardStringSetter(IXValueObject xValue)
        {
            return new XmlBooleanSetter(BooleanConverter.StoreAsString, xValue);
        }

        /// <summary>
        /// string to be used by the IDisplayTextProvider.DisplayText explicit interface
        /// </summary>
        protected string DisplayTextProvider { get => DispText ?? XValueProvider.GetName(); set => SetProperty(ref DispText, value, nameof(ControlInterfaces.IDisplayTextProvider.DisplayText)); }
        private string DispText;

        /// <summary>
        /// string to be used by the IRadioButtonDefinition explicit interface
        /// </summary>
        protected string IRadioButtonGroupName { get => groupName; set => SetProperty(ref groupName, value, nameof(ControlInterfaces.IRadioButtonDefinition.GroupName)); }
        private string groupName;

        /// <summary>
        /// The converter that converts between the string value and the boolean value
        /// </summary>
        public IBooleanConverter Converter { get; }

        /// <summary>
        /// The object that provides XNode whose value will be updated
        /// </summary>
        public IXValueObject XValueProvider { get; }

        /// <summary>
        /// The boolean value of the node. 
        /// </summary>
        /// <remarks>
        /// Evaluates the <see langword="IsEnabled"/> property prior to settings or retrieving the value. <br/>
        /// Get => Converts the boolean value from the <see langword="XValue"/> <br/>
        /// Set => Converts then stored the value to the <see langword="XValue"/> <br/>
        /// </remarks>
        public bool Value
        {
            get { return IsEnabled && Converter.Convert(XValueProvider.Value); }
            set
            {
                if (IsEnabled)
                {
                    XValueProvider.Value = Converter.Convert(value);
                }
            }
        }
        private bool? ValueField;

        /// <summary>
        /// This is called whenever the XValue.XNodeChanged event is fired.
        /// </summary>
        /// <remarks>Raises OnPropertyChanged for IsEnabled and Value</remarks>
        protected virtual void XValue_XNodeChanged(object sender, EventArgs e)
        {
            ValueField = null;
            if (!IsEnabled)
            {
                IndeterminateEvent?.Invoke(this, new());
                StateChangeEvent?.Invoke(this, new());
            }
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(IsEnabled));
        }

        private void XValueProvider_ValueChanged(object sender, EventArgs e)
        {
            if (!IsEnabled) return;
            bool value = Value;
            if (value != ValueField)
            {
                if (value)
                    CheckedEvent?.Invoke(this, new());
                else
                    UnCheckedEvent?.Invoke(this, new());
                StateChangeEvent?.Invoke(this, new());
                ValueField = value;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(ControlInterfaces.IDisplayTextProvider.DisplayText));
            }
        }

        /// <inheritdoc/>
        public void Toggle()
        {
            Value = !Value;
        }

        /// <summary>
        /// Determine if the value can be set.
        /// </summary>
        public bool IsEnabled => XValueProvider.XObject != null;

        #region < Explicit Interfaces >

        string ControlInterfaces.IDisplayTextProvider.DisplayText => DisplayTextProvider;
        string ControlInterfaces.IRadioButtonDefinition.GroupName { get => IRadioButtonGroupName; set => IRadioButtonGroupName = value; }
        bool? ControlInterfaces.IToggleButtonDefinition.IsChecked { get => Value; set => Value = value ?? false; }
        bool ControlInterfaces.IToggleButtonDefinition.IsThreeState { get => false; set { } }

        private event EventHandler CheckedEvent;
        private event EventHandler UnCheckedEvent;
        private event EventHandler IndeterminateEvent;
        private event EventHandler StateChangeEvent;

        event EventHandler ControlInterfaces.IToggleButtonDefinition.Checked
        {
            add
            {
                CheckedEvent += value;
            }

            remove
            {
                CheckedEvent -= value;
            }
        }

        event EventHandler ControlInterfaces.IToggleButtonDefinition.Indeterminate
        {
            add
            {
                IndeterminateEvent += value;
            }

            remove
            {
                IndeterminateEvent -= value;
            }
        }

        event EventHandler ControlInterfaces.IToggleButtonDefinition.StateChange
        {
            add
            {
                StateChangeEvent += value;
            }

            remove
            {
                StateChangeEvent -= value;
            }
        }

        event EventHandler ControlInterfaces.IToggleButtonDefinition.Unchecked
        {
            add
            {
                UnCheckedEvent += value;
            }

            remove
            {
                UnCheckedEvent -= value;
            }
        }

        #endregion


    }
}
