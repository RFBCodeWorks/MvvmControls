using System;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Interface for ToggleButton objects`
    /// </summary>
    public interface IToggleButton : IDisplayTextProvider, IControlDefinition
    {
        /// <inheritdoc cref="Primitives.ToggleButtonDefinition.IsChecked"/>
        bool? IsChecked { get; set; }
        /// <inheritdoc cref="Primitives.ToggleButtonDefinition.IsThreeState"/>
        bool IsThreeState { get; set; }

        /// <inheritdoc cref="Primitives.ToggleButtonDefinition.Checked"/>
        event EventHandler Checked;
        /// <inheritdoc cref="Primitives.ToggleButtonDefinition.Indeterminate"/>
        event EventHandler Indeterminate;
        /// <inheritdoc cref="Primitives.ToggleButtonDefinition.StateChange"/>
        event EventHandler StateChange;
        /// <inheritdoc cref="Primitives.ToggleButtonDefinition.Unchecked"/>
        event EventHandler Unchecked;

        /// <inheritdoc cref="Primitives.ToggleButtonDefinition.Toggle"/>
        void Toggle();
    }

}