namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Provides a definition for Radio Button controls
    /// </summary>
    public class RadioButtonDefinition : ToggleButtonDefinition, IRadioButtonDefinition
    {
        /// <inheritdoc cref="System.Windows.Controls.RadioButton.GroupName"/>
        public string GroupName
        {
            get { return GroupNameField; }
            set { SetProperty(ref GroupNameField, value, nameof(GroupName)); }
        }
        private string GroupNameField;

        /// <summary>
        /// Radio Buttons do not support 3-States, so this will always be false.
        /// </summary>
        public override bool IsThreeState { get => base.IsThreeState; set { } }

    }
}
