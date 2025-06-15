using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using RFBCodeWorks.Mvvm.Primitives;

using Toolkit = CommunityToolkit.Mvvm.Input;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm.Primitives
{
    /// <summary>
    /// Abstract Base class for buttons that update their tooltips and display text between two states dynamically
    /// </summary>
    public abstract class AbstractTwoStateButton : ControlBase, IButtonDefinition, ICommand, IDisplayTextProvider, Input.IRelayCommand, Toolkit.IRelayCommand
    {
        
        private bool _enabled = true;
        private bool IsDefaultStateField = true;

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

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
        /// Returns the result of the last <see cref="ICommand.CanExecute(object)"/> call.
        /// </summary>
        /// <remarks>
        /// By default, this will return the last result of the <see cref="CanExecute()"/> evaluation.<br/>
        /// If set to false in code, disables the button entirely, including ignoring checks for <see cref="CanExecute()"/>
        /// </remarks>
        public override bool IsEnabled
        {
            get => _enabled && base.IsEnabled;
            set
            {
                _enabled = value;
                base.IsEnabled = value;
            }
        }

        /// <summary>
        /// Text to display on the button
        /// </summary>
        public string DisplayText => DisplayTextProvider.DisplayText;

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
            OnPropertyChanged(EventArgSingletons.DisplayText);
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
        public bool CanExecute()
        {
            if (_enabled)
            {
                if (IsDefaultState)
                {
                    base.IsEnabled = CanExecute();
                }
                else
                {
                    base.IsEnabled = AlternateCanExecute();
                }
            }
            return _enabled && base.IsEnabled;
        }

        /// <summary>
        /// Calls either <see cref="DefaultExecute"/> or <see cref="AlternateExecute"/> based on the value of <see cref="IsDefaultState"/>
        /// </summary>
        public void Execute()
        {
            if (IsDefaultState)
            {
                if (DefaultCanExecute())
                    DefaultExecute();
            }
            else
            {
                if (AlternateCanExecute())
                    AlternateExecute();
            }
        }

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged(object sender, EventArgs e) => NotifyCanExecuteChanged();

        void ICommand.Execute(object parameter) => Execute();

        bool ICommand.CanExecute(object parameter) => CanExecute();
    }
}
