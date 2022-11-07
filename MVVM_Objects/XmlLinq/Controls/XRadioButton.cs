using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFBCodeWorks.MVVMObjects.XmlLinq.ValueSetters;

namespace RFBCodeWorks.MVVMObjects.XmlLinq.Controls
{
    /// <summary>
    /// Combines the <see cref="CheckBoxDefinition"/> with an <see cref="XBooleanSetter"/>
    /// </summary>
    public class XRadioButton : RadioButtonDefinition, IXValueControl
    {
        private XRadioButton(XBooleanSetter setter) : base()
        {
            base.StateChange += XRadioButton_StateChange;
            NodeValueSetter = setter;
            NodeValueSetter.IsThreeState = false;
            NodeValueSetter.XValueProvider.ValueChanged += XValueProvider_ValueChanged;
            NodeValueSetter.XValueProvider.Removed += CheckNodeAvailable;
            NodeValueSetter.XValueProvider.Added += CheckNodeAvailable;
        }

        /// <inheritdoc cref="XBooleanSetter.XBooleanSetter(IBooleanConverter, IXValueObject)"/>
        public XRadioButton(IXValueObject xValueProvider, IBooleanConverter converter) : this(new XBooleanSetter(converter, xValueProvider)) { }

        /// <inheritdoc cref="XBooleanSetter.XBooleanSetter(IBooleanConverter, IXValueObject)"/>
        /// <remarks>The boolean converter will be a <see cref="DynamicBooleanConverter"/> that will decide what string value to return based on the node's starting value.</remarks>
        public XRadioButton(IXValueObject xValueProvider) : this(new XBooleanSetter(new DynamicBooleanConverter(), xValueProvider)) { }

        /// <inheritdoc/>
        public event EventHandler NodeChanged;

        /// <summary>
        /// The object that handles setting the value of the underlying XML Node
        /// </summary>
        public XBooleanSetter NodeValueSetter { get; }

        /// <inheritdoc/>
        public override bool IsEnabled
        {
            get => base.IsEnabled;
            set => base.IsEnabled = value;
        }

        private void XRadioButton_StateChange(object sender, EventArgs e)
        {
            NodeValueSetter.Value = IsChecked;
        }

        private void XValueProvider_ValueChanged(object sender, EventArgs e)
        {
            IsChecked = NodeValueSetter.Value ?? false;
        }
        private void CheckNodeAvailable(object sender, EventArgs e)
        {
            IsEnabled = NodeValueSetter.XValueProvider.CanBeCreated;
            NodeChanged?.Invoke(sender, e);
        }
    }
}
