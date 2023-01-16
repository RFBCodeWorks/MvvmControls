using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm.Primitives
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
        public override bool IsEnabled { get => base.IsEnabled; set { } }

        /// <summary> The method through which the abstract base object implements <see cref="ICommand.CanExecute(object)"/> </summary>
        /// <inheritdoc cref="ICommand.CanExecute(object)"/>
        public abstract bool CanExecute();

        /// <summary> The method through which the abstract base object implements <see cref="ICommand.Execute(object)"/> </summary>
        public abstract void Execute();

        /// <inheritdoc/>
        public abstract void NotifyCanExecuteChanged();

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged(object sender, EventArgs e) => this.NotifyCanExecuteChanged();

        #region < ICommand Explicit Implementation >

        /// <inheritdoc/>
        public abstract event EventHandler CanExecuteChanged;
        
        void ICommand.Execute(object parameter) => this.Execute();
        bool ICommand.CanExecute(object parameter)
        {
            base.IsEnabled = CanExecute();
            return base.IsEnabled;
        }
        
        #endregion
    }

}
