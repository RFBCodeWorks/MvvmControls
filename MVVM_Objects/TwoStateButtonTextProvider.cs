using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects
{
    /// <summary>
    /// Object that provides display Two-State text for both the ToolTip and the Display Text for a button via <see cref="TwoStateTextProvider"/> objects
    /// </summary>
    public class TwoStateButtonTextProvider : ObservableObject, IToolTipProvider, IButtonTextProvider
    {
        /// <summary>
        /// Instantiate thje object
        /// </summary>
        public TwoStateButtonTextProvider()
        {
            ButtonTextProvider = new();
            ToolTipTextProvider = new();
            ButtonTextProvider.PropertyChanged += ButtonTextUpdatedHandler;
            ToolTipTextProvider.PropertyChanged += ToolTipTextUpdatedHandler;
        }

        /// <inheritdoc />
        public TwoStateTextProvider ButtonTextProvider { get; }
        /// <inheritdoc />
        public TwoStateTextProvider ToolTipTextProvider { get; }

        /// <inheritdoc />
        public string ButtonText => ButtonTextProvider.DisplayText;

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
        public void NotifyButtonTextUpdated() => OnPropertyChanged(nameof(ButtonText));

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
