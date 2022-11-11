using System;

namespace RFBCodeWorks.MvvmControls.Specialized
{
    /// <summary>
    /// This object is comprised of two RelayCommands, but which one is active depends on the state of the object
    /// </summary>
    public class TwoStateButton : AbstractTwoStateButton, IButtonDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public TwoStateButton() : base()
        {
            System.Windows.Input.CommandManager.RequerySuggested += CanExecuteChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultAction"></param>
        /// <param name="defaultCanExecute"></param>
        /// <param name="alternateAction"></param>
        /// <param name="alternateCanExecute"></param>
        public TwoStateButton(Action defaultAction, Func<bool> defaultCanExecute, Action alternateAction, Func<bool> alternateCanExecute) : this()
        {
            DefaultAction = defaultAction;
            DefaultActionCanExecute = defaultCanExecute;
            AlternateAction = alternateAction;
            AlternateActionCanExecute = alternateCanExecute;
        }

        private bool defaultEnabled = true;

        /// <inheritdoc/>
        public override event EventHandler CanExecuteChanged;

        /// <summary>
        /// The action to perform while <see cref="AbstractTwoStateButton.IsDefaultState"/> == true
        /// </summary>
        public Action DefaultAction { get; init; }

        /// <summary>
        /// The action to perform while <see cref="AbstractTwoStateButton.IsDefaultState"/> == false
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void DefaultExecute()
        {
            DefaultAction?.Invoke();
        }

        protected override void AlternateExecute()
        {
            AlternateAction?.Invoke();
        }

        protected override bool DefaultCanExecute()
        {
            return DefaultAction is not null && (DefaultActionCanExecute?.Invoke() ?? false);
        }

        protected override bool AlternateCanExecute()
        {
            return AlternateAction is not null && (AlternateActionCanExecute?.Invoke() ?? false);
        }

        public override void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, System.EventArgs.Empty);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
