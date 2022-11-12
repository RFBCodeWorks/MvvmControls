using System;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Interface for ToggleButton objects`
    /// </summary>
    public interface IToggleButton : IDisplayTextProvider, IControlDefinition
    {
        /// <inheritdoc cref="Primitives.ToggleButton.IsChecked"/>
        bool? IsChecked { get; set; }
        /// <inheritdoc cref="Primitives.ToggleButton.IsThreeState"/>
        bool IsThreeState { get; set; }

        /// <inheritdoc cref="Primitives.ToggleButton.Checked"/>
        event EventHandler Checked;
        /// <inheritdoc cref="Primitives.ToggleButton.Indeterminate"/>
        event EventHandler Indeterminate;
        /// <inheritdoc cref="Primitives.ToggleButton.StateChange"/>
        event EventHandler StateChange;
        /// <inheritdoc cref="Primitives.ToggleButton.Unchecked"/>
        event EventHandler Unchecked;

        /// <inheritdoc cref="Primitives.ToggleButton.Toggle"/>
        void Toggle();
    }

}