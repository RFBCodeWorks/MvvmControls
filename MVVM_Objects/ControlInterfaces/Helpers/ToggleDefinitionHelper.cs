using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.MVVMObjects.ControlInterfaces.Helpers
{
    /// <summary>
    /// Class that implements the IToggleButtonDefinition events
    /// </summary>
    public class ToggleDefinitionHelper : IToggleButtonDefinition, ICheckBoxDefinition
    {
        bool IToggleButtonDefinition.IsThreeState { get => false; set { } }

        string IDisplayTextProvider.DisplayText => "";

        Visibility IControlDefinition.Visibility { get => Visibility.Visible; set { } }
        bool IControlDefinition.IsVisible { get => true; set { } }

        string IToolTipProvider.ToolTip => throw new NotImplementedException();

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                
            }

            remove
            {
                
            }
        }

        /// <inheritdoc/>
        public event EventHandler Checked;
        /// <inheritdoc/>
        public event EventHandler Indeterminate;
        /// <inheritdoc/>
        public event EventHandler StateChange;
        /// <inheritdoc/>
        public event EventHandler Unchecked;

        /// <inheritdoc/>
        public bool? IsChecked
        {
            get { return IsCheckedField; }
            set
            {
                IsCheckedField = value;
                if (IsCheckedField != value)
                {
                    IsCheckedField = value;
                    if (value ?? false)
                        RaiseChecked();
                    else if (value is null)
                        RaiseIndeterminate();
                    else
                        RaiseUnchecked();
                }
            }
        }
        private bool? IsCheckedField;


        /// <summary>
        /// Raises the Checked event
        /// </summary>
        public void RaiseChecked(object sender = null, EventArgs e = null)
        {
            Checked?.Invoke(sender, e ?? new());
        }

        /// <summary>
        /// Raises the Indeterminate event
        /// </summary>
        public void RaiseIndeterminate(object sender = null, EventArgs e = null)
        {
            Indeterminate?.Invoke(sender, e ?? new());
        }

        /// <summary>
        /// Raises the StateChange event
        /// </summary>
        public void RaiseStateChange(object sender = null, EventArgs e = null)
        {
            StateChange?.Invoke(sender, e ?? new());
        }

        /// <summary>
        /// Raises the Unchecked event
        /// </summary>
        public void RaiseUnchecked(object sender = null, EventArgs e = null)
        {
            Unchecked?.Invoke(sender, e ?? new());
        }

        /// <summary>
        /// This does nothing
        /// </summary>
        public void Toggle()
        {
            
        }
    }
}
