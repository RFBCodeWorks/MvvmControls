using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RFBCodeWorks.MvvmControls.Primitives
{
    /// <summary>
    /// Abstract base object that inherits from the following:
    /// <br/> - <see cref="ControlBase"/>
    /// <br/> - <see cref="IRelayCommand"/>
    /// <br/> - <see cref="ICommand"/> - This is explicitly implemented via the protected abstract methods
    /// <br/> - <see cref="IToolTipProvider"/>
    /// <br/> - <see cref="INotifyPropertyChanged"/>
    /// <br/> - <see cref="IButtonDefinition"/>
    /// </summary>
    public abstract class AbstractButtonDefinition : ControlBase, IButtonDefinition, ICommand
    {
        /// <summary> Static method that can be used as the default Func{bool} for <see cref="ICommand.CanExecute(object)"/> </summary>
        /// <returns><see langword="true"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool ReturnTrue() => true;

        /// <summary> Static method that can be used as the default Func{bool} for <see cref="ICommand.CanExecute(object)"/> </summary>
        /// <returns><see langword="true"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool ReturnTrue<T>(T ignoredParameter) => true;

        /// <inheritdoc/>
        public virtual string DisplayText
        {
            get { return ButtonTextField; }
            set { SetProperty(ref ButtonTextField, value, nameof(DisplayText)); }
        }
        private string ButtonTextField;

        /// <summary>
        /// Returns the result of the last <see cref="ICommand.CanExecute(object)"/> call.
        /// <br/> The set method has no effect.
        /// </summary>
        public override bool IsEnabled { get => canExecute; set { } }
        private bool canExecute = true;

        /// <summary> The method through which the abstract base object implements <see cref="ICommand.CanExecute(object)"/> </summary>
        /// <inheritdoc cref="ICommand.CanExecute(object)"/>
        protected abstract bool CanExecute(object parameter);

        /// <summary> The method through which the abstract base object implements <see cref="ICommand.Execute(object)"/> </summary>
        protected abstract void Execute(object parameter);

        /// <inheritdoc/>
        public abstract void NotifyCanExecuteChanged();

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged(object sender, EventArgs e) => this.NotifyCanExecuteChanged();

        #region < ICommand Explicit Implementation >

        /// <inheritdoc/>
        public abstract event EventHandler CanExecuteChanged;
        
        void ICommand.Execute(object parameter) => this.Execute(parameter);
        bool ICommand.CanExecute(object parameter)
        {
            _ = SetProperty(ref canExecute, CanExecute(parameter), nameof(IsEnabled));
            return canExecute;
        }
        
        #endregion
    }

}
