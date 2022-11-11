using System;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Interface for ToggleButton objects`
    /// </summary>
    public interface IToggleButtonDefinition : IDisplayTextProvider, IControlDefinition
    {
        /// <inheritdoc cref="ToggleButtonDefinition.IsChecked"/>
        bool? IsChecked { get; set; }
        /// <inheritdoc cref="ToggleButtonDefinition.IsThreeState"/>
        bool IsThreeState { get; set; }

        /// <inheritdoc cref="ToggleButtonDefinition.Checked"/>
        event EventHandler Checked;
        /// <inheritdoc cref="ToggleButtonDefinition.Indeterminate"/>
        event EventHandler Indeterminate;
        /// <inheritdoc cref="ToggleButtonDefinition.StateChange"/>
        event EventHandler StateChange;
        /// <inheritdoc cref="ToggleButtonDefinition.Unchecked"/>
        event EventHandler Unchecked;

        /// <inheritdoc cref="ToggleButtonDefinition.Toggle"/>
        void Toggle();
    }

}