using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// RelayCommand object that will execute an action that does not require any parameters
    /// </summary>
    public class RelayCommand : Primitives.AbstractCommand
    {

        /// <inheritdoc cref="RelayCommand.RelayCommand(Action, Func{ bool}, Action{ Exception})"/>
        public RelayCommand(Action execute)
            : this(execute, ReturnTrue) { }

        /// <inheritdoc cref="RelayCommand.RelayCommand(Action, Func{ bool}, Action{ Exception})"/>
        /// <exception cref="ArgumentNullException"/>
        public RelayCommand(Action execute, Func<bool> canExecute) : base(true)
        {
            ExecuteAction = execute ?? throw new ArgumentNullException(nameof(execute));
            CanExecuteFunction = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <inheritdoc cref="RelayCommand.RelayCommand(Action, Func{ bool}, Action{ Exception})"/>
        public RelayCommand(Action execute, Action<Exception> errorHandler)
            : this(execute, ReturnTrue, errorHandler) { }

        /// <summary>
        /// Create a new RelayCommand
        /// </summary>
        /// <inheritdoc cref="Primitives.AbstractCommand.AbstractCommand(bool)"/>
        /// <inheritdoc cref="Microsoft.Toolkit.Mvvm.Input.RelayCommand.RelayCommand(Action, Func{bool})"/>
        /// <exception cref="ArgumentNullException"/>
        public RelayCommand(Action execute, Func<bool> canExecute, Action<Exception> errorHandler) : base(true)
        {
            ExecuteAction = execute ?? throw new ArgumentNullException(nameof(execute));
            CanExecuteFunction = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            ErrorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        }

        private readonly Action ExecuteAction;
        private readonly Func<bool> CanExecuteFunction;
        private readonly Action<Exception> ErrorHandler;

        /// <inheritdoc/>
        public sealed override bool CanExecute()
        {
            return CanExecuteFunction();
        }

        /// <inheritdoc/>
        public sealed override void Execute()
        {
            try
            {
                ExecuteAction();
            }
            catch (Exception e) when (ErrorHandler != null)
            {
                ErrorHandler(e);
            }
        }
    }
}
