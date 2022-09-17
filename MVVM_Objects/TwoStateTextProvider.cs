using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects
{
    /// <summary>
    /// Object designed to alternate between two displayed values based on a boolean property
    /// </summary>
    public class TwoStateTextProvider : ObservableObject, IDisplayTextProvider
    {
        /// <summary>
        /// Instantiate the command without providing any data.
        /// </summary>
        public TwoStateTextProvider() { }

        /// <summary>
        /// Provide the DefaultText to display
        /// </summary>
        public TwoStateTextProvider(string defaultText)
        {
            DefaultText = defaultText;
        }

        /// <summary>
        /// Provide the DefaultText and the AlternateText to display
        /// </summary>
        public TwoStateTextProvider(string defaultText, string alternateText)
        {
            DefaultText = defaultText;
            AlternateText = alternateText;
        }

        private bool displayAlternateText;
        private string defaultText;
        private string altText;

        /// <summary>
        /// Text to display on the button
        /// </summary>
        public string DisplayText => DisplayAlternateText ? AlternateText : DefaultText;

        /// <summary>
        /// Default text to display on the button
        /// </summary>
        public string DefaultText
        {
            get => defaultText;
            set
            {
                if (SetProperty(ref defaultText, value, nameof(DefaultText)))
                    if (!DisplayAlternateText)
                        OnUpdateButtonText();
            }
        }

        /// <summary>
        /// Alternate text to display on the button
        /// </summary>
        public virtual string AlternateText
        {
            get => altText;
            set
            {
                if (SetProperty(ref altText, value, nameof(AlternateText)))
                    if (DisplayAlternateText)
                        OnUpdateButtonText();
            }
        }

        /// <summary>
        /// Raise 'PropertyChanged' event for <see cref="DisplayText"/>
        /// </summary>
        public void OnUpdateButtonText() => OnPropertyChanged(nameof(DisplayText));

        /// <summary>
        /// Flag that determines which text is returned by <see cref="DisplayText"/>
        /// </summary>
        public virtual bool DisplayAlternateText
        {
            get => displayAlternateText;
            set => _ = OnDisplayAlternateTextChanged(value);
        }


        /// <summary>
        /// Sets the private field to the <paramref name="newValue"/>, then raises the PropertyChanged event if the value was updated
        /// </summary>
        /// <param name="newValue"></param>
        protected bool OnDisplayAlternateTextChanged(bool newValue)
        {
            if (SetProperty(ref displayAlternateText, newValue, nameof(DisplayAlternateText)))
            {
                OnUpdateButtonText();
                return true;
            }
            return false;
        }
    }

}
