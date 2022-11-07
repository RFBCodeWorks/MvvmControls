using System;

namespace RFBCodeWorks.MvvmControls.ControlInterfaces
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

    /// <summary>
    /// Interface for CheckBox Definitions
    /// </summary>
    public interface ICheckBoxDefinition : IToggleButtonDefinition
    {

    }

    /// <summary>
    /// Interface for Radio Button Definitions
    /// </summary>
    public interface IRadioButtonDefinition : IToggleButtonDefinition
    {
        /// <inheritdoc cref="MVVMObjects.RadioButtonDefinition.GroupName"/>
        public string GroupName { get; set; }
    }
}