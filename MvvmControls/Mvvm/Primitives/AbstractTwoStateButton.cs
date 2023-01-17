using System;

namespace RFBCodeWorks.Mvvm.Primitives
{
    /// <summary>
    /// Abstract Base class for buttons that update their tooltips and display text between two states dynamically
    /// </summary>
    public abstract class AbstractTwoStateButton : AbstractButtonDefinition, IDisplayTextProvider
    {
        /// <summary>
        /// Instantiate the object
        /// </summary>
        protected AbstractTwoStateButton()
        {
            DisplayTextProvider = new();
            DisplayTextProvider.DisplayTextUpdated += DisplayTextProvider_PropertyChanged;
            ToolTipProvider = new();
            ToolTipProvider.DisplayTextUpdated += ToolTipProvider_PropertyChanged;
        }

        /// <summary>
        /// Determines which text is provided, and which function will run
        /// </summary>
        public bool IsDefaultState
        {
            get => IsDefaultStateField;
            set
            {
                if (value != IsDefaultStateField)
                {
                    OnPropertyChanging(EventArgSingletons.IsDefaultState);
                    DisplayTextProvider.DisplayAlternateText = value;
                    ToolTipProvider.DisplayAlternateText = value;
                    IsDefaultStateField = value;
                    NotifyCanExecuteChanged();
                    OnPropertyChanged(EventArgSingletons.IsDefaultState);
                }
            }
        }
        private bool IsDefaultStateField = true;

        #region < Properties and Methods for Text Providing >

        /// <summary>
        /// Provides the <see cref="DisplayText" />
        /// </summary>
        private TwoStateTextProvider DisplayTextProvider { get; }

        /// <summary>
        /// Provides the <see cref="ToolTip" />
        /// </summary>
        private TwoStateTextProvider ToolTipProvider { get; }

        /// <summary>
        /// Text to display on the button
        /// </summary>
        public sealed override string DisplayText => DisplayTextProvider.DisplayText;

        /// <inheritdoc/>
        public sealed override string ToolTip => ToolTipProvider.DisplayText;

        /// <summary>
        /// Tooltip to display while <see cref="IsDefaultState"/> == true
        /// </summary>
        public string DefaultTooltip { get => ToolTipProvider.DefaultText; set => ToolTipProvider.DefaultText = value; }

        /// <summary>
        /// Tooltip to display while <see cref="IsDefaultState"/> == false
        /// </summary>
        public string AlternateToolTip { get => ToolTipProvider.AlternateText; set => ToolTipProvider.AlternateText = value; }

        /// <summary>
        /// Button Text  to display while <see cref="IsDefaultState"/> == true
        /// </summary>
        public string DefaultDisplayText { get => DisplayTextProvider.DefaultText; set => DisplayTextProvider.DefaultText = value; }
        /// <summary>
        /// Button Text  to display while <see cref="IsDefaultState"/> == false
        /// </summary>
        public string AlternateDisplayText { get => DisplayTextProvider.AlternateText; set => DisplayTextProvider.AlternateText = value; }

        private void ToolTipProvider_PropertyChanged(object sender, System.EventArgs e)
        {
            OnPropertyChanged(EventArgSingletons.ToolTip);
        }
        private void DisplayTextProvider_PropertyChanged(object sender, System.EventArgs e)
        {
            OnPropertyChanged(EventArgSingletons.DisplayName);
        }

        #endregion

        /// <summary>
        /// Actions to perform when <see cref="IsDefaultState"/> is <see langword="true"/>
        /// </summary>
        protected abstract void DefaultExecute();
        
        /// <summary>
        /// Actions to perform when <see cref="IsDefaultState"/> is <see langword="false"/>
        /// </summary>
        protected abstract void AlternateExecute();
        
        /// <summary>
        /// Check if able to perform the <see cref="DefaultExecute"/>
        /// </summary>
        protected abstract bool DefaultCanExecute();
        
        /// <summary>
        /// Check if able to perform the <see cref="AlternateExecute"/>
        /// </summary>
        protected abstract bool AlternateCanExecute();

        /// <inheritdoc/>
        public sealed override bool CanExecute()
        {
            return IsDefaultState ? DefaultCanExecute() : AlternateCanExecute();
        }

        /// <inheritdoc/>
        public sealed override void Execute()
        {
            if (IsDefaultState)
                DefaultExecute();
            else
                AlternateCanExecute();
        }
    }
}
