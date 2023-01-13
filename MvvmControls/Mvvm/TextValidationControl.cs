using System;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace RFBCodeWorks.Mvvvm
{
    /// <summary>
    /// Interface for interacting with a <see cref="System.Windows.Controls.TextBox"/>
    /// </summary>
    public interface ITextValidationControl : ITextControl
    {
        
    }

    /// <summary>
    /// A definition for a control that displays text and validates it using Regex
    /// <para/> Inherits: 
    /// <br/> - <see cref="TextControlDefinition"/>
    /// <br/> - <see cref="ITextValidationControl"/>
    /// <para/>Explicitly Implements:
    /// <br/> - <see cref="IDataErrorInfo"/>
    /// <br/> - <see cref="INotifyDataErrorInfo"/>
    /// </summary>
    public class TextValidationControl : TextControlDefinition, ITextValidationControl, IDataErrorInfo, INotifyDataErrorInfo
    {
        /// <summary>
        /// Create the Text Validation Control
        /// </summary>
        /// <param name="validationRegex"></param>
        public TextValidationControl(Regex validationRegex)
        {
            TextValidation = validationRegex ?? throw new ArgumentNullException(nameof(validationRegex));
        }

        /// <summary>
        /// Occurs when the <see cref="HasErrors"/> value is updated
        /// </summary>
        public event EventHandler ErrorStatusChanged;

        /// <summary>
        /// Raise the <see cref="ErrorStatusChanged"/> event
        /// </summary>
        protected void OnErrorStatusChanged()
        {
            ErrorStatusChanged?.Invoke(this, EventArgs.Empty);
            errored?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Text)));
        }

        /// <inheritdoc/>
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                HasErrors = !TextValidation.IsMatch(value);
            }
        }

        /// <summary>
        /// The Regex used to validate the input text
        /// </summary>
        public Regex TextValidation { get; }

        /// <inheritdoc/>
        public bool HasErrors
        {
            get => HasErrorsField;
            private set
            {
                if (value != HasErrorsField)
                {
                    HasErrorsField = value;
                    OnErrorStatusChanged();
                }
            }
        }
        private bool HasErrorsField;

        /// <summary>
        /// The error text to be used if the <see cref="Text"/> property was deemed invalid
        /// </summary>
        public string Error { get; set; } = "Regex Validation Failed";

        string IDataErrorInfo.Error => HasErrors ? Error : string.Empty;
        string IDataErrorInfo.this[string columnName] => HasErrors ? Error : string.Empty;
        private event EventHandler<DataErrorsChangedEventArgs> errored;
        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add => errored += value;
            remove => errored -= value;
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            return HasErrors ? new string[] { Error } : Array.Empty<string>();
        }
    }
}
