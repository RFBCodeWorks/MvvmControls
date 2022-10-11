using System;

namespace RFBCodeWorks.MVVMObjects.ControlInterfaces
{
    /// <summary>
    /// Interface for ToggleButton objects`
    /// </summary>
    public interface IToggleButtonDefinition : IDisplayTextProvider, IControlDefinition
    {
        /// <inheritdoc cref="BaseControlDefinitions.ToggleButtonDefinition.IsChecked"/>
        bool? IsChecked { get; set; }
        /// <inheritdoc cref="BaseControlDefinitions.ToggleButtonDefinition.IsThreeState"/>
        bool IsThreeState { get; set; }

        /// <inheritdoc cref="BaseControlDefinitions.ToggleButtonDefinition.Checked"/>
        event EventHandler Checked;
        /// <inheritdoc cref="BaseControlDefinitions.ToggleButtonDefinition.Indeterminate"/>
        event EventHandler Indeterminate;
        /// <inheritdoc cref="BaseControlDefinitions.ToggleButtonDefinition.StateChange"/>
        event EventHandler StateChange;
        /// <inheritdoc cref="BaseControlDefinitions.ToggleButtonDefinition.Unchecked"/>
        event EventHandler Unchecked;

        /// <inheritdoc cref="BaseControlDefinitions.ToggleButtonDefinition.Toggle"/>
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