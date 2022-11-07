using System;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// This object is comprised of two RelayCommands, but which one is active depends on the state of the object
    /// </summary>
    public class TwoStateRelayCommand : AbstractTwoStateButtonDefinition, IButtonDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public TwoStateRelayCommand() : base() {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultAction"></param>
        /// <param name="defaultCanExecute"></param>
        /// <param name="alternateAction"></param>
        /// <param name="alternateCanExecute"></param>
        public TwoStateRelayCommand(Action defaultAction, Func<bool> defaultCanExecute, Action alternateAction, Func<bool> alternateCanExecute) : base() 
        {
            DefaultAction = defaultAction;
            DefaultActionCanExecute = defaultCanExecute;
            AlternateAction = alternateAction;
            AlternateActionCanExecute = alternateCanExecute;
        }
        
        private bool defaultEnabled = true;

        /// <summary>
        /// Update the state of the RelayCommand, which updates all the properties of the button
        /// </summary>
        public bool IsDefaultState {
            get => defaultEnabled;
            set
            {
                SetProperty(ref defaultEnabled, value, nameof(IsDefaultState));
                base.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// The action to perform while <see cref="IsDefaultState"/> == true
        /// </summary>
        public Action DefaultAction { get; init; }

        /// <summary>
        /// The action to perform while <see cref="IsDefaultState"/> == false
        /// </summary>
        public Action AlternateAction { get; init; }

        /// <summary>
        /// Determine if the <see cref="DefaultAction"/> can execute
        /// </summary>
        public Func<bool> DefaultActionCanExecute { get; init; }

        /// <summary>
        /// Determine if the <see cref="AlternateAction"/> can execute
        /// </summary>
        public Func<bool> AlternateActionCanExecute { get; init; }

        /// <summary>
        /// Tooltip to display while <see cref="IsDefaultState"/> == true
        /// </summary>
        public string DefaultTooltip { get => base.ButtonDefinition.ToolTipTextProvider.DefaultText; set => base.ButtonDefinition.ToolTipTextProvider.DefaultText = value; }

        /// <summary>
        /// Tooltip to display while <see cref="IsDefaultState"/> == false
        /// </summary>
        public string AlternateToolTip { get => base.ButtonDefinition.ToolTipTextProvider.AlternateText; set => base.ButtonDefinition.ToolTipTextProvider.AlternateText = value; }

        /// <summary>
        /// Button Text  to display while <see cref="IsDefaultState"/> == true
        /// </summary>
        public string DefaultButtonText { get => base.ButtonDefinition.ButtonTextProvider.DefaultText; set => base.ButtonDefinition.ButtonTextProvider.DefaultText = value; }
        /// <summary>
        /// Button Text  to display while <see cref="IsDefaultState"/> == false
        /// </summary>
        public string AlternateButtonText { get => base.ButtonDefinition.ButtonTextProvider.AlternateText; set => base.ButtonDefinition.ButtonTextProvider.AlternateText = value; }

        /// <inheritdoc/>
        public override bool CanExecute(object parameter)
        {
            return IsDefaultState ? DefaultActionCanExecute() : AlternateActionCanExecute();
        }

        /// <inheritdoc/>
        public override void Execute(object parameter)
        {
            if (IsDefaultState)
                DefaultAction();
            else
                AlternateAction();
        }

        /// <summary>
        /// Evaluate the <see cref="DefaultActionCanExecute" /> function and set the result to <see cref="IsDefaultState"/>
        /// </summary>
        /// <param name="sender">This is part of the signature only to allow subscription to events. This value is not used.</param>
        /// <param name="e">This is part of the signature only to allow subscription to events. This value is not used.</param>
        public virtual void UpdateState(object sender = null, EventArgs e = null)
        {
            IsDefaultState = DefaultActionCanExecute?.Invoke() ?? true;
        }
    }
}
