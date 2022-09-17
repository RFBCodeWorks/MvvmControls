using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.MVVMObjects
{
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

        /// <inheritdoc/>
        public virtual string ToolTip
        {
            get => toolTip;
            set => base.SetProperty(ref toolTip, value, nameof(ToolTip));
        }
        private string toolTip;


        /// <inheritdoc/>
        public virtual string ButtonText
        {
            get { return ButtonTextField; }
            set { SetProperty(ref ButtonTextField, value, nameof(ButtonText)); }
        }
        private string ButtonTextField;


        /// <inheritdoc/>
        public virtual System.Windows.Visibility Visibility
        {
            get { return VisibilityField; }
            set { SetProperty(ref VisibilityField, value, nameof(Visibility)); }
        }
        private System.Windows.Visibility  VisibilityField;


        /// <summary>
        /// <inheritdoc cref="ICommand.CanExecuteChanged"/>
        /// </summary>
        public virtual event EventHandler CanExecuteChanged;

        /// <inheritdoc cref="ICommand.CanExecute(object)"/>
        public virtual bool CanExecute(object parameter) => true;

        /// <inheritdoc cref="ICommand.Execute(object)"/>
        public abstract void Execute(object parameter);

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event
        /// </summary>
        public virtual void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());
    }

}
