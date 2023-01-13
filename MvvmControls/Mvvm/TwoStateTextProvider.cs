using System;

namespace RFBCodeWorks.Mvvvm
{
    /// <summary>
    /// Object designed to alternate between two displayed values based on a boolean property
    /// </summary>
    public sealed class TwoStateTextProvider : ObservableObject, IDisplayTextProvider
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
        /// Occurs when the DisplayText property is updated
        /// </summary>
        public event EventHandler DisplayTextUpdated;

        /// <summary>
        /// Text to display on the button
        /// </summary>
        public string DisplayText => DisplayAlternateText ? AlternateText : DefaultText;

        private static readonly INotifyArgs DisplayAltArgs = new(nameof(DisplayAlternateText));
        private static readonly INotifyArgs DefaultTextArgs = new(nameof(DefaultText));
        private static readonly INotifyArgs AlternateTextArgs = new(nameof(AlternateText));

        /// <summary>
        /// Default text to display on the button
        /// </summary>
        public string DefaultText
        {
            get => defaultText;
            set
            {
                if (defaultText != value)
                {
                    OnPropertyChanging(DefaultTextArgs);
                    defaultText = value;
                    if (!DisplayAlternateText) OnUpdateButtonText();
                    OnPropertyChanged(DefaultTextArgs);
                }
            }
        }

        /// <summary>
        /// Alternate text to display on the button
        /// </summary>
        public string AlternateText
        {
            get => altText;
            set
            {
                if (altText != value)
                {
                    OnPropertyChanging(AlternateTextArgs);
                    altText = value;
                    if (DisplayAlternateText) OnUpdateButtonText();
                    OnPropertyChanged(AlternateTextArgs);
                }
            }
        }

        /// <summary>
        /// Raise 'PropertyChanged' event for <see cref="DisplayText"/>
        /// </summary>
        public void OnUpdateButtonText()
        {
            DisplayTextUpdated?.Invoke(this, System.EventArgs.Empty);
            OnPropertyChanged(EventArgSingletons.DisplayName);
        }

        /// <summary>
        /// Flag that determines which text is returned by <see cref="DisplayText"/>
        /// </summary>
        public bool DisplayAlternateText
        {
            get => displayAlternateText;
            set
            {
                if (value != displayAlternateText)
                {
                    OnPropertyChanging(DisplayAltArgs);
                    displayAlternateText = value;
                    OnUpdateButtonText();
                    OnPropertyChanged(DisplayAltArgs);
                }
            }
        }
        
    }
}
