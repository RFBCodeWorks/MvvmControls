using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects
{
    /// <summary>
    /// Abstract Base class for buttons that update their tooltips and display text between two states dynamically
    /// </summary>
    public abstract class AbstractTwoStateButtonDefinition : AbstractButtonDefinition, IButtonTextProvider
    {
        /// <summary>
        /// Instantiate the object
        /// </summary>
        public AbstractTwoStateButtonDefinition()
        {
            ButtonDefinition = ButtonDefinition ?? new();
            ButtonDefinition.PropertyChanged += (o, e) => OnPropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// Object that provides the behavior to update the button text
        /// </summary>
        protected TwoStateButtonTextProvider ButtonDefinition { get; } = new();

        /// <summary>
        /// Text to display on the button
        /// </summary>
        public  override string ButtonText => ButtonDefinition.ButtonText;

        /// <inheritdoc/>
        public override string ToolTip => ButtonDefinition.ToolTip;

        /// <summary>
        /// Raise the 'CanExecuteChanged' event, and updates the button text and the tooltip
        /// </summary>
        public override void NotifyCanExecuteChanged()
        {
            ButtonDefinition.NotifyButtonAndToolTip();
            base.NotifyCanExecuteChanged();
        }

    }
}
