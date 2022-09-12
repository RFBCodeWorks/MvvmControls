using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMObjects
{
    /// <summary>
    /// Provide a Command and a ToolTip <br/>
    /// Implements:
    /// <br/> - <see cref="IRelayCommand"/>
    /// <br/> - <see cref="ICommand"/>
    /// <br/> - <see cref="IToolTipProvider"/>
    /// <br/> - <see cref="INotifyPropertyChanged"/> ( Notify when updating the <see cref="ToolTip"/> )
    /// </summary>
    public interface IButtonDefinition : IRelayCommand, ICommand, IToolTipProvider, INotifyPropertyChanged
    {
    }

    /// <summary>
    /// Interface that enforces the 'ToolTip' property
    /// </summary>
    public interface IToolTipProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Provide a tooltip for some UI control
        /// </summary>
        string ToolTip { get; }
    }

    /// <summary>
    /// Interface that enforces the 'ToolTip' property
    /// </summary>
    public interface IButtonTextProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Provide the ButtonText string
        /// </summary>
        string ButtonText { get; }
    }

    /// <summary>
    /// Gets some text
    /// </summary>
    public interface IDisplayTextProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the text to be displayed to the user
        /// </summary>
        string DisplayText { get; }
    }

    /// <summary>
    /// Abstract base object that inherits from the following:
    /// <br/> - <see cref="ObservableObject"/>
    /// <br/> - <see cref="IRelayCommand"/>
    /// <br/> - <see cref="ICommand"/>
    /// <br/> - <see cref="IToolTipProvider"/>
    /// <br/> - <see cref="INotifyPropertyChanged"/> ( Notify when updating the <see cref="ToolTip"/> )
    /// <br/> - <see cref="IButtonDefinition"/>
    /// </summary>
    public abstract class AbstractButtonDefinition : ObservableObject, IButtonDefinition
    {
        
        /// <inheritdoc cref="IToolTipProvider.ToolTip"/>
        public string ToolTip { 
            get => toolTip;
            set => base.SetProperty(ref toolTip, value, nameof(ToolTip));
        }
        private string toolTip;


        /// <summary>
        /// <inheritdoc cref="ICommand.CanExecuteChanged"/>
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <inheritdoc cref="ICommand.CanExecute(object)"/>
        public virtual bool CanExecute(object parameter) => true;

        /// <inheritdoc cref="ICommand.Execute(object)"/>
        public abstract void Execute(object parameter);

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event
        /// </summary>
        public virtual void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());
    }
    
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

        /// <inheritdoc />
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

        public void NotifyButtonAndToolTip()
        {
            NotifyButtonTextUpdated();
            NotifyToolTipUpdated();
        }

        public void NotifyButtonTextUpdated() => OnPropertyChanged(nameof(ButtonText));
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
            set {
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
        protected TwoStateButtonTextProvider ButtonDefinition{ get; } = new();

        /// <summary>
        /// Text to display on the button
        /// </summary>
        public string ButtonText => ButtonDefinition.ButtonText;

        public string ToolTip => ButtonDefinition.ToolTip;

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
