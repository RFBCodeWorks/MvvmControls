using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects
{
    /// <summary>
    /// Provides a definition for Radio Button controls
    /// </summary>
    public class RadioButtonDefinition : BaseControlDefinitions.ToggleButtonDefinition, ControlInterfaces.IRadioButtonDefinition
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
