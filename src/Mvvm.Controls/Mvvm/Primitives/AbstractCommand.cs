using RFBCodeWorks.Mvvm.Input;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm.Primitives
{
    /// <summary>
    /// Abstract base class for IRelayCommand objects that do not require parameters
    /// </summary>
    /// <inheritdoc cref="CommunityToolkit.Mvvm.Input.RelayCommand"/>
    public abstract class AbstractCommand : CommandBase, IRelayCommand
    {

        /// <inheritdoc/>
        protected AbstractCommand() : this(true) { }

        /// <summary> Initialize the object </summary>
        /// <inheritdoc/>
        protected AbstractCommand(bool subscribeToCommandManager) : base(subscribeToCommandManager) { }

        /// <inheritdoc/>
        public abstract bool CanExecute();
        
        /// <inheritdoc/>
        public abstract void Execute();

        #region < Interface Implementations >

        bool ICommand.CanExecute(object parameter) => CanExecute();
        void ICommand.Execute(object parameter) => Execute();

        #endregion

    }
}
