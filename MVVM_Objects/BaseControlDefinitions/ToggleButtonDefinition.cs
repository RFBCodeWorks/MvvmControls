using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using RFBCodeWorks.MVVMObjects.ControlInterfaces;

namespace RFBCodeWorks.MVVMObjects.BaseControlDefinitions
{
    /// <summary>
    /// Base Definition for a CheckBox or other Toggle Button
    /// </summary>
    public class ToggleButtonDefinition : BaseControlDefinitions.BaseControlDefinition, IDisplayTextProvider, IToggleButtonDefinition
    {
        /// <summary>
        /// Occurs when the value is <see cref="IsChecked"/> is updated to a new value, prior to the corresponding event that is raised based on the actual new value
        /// </summary>
        public event EventHandler StateChange;
        /// <summary>
        /// Occurs when the IsChecked property gets set to TRUE
        /// </summary>
        public event EventHandler Checked;
        /// <summary>
        /// Occurs when the IsChecked property gets set to FALSE
        /// </summary>
        public event EventHandler Unchecked;
        /// <summary>
        /// Occurs when the IsChecked property gets set to null
        /// </summary>
        public event EventHandler Indeterminate;

        /// <inheritdoc cref="ToggleButton.IsChecked"/>
        public virtual bool? IsChecked
        {
            get { return IsCheckedField; }
            set
            {
                if (value is null && !IsThreeState) return; //ignore null values if IsThreeState is set to FALSE
                var updated = SetProperty(ref IsCheckedField, value, nameof(IsChecked));
                if (updated)
                {
                    StateChange?.Invoke(this, new());
                    if (value is null)
                        Indeterminate?.Invoke(this, new());
                    else if (value.Value)
                        Checked?.Invoke(this, new());
                    else
                        Unchecked?.Invoke(this, new());
                }
            }
        }
        private bool? IsCheckedField = false;

        /// <inheritdoc cref="ToggleButton.IsThreeState"/>
        public virtual bool IsThreeState
        {
            get { return IsThreeStateField; }
            set { SetProperty(ref IsThreeStateField, value, nameof(IsThreeState)); }
        }
        private bool IsThreeStateField;

        /// <inheritdoc/>
        public string DisplayText
        {
            get { return DisplayTextField; }
            set { SetProperty(ref DisplayTextField, value, nameof(DisplayText)); }
        }
        private string DisplayTextField;

        /// <summary>
        /// Inverts the value of <see cref="IsChecked"/> <br/>
        /// ( If false or null, set true. If true, set false )
        /// </summary>
        public void Toggle()
        {
            IsChecked = !(IsChecked ?? false);
        }

        /// <summary>
        /// Gets the value as a boolean, where null represents a false value
        /// </summary>
        public bool GetBoolean() => IsChecked ?? false;

    }
}
