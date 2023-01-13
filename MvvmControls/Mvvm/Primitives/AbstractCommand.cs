using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvvm.Primitives
{
    /// <summary>
    /// Abstract base class for IRelayCommand objects that do not require parameters
    /// </summary>
    /// <inheritdoc cref="Microsoft.Toolkit.Mvvm.Input.RelayCommand"/>
    public abstract class AbstractCommand : ObservableObject, IRelayCommand
    {

        /// <summary>
        /// Static method to be used in place of Func{bool} methods to always return true
        /// </summary>
        /// <returns><see langword="true"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReturnTrue() => true;

        #region < Constructors >

        /// <inheritdoc/>
        protected AbstractCommand() : this(true) { }

        /// <summary>
        /// Initialize the <see cref="AbstractAsyncCommand"/> object
        /// </summary>
        /// <param name="subscribeToCommandManager"><inheritdoc cref="SubscribeToCommandManager" path="*"/></param>
        protected AbstractCommand(bool subscribeToCommandManager) : base()
        {
            SubscribeToCommandManager = subscribeToCommandManager;
        }

        #endregion

        #region < Properties and Events >

        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// While <see langword="true"/>, <see cref="CanExecuteChanged"/> will be raised as determined by the <see cref="CommandManager.RequerySuggested"/> event.
        /// <br/><br/>Set this to <see langword="false"/> to only allow explicitly raising the CanExecuteChanged event.
        /// </summary>
        /// <remarks>
        /// This is set <see langword="true"/> by default in the constructor.
        /// </remarks>
        public bool SubscribeToCommandManager
        {
            get => SubscribeToCommandManagerField;
            set
            {
                if (value)
                    CommandManager.RequerySuggested += CanExecuteChanged;
                else
                    CommandManager.RequerySuggested -= CanExecuteChanged;
                SubscribeToCommandManagerField = value;
            }
        }
        private bool SubscribeToCommandManagerField;

        #endregion

        /// <inheritdoc/>
        public abstract bool CanExecute();
        
        /// <inheritdoc/>
        public abstract void Execute();

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NotifyCanExecuteChanged(object sender, EventArgs e) => NotifyCanExecuteChanged();

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #region < Interface Implementations >

        bool ICommand.CanExecute(object parameter) => CanExecute();
        void ICommand.Execute(object parameter) => Execute();

        #endregion

    }
}
