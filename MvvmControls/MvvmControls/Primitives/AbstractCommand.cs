using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.MvvmControls.Primitives
{
    /// <summary>
    /// Abstract base class for IRelayCommand objects that do not require parameters
    /// </summary>
    /// <inheritdoc cref="Microsoft.Toolkit.Mvvm.Input.RelayCommand"/>
    public abstract class AbstractCommand : AbstractCommandBase, IRelayCommand
    {
        /// <inheritdoc/>
        protected AbstractCommand() : this(true) { }
        
        /// <inheritdoc/>
        protected AbstractCommand(bool subscribeToCommandManager) : base(subscribeToCommandManager) { }
        
        /// <inheritdoc/>
        public abstract bool CanExecute();
        
        /// <inheritdoc/>
        public abstract void Execute();
        
        bool ICommand.CanExecute(object parameter) => CanExecute();
        void ICommand.Execute(object parameter) => Execute();
    }

    /// <summary>
    /// Abstract base class for IRelayCommand objects that accept a parameter of a specified type
    /// </summary>
    /// <inheritdoc cref="Microsoft.Toolkit.Mvvm.Input.RelayCommand{T}"/>
    public abstract class AbstractCommand<T> : AbstractCommandBase, IRelayCommand<T>
    {
        /// <inheritdoc/>
        protected AbstractCommand() : this(true) { }
        
        /// <inheritdoc/>
        protected AbstractCommand(bool subscribeToCommandManager) : base(subscribeToCommandManager) { }

        /// <inheritdoc/>
        public abstract bool CanExecute(T parameter);
        
        /// <inheritdoc/>
        public abstract void Execute(T parameter);

        bool ICommand.CanExecute(object parameter) => CanExecute(ThrowExceptionIfInvalidType<T>(parameter));
        void ICommand.Execute(object parameter) => Execute(ThrowExceptionIfInvalidType<T>(parameter));

        //bool IRelayCommand.CanExecute() => ((ICommand)this).CanExecute(null);
        //void IRelayCommand.Execute() => throw new NotImplementedException("RFBCodeWorks.MvvmControls.IRelayCommand.Execute() is not supported unless explicitly implemented by the dervied class");
    }
}
