using System;

namespace RFBCodeWorks.MvvmControls.XmlLinq.Controls
{
    /// <summary>
    /// Represents a control that has an integer value within a specified range
    /// </summary>
    public class XNumericBox : 
        BaseControlDefinition, IXValueControl,
        IRangeControlDefinition, 
        IDisplayTextProvider,
        IToolTipProvider
    {
        /// <summary>
        /// Create a control that interacts with an XML node, settings it to an integer within a  specified range
        /// </summary>
        /// <inheritdoc cref="ValueSetters.XIntegerSetter.XIntegerSetter(IXValueObject)"/>
        public XNumericBox(IXValueObject xValueProvider)
        {
            NodeValueSetter = new(xValueProvider);
            NodeValueSetter.PropertyChanged += OnPropertyChanged;
            NodeValueSetter.InvalidValueSubmitted += InvalidValueSubmitted;
            NodeValueSetter.XValueProvider.Added += CheckNodeAvailable;
            NodeValueSetter.XValueProvider.Removed += CheckNodeAvailable;
            NodeValueSetter.ValueChanged += ValueChanged;
        }
        /// <inheritdoc/>
        public event EventHandler NodeChanged;
        
        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <inheritdoc cref="ValueSetters.XNumericSetterBase{T}.InvalidValueSubmitted"/>
        public event EventHandler<ValueEventArgs<int>> InvalidValueSubmitted;
        
        /// <summary>
        /// The object that interacts with the node directly
        /// </summary>
        public XmlLinq.ValueSetters.XIntegerSetter NodeValueSetter { get; }

        /// <inheritdoc cref="ValueSetters.XNumericSetterBase{T}.Value"/>
        public int Value { get => NodeValueSetter.Value; set => NodeValueSetter.Value = value; }
        
        /// <inheritdoc cref="ValueSetters.XNumericSetterBase{T}.Minimum"/>
        public int Minimum
        {
            get => NodeValueSetter.Minimum; 
            set
            {
                NodeValueSetter.Minimum = value;
                OnRangeChanged();
            }
        }
        
        /// <inheritdoc cref="ValueSetters.XNumericSetterBase{T}.Maximum"/>
        public int Maximum { 
            get => NodeValueSetter.Maximum;
            set
            {
                NodeValueSetter.Maximum = value;
                OnRangeChanged();
            }
        }

        /// <inheritdoc cref="IRangeControlDefinition.SmallChange"/>
        public int SmallChange { get; set; }
        /// <inheritdoc cref="IRangeControlDefinition.LargeChange"/>
        public int LargeChange { get; set; }

        /// <summary>
        /// Update the tooltip when the Min/Max range changes
        /// </summary>
        protected virtual void OnRangeChanged()
        {
            ToolTip = $"Minimum: {Minimum}\nMaximum: {Maximum}";
        }

        private void CheckNodeAvailable(object sender, EventArgs e)
        {
            IsEnabled = NodeValueSetter.XValueProvider.CanBeCreated;
            NodeChanged?.Invoke(sender, e);
        }

        string IDisplayTextProvider.DisplayText => throw new NotImplementedException();
        double IRangeControlDefinition.Minimum { get => Minimum; set => Minimum = (int)value; }
        double IRangeControlDefinition.Maximum { get => Maximum; set => Maximum = (int)value; }
        double IRangeControlDefinition.SmallChange { get => SmallChange; set => SmallChange = (int)value; }
        double IRangeControlDefinition.LargeChange { get => LargeChange; set => LargeChange = (int)value; }
        double IRangeControlDefinition.Value { get => Value; set => Value = (int)value; }
    }
}
