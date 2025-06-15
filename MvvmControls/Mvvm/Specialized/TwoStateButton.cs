using System;

namespace RFBCodeWorks.Mvvm.Specialized
{
    /// <summary>
    /// This object is comprised of two RelayCommands, but which one is active depends on the state of the object
    /// </summary>
    public class TwoStateButton : Primitives.AbstractTwoStateButton, IButtonDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public TwoStateButton() : base() { }

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

        /// <summary>
        /// The action to perform while <see cref="Primitives.AbstractTwoStateButton.IsDefaultState"/> == true
        /// </summary>
        public Action DefaultAction { get; init; }

        /// <summary>
        /// The action to perform while <see cref="Primitives.AbstractTwoStateButton.IsDefaultState"/> == false
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
