using System.ComponentModel;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Object that provides display Two-State text for both the ToolTip and the Display Text for a button via <see cref="TwoStateTextProvider"/> objects
    /// </summary>
    public class TwoStateButtonTextProvider : ObservableObject, IToolTipProvider, IDisplayTextProvider
    {
        /// <summary>
        /// Instantiate thje object
        /// </summary>
        public TwoStateButtonTextProvider()
        {
            ButtonTextProvider = new();
            ToolTipTextProvider = new();
        }

        /// <inheritdoc />
        public TwoStateTextProvider ButtonTextProvider
        {
            get => buttontextprovider;
            init
            {
                if (buttontextprovider != null) buttontextprovider.PropertyChanged -= ButtonTextUpdatedHandler;
                buttontextprovider = value;
                value.PropertyChanged -= ButtonTextUpdatedHandler;
            }
        }
        private TwoStateTextProvider buttontextprovider;

        /// <inheritdoc />
        public TwoStateTextProvider ToolTipTextProvider
        {
            get => tooltiptextprovider;
            init
            {
                if (tooltiptextprovider != null) tooltiptextprovider.PropertyChanged -= ToolTipTextUpdatedHandler;
                tooltiptextprovider = value;
                value.PropertyChanged -= ToolTipTextUpdatedHandler;
            }
        }
        private TwoStateTextProvider tooltiptextprovider;

        /// <inheritdoc />
        public string DisplayText => ButtonTextProvider.DisplayText;

        /// <inheritdoc />
        public string ToolTip => ToolTipTextProvider.DisplayText;

        /// <summary>
        /// Set FALSE to display the default text, set TRUE to display the alternate text
        /// </summary>
        public bool DisplayAlternateText
        {
            get => displayAltText;
            set
            {
                ButtonTextProvider.DisplayAlternateText = value;
                ToolTipTextProvider.DisplayAlternateText = value;
                SetProperty(ref displayAltText, value, nameof(DisplayAlternateText));
                NotifyButtonAndToolTip();
            }
        }

        /// <summary>
        /// Raises the notifications to update the ButtonText and ToolTip properties
        /// </summary>
        public void NotifyButtonAndToolTip()
        {
            NotifyButtonTextUpdated();
            NotifyToolTipUpdated();
        }

        /// <summary>
        /// OnPropertyChanged(ButtonText)
        /// </summary>
        public void NotifyButtonTextUpdated() => OnPropertyChanged(nameof(DisplayText));

        /// <summary>
        /// OnPropertyChanged(ToolTip)
        /// </summary>
        public void NotifyToolTipUpdated() => OnPropertyChanged(nameof(ToolTip));


        private void ButtonTextUpdatedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ButtonTextProvider.DisplayText))
                NotifyButtonTextUpdated();
        }
        private void ToolTipTextUpdatedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ToolTipTextProvider.DisplayText))
                NotifyToolTipUpdated();
        }
        private bool displayAltText;
    }

}
