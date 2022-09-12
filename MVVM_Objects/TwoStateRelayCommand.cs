using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects
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

        private bool defaultEnabled;

        public bool IsDefaultState {
            get => defaultEnabled;
            set
            {
                SetProperty(ref defaultEnabled, value, nameof(IsDefaultState));
                base.NotifyCanExecuteChanged();
            }
        }

        public Action DefaultAction { get; init; }
        public Action AlternateAction { get; init; }

        public Func<bool> DefaultActionCanExecute { get; init; }
        public Func<bool> AlternateActionCanExecute { get; init; }

        public string DefaultTooltip { get => base.ButtonDefinition.ToolTipTextProvider.DefaultText; set => base.ButtonDefinition.ToolTipTextProvider.DefaultText = value; }
        public string AlternateToolTip { get => base.ButtonDefinition.ToolTipTextProvider.AlternateText; set => base.ButtonDefinition.ToolTipTextProvider.AlternateText = value; }

        public string DefaultButtonText { get => base.ButtonDefinition.ButtonTextProvider.DefaultText; set => base.ButtonDefinition.ButtonTextProvider.DefaultText = value; }
        public string AlternateButtonText { get => base.ButtonDefinition.ButtonTextProvider.AlternateText; set => base.ButtonDefinition.ButtonTextProvider.AlternateText = value; }

        public override bool CanExecute(object parameter)
        {
            return IsDefaultState ? DefaultActionCanExecute() : AlternateActionCanExecute();
        }

        public override void Execute(object parameter)
        {
            if (IsDefaultState)
                DefaultAction();
            else
                AlternateAction();
        }
    }
}
