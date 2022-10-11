using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFBCodeWorks.MVVMObjects.ControlInterfaces;

namespace RFBCodeWorks.MVVMObjects
{
    /// <summary>
    /// A definition for a control that displays text
    /// </summary>
    public class TextControlDefinition : BaseControlDefinitions.BaseControlDefinition, IDisplayTextProvider
    {

        /// <summary>
        /// A function that returns a string of text. <br/>
        /// This will be used to apply the result to the <see cref="Text"/> property when the <see cref="Refresh()"/> method is invoked
        /// </summary>
        public Func<string> GetText { get; init; }

        /// <summary>
        /// The text to display
        /// </summary>
        public virtual string Text
        {
            get { return TextField; }
            set { SetProperty(ref TextField, value, nameof(Text)); }
        }
        private string TextField;

        /// <summary>
        /// Flag for Textboxes to set if they are ReadOnly or not
        /// </summary>
        public bool IsReadOnly
        {
            get { return IsReadOnlyField; }
            set { SetProperty(ref IsReadOnlyField, value, nameof(IsReadOnly)); }
        }
        private bool IsReadOnlyField;

        string IDisplayTextProvider.DisplayText => Text;

        /// <summary>
        /// Set the value of Text to the result of <see cref="GetText"/> if a function is defined.
        /// </summary>
        public virtual void Refresh()
        {
            if (GetText != null)
                Text = GetText();
        }

        /// <summary>
        /// Public EventHandler method to allow triggering the refresh via another object's event
        /// </summary>
        public virtual void Refresh(object sender, EventArgs e) => Refresh();

    }
}
