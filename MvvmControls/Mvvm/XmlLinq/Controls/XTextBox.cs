using System;
using System.Text.RegularExpressions;
using RFBCodeWorks.Mvvvm.XmlLinq.ValueSetters;

namespace RFBCodeWorks.Mvvvm.XmlLinq.Controls
{
    /// <summary>
    /// Extends the <see cref="TextControlDefinition"/> to update an <see cref="IXValueObject"/>
    /// </summary>
    public class XTextBox : TextControlDefinition, IXValueControl
    {
        /// <inheritdoc cref="XStringSetter.XStringSetter(IXValueObject)"/>
        public XTextBox(IXValueObject xValueProvider) : base()
        {
            NodeValueSetter = new(xValueProvider);
            NodeValueSetter.XValueProvider.ValueChanged += XValueProvider_ValueChanged; ;
            NodeValueSetter.XValueProvider.Removed += CheckNodeAvailable;
            NodeValueSetter.XValueProvider.Added += CheckNodeAvailable;
            NodeValueSetter.InvalidValueSubmitted += InvalidValueSubmitted;
            NodeValueSetter.XValueProvider.Added += CheckNodeAvailable;
            NodeValueSetter.XValueProvider.Removed += CheckNodeAvailable;
        }

        /// <inheritdoc/>
        public event EventHandler NodeChanged;

        /// <inheritdoc cref="XStringSetter.InvalidValueSubmitted"/>
        public event EventHandler<ValueEventArgs<string>> InvalidValueSubmitted;

        /// <summary>
        /// The object that handles setting the value of the underlying XML Node
        /// </summary>
        public XStringSetter NodeValueSetter { get; }

        /// <inheritdoc/>
        public override string Text
        {
            get => base.Text;
            set
            {
                NodeValueSetter.Value = value;
                base.Text = NodeValueSetter.Value;
            }
        }

        /// <inheritdoc cref="XStringSetter.ValidationRegex"/>
        public Regex ValidationRegex { get => NodeValueSetter.ValidationRegex; set => NodeValueSetter.ValidationRegex = value; }

        private void XValueProvider_ValueChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Text));
        }

        private void CheckNodeAvailable(object sender, EventArgs e)
        {
            IsEnabled = NodeValueSetter.XValueProvider.CanBeCreated;
            NodeChanged?.Invoke(sender, e);
        }
    }
}
