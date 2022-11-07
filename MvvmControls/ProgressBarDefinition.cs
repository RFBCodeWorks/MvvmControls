using System;
using System.Windows.Controls;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Interface for an IProgressBar definition
    /// </summary>
    public interface IProgressBarDefinition : IControlDefinition
    {

        /// <inheritdoc cref="ProgressBarDefinition.IsIndeterminate"/>
        bool IsIndeterminate { get; set; }
        /// <inheritdoc cref="ProgressBarDefinition.Maximum"/>
        double Maximum { get; set; }
        /// <inheritdoc cref="ProgressBarDefinition.Minimum"/>
        double Minimum { get; set; }
        /// <inheritdoc cref="ProgressBarDefinition.Value"/>
        double Value { get; set; }
    }

    /// <summary>
    /// Definition for a Progress Bar
    /// </summary>
    public class ProgressBarDefinition : BaseControlDefinition, IProgressBarDefinition
    {
        /// <inheritdoc cref="System.Windows.Controls.Primitives.RangeBase.ValueChanged"/>
        event EventHandler ValueChanged;

        /// <summary>
        /// Raise the ValueChanged event
        /// </summary>
        protected virtual void OnValueChanged(EventArgs e = null)
        {
            ValueChanged?.Invoke(this, e ?? new());
        }

        /// <inheritdoc cref="ProgressBar.IsIndeterminate"/>
        public bool IsIndeterminate
        {
            get { return IsIndeterminateField; }
            set { SetProperty(ref IsIndeterminateField, value, nameof(IsIndeterminate)); }
        }
        private bool IsIndeterminateField;


        /// <inheritdoc cref="System.Windows.Controls.Primitives.RangeBase.Value"/>
        public double Value
        {
            get { return ValueField; }
            set { SetProperty(ref ValueField, value, nameof(Value)); }
        }
        private double ValueField;


        /// <inheritdoc cref="System.Windows.Controls.Primitives.RangeBase.Maximum"/>
        public double Maximum
        {
            get { return MaximumField; }
            set { SetProperty(ref MaximumField, value, nameof(Maximum)); }
        }
        private double MaximumField = 100;


        /// <inheritdoc cref="System.Windows.Controls.Primitives.RangeBase.Minimum"/>
        public double Minimum
        {
            get { return MinimumField; }
            set { SetProperty(ref MinimumField, value, nameof(Minimum)); }
        }
        private double MinimumField;


    }
}
