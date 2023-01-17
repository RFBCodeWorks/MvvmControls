using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
    public abstract class AbstractAsyncButtonDefinition<T> : ControlBase, IButtonDefinition, ICommand
    {
        /// <summary> Static method that can be used as the default Func{bool} for <see cref="ICommand.CanExecute(object)"/> </summary>
        /// <returns><see langword="true"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool ReturnTrue(T ignoredParameter) => true;

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

        /// <inheritdoc cref="ICommand.CanExecute(object)"/>
        public abstract bool CanExecute(T parameter);

        ///<summary> Start a task that accepts an input of type <typeparamref name="T"/></summary>
        /// <inheritdoc cref="CommunityToolkit.Mvvm.Input.IAsyncRelayCommand{T}.ExecuteAsync(T)"/>
        public abstract Task ExecuteAsync(T parameter);

        /// <inheritdoc cref="CommunityToolkit.Mvvm.Input.IAsyncRelayCommand.Cancel"/>
        public abstract void Cancel();

        /// <inheritdoc/>
        public abstract void NotifyCanExecuteChanged();

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged(object sender, EventArgs e) => this.NotifyCanExecuteChanged();

        #region < ICommand Explicit Implementation >

        /// <inheritdoc/>
        public abstract event EventHandler CanExecuteChanged;
        
        async void ICommand.Execute(object parameter) => await this.ExecuteAsync(AbstractCommand<T>.ThrowIfInvalidParameter(parameter));
        bool ICommand.CanExecute(object parameter)
        {
            // Special case a null value for a value type argument type.
            // This ensures that no exceptions are thrown during initialization.
            //https://github.com/CommunityToolkit/dotnet/blob/e8969781afe537ea41a964a15b4ccfee32e095df/src/CommunityToolkit.Mvvm/Input/RelayCommand%7BT%7D.cs#L87
            if (parameter is null && default(T) is not null)
            {
                return false;
            }
            base.IsEnabled = CanExecute(AbstractCommand<T>.ThrowIfInvalidParameter(parameter));
            return base.IsEnabled;
        }
        
        #endregion
    }

}
