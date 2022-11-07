using Microsoft.Toolkit.Mvvm.Input;
using RFBCodeWorks.MvvmControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using RelayCmd = Microsoft.Toolkit.Mvvm.Input.RelayCommand;
using RFBCodeWorks.MvvmControls.ControlInterfaces;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// A Variant of <see cref="RelayCmd"/> that allows inheritance but also allows for accepting a parameter of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : AbstractButtonDefinition, IButtonDefinition, ICommand, IRelayCommand
    {
        #region < Constructors >

        /// <inheritdoc cref="RelayCommand{T}.RelayCommand(Action{T}, Func{T, bool})"/>
        public RelayCommand(Action<T> execute)
        {
            ExecuteAction = execute;
            CanExecuteFunction = null;
            CommandManager.RequerySuggested += base.NotifyCanExecuteChanged; 
        }

        /// <summary>
        /// Create a new ObjectCommand
        /// </summary>
        /// <param name="execute">The execute to perform against the <see cref="ObjectViewModel{T}.ObjectModel"/></param>
        /// <param name="canExecute">Some Functions that returns TRUE/FALSE if the <see cref="Execute(object)"/> should be callable.</param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            ExecuteAction = execute;
            CanExecuteFunction = canExecute;
        }

        #endregion

        #region < Properties >

        /// <summary>
        /// Action to execute via the <see cref="Execute(object)"/> method. <br/> This can be left null if the <see cref="Execute(object)"/> method is overridden.
        /// </summary>
        protected Action<T> ExecuteAction { get; init; }

        /// <summary>
        /// The function that is called by the base <see cref="CanExecute(object)"/> method.
        /// </summary>
        protected Func<T, bool> CanExecuteFunction { get; init; }

        #endregion

        #region < Methods >

        /// <remarks>
        /// Gets queried after CanExecuteChanged is raised. 
        /// </remarks>
        /// <inheritdoc cref="ICommand.CanExecute(object)"/>
        public override bool CanExecute(object parameter)
        {
            if (CanExecuteFunction is null) return true; // If a custom execute is not specified, return true
            if (parameter is T | parameter is null)
                return CanExecuteFunction((T)parameter);
            else
                throw new ArgumentException($"Invalid object type passed to ICommand object.\n Expected: {typeof(T)}\nReceived: {parameter.GetType()}");

        }

        /// <inheritdoc cref="ICommand.Execute(object)"/>
        /// <remarks>
        /// If the <paramref name="parameter"/> is not of type <typeparamref name="T"/>, then an <see cref="ArgumentException"/> will be thrown. <br/>
        /// Does not throw if parameter is null.
        /// </remarks>
        public override void Execute(object parameter)
        {
            if (parameter is T | parameter is null)
                ExecuteAction((T)parameter);
            else
                throw new ArgumentException($"Invalid object type passed to ICommand object.\n Expected: {typeof(T)}\nReceived: {parameter.GetType()}");
        }

        #endregion
    }

    /// <summary>
    /// A Localization of the <see cref="RelayCmd"/> class that allows inheritance and extends functionality with <see cref="IButtonDefinition"/>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="RelayCmd" path="*"/>
    /// </remarks>
    public class RelayCommand : AbstractButtonDefinition, IButtonDefinition, IRelayCommand
    {
        /// <inheritdoc cref="RelayCmd.RelayCommand(Action)"/>
        public RelayCommand(Action execute) : this(execute, ReturnTrue) { }

        /// <inheritdoc cref="RelayCmd.RelayCommand(Action, Func{bool})"/>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            ExecuteAction = execute;
            CanExecuteFunction = canExecute;
        }

        private static bool ReturnTrue() => true;

        /// <summary>
        /// Action to execute via the <see cref="Execute(object)"/> method. <br/> This can be left null if the <see cref="Execute(object)"/> method is overridden.
        /// </summary>
        protected Action ExecuteAction { get; init; }

        /// <summary>
        /// The function that is called by the base <see cref="CanExecute(object)"/> method.
        /// </summary>
        protected Func<bool> CanExecuteFunction { get; init; }

        #region < IRelayCommand >

        /// <remarks>
        /// Gets queried after CanExecuteChanged is raised. 
        /// </remarks>
        /// <inheritdoc cref="ICommand.CanExecute(object)"/>
        public override bool CanExecute(object parameter)
        {
            return CanExecuteFunction();
        }

        /// <inheritdoc cref="ICommand.Execute(object)"/>
        public override void Execute(object parameter)
        {
            //if (CanExecute(parameter))
                ExecuteAction();
            //else
            //    NotifyCanExecuteChanged();
        }

        #endregion
    }

}
