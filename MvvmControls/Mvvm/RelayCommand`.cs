using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm
{

    /// <summary>
    /// RelayCommand object that will execute an action that does not require any parameters
    /// </summary>
    /// <inheritdoc/>
    public sealed class RelayCommand<T> : Primitives.AbstractCommand<T>, IRelayCommand<T>
    {

        /// <inheritdoc cref="RelayCommand{T}.RelayCommand(Action{T}, Func{T, bool}, Action{T, Exception})"/>
        public RelayCommand(Action<T> execute) 
            : this(execute, ReturnTrue) { }

        /// <inheritdoc cref="RelayCommand{T}.RelayCommand(Action{T}, Func{T, bool}, Action{T, Exception})"/>
        /// <exception cref="ArgumentNullException"/>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute) : base(true)
        {
            ExecuteAction = execute ?? throw new ArgumentNullException(nameof(execute));
            CanExecuteFunction = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <inheritdoc cref="RelayCommand{T}.RelayCommand(Action{T}, Func{T, bool}, Action{T, Exception})"/>
        /// <exception cref="ArgumentNullException"/>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute, Action<Exception> errorHandler) : base(true)
        {
            ExecuteAction = execute ?? throw new ArgumentNullException(nameof(execute));
            CanExecuteFunction = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            if (errorHandler is null) throw new ArgumentNullException(nameof(errorHandler));
            ErrorHandler = (o, e) => errorHandler(e);
        }

        /// <inheritdoc cref="RelayCommand{T}.RelayCommand(Action{T}, Func{T, bool}, Action{T, Exception})"/>
        public RelayCommand(Action<T> execute, Action<Exception> errorHandler) 
            : this(execute, ReturnTrue, errorHandler) { }

        /// <inheritdoc cref="RelayCommand{T}.RelayCommand(Action{T}, Func{T, bool}, Action{T, Exception})"/>
        public RelayCommand(Action<T> execute, Action<T, Exception> errorHandler) 
            : this(execute, ReturnTrue, errorHandler) { }

        /// <summary>
        /// Create a new RelayCommand
        /// </summary>
        /// <inheritdoc cref="Primitives.AbstractCommand.AbstractCommand(bool)"/>
        /// <inheritdoc cref="Microsoft.Toolkit.Mvvm.Input.RelayCommand.RelayCommand(Action, Func{bool})"/>
        /// <exception cref="ArgumentNullException"/>
        public RelayCommand(Action<T> execute, Func<T,bool> canExecute, Action<T,Exception> errorHandler) : base(true)
        {
            ExecuteAction = execute ?? throw new ArgumentNullException(nameof(execute));
            CanExecuteFunction = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            ErrorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        }

        private readonly Action<T> ExecuteAction; 
        private readonly Func<T, bool> CanExecuteFunction;
        private readonly Action<T, Exception> ErrorHandler;

        /// <inheritdoc/>
        public sealed override bool CanExecute(T parameter)
        {
            return CanExecuteFunction(parameter);
        }

        /// <inheritdoc/>
        public sealed override void Execute(T parameter)
        {
            try
            {
                ExecuteAction(parameter);
            }
            catch (Exception e) when (ErrorHandler != null)
            {
                ErrorHandler(parameter, e);
            }
        }
    }
}
