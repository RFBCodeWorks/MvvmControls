using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RFBCodeWorks.MvvmControls.ControlInterfaces;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Abstract base object that inherits from the following:
    /// <br/> - <see cref="ObservableObject"/>
    /// <br/> - <see cref="IRelayCommand"/>
    /// <br/> - <see cref="ICommand"/>
    /// <br/> - <see cref="IToolTipProvider"/>
    /// <br/> - <see cref="INotifyPropertyChanged"/>
    /// <br/> - <see cref="IButtonDefinition"/>
    /// </summary>
    public abstract class AbstractButtonDefinition : BaseControlDefinition, IButtonDefinition
    {

        /// <inheritdoc/>
        public virtual string DisplayText
        {
            get { return ButtonTextField; }
            set { SetProperty(ref ButtonTextField, value, nameof(DisplayText)); }
        }
        private string ButtonTextField;

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

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged(object sender, EventArgs e) => this.NotifyCanExecuteChanged();
    }

}
