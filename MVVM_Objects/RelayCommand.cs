using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using RelayCmd = Microsoft.Toolkit.Mvvm.Input.RelayCommand;

namespace RFBCodeWorks.MVVMObjects
{
    /// <summary>
    /// A Variant of <see cref="RelayCmd"/> that allows inheritance but also allows for accepting a parameter of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : ObservableObject, IButtonDefinition, ICommand, IRelayCommand
    {
        #region < Constructors >

        /// <inheritdoc cref="RelayCommand{T}.RelayCommand(Action{T}, Func{T, bool})"/>
        public RelayCommand(Action<T> execute)
        {
            ExecuteAction = execute;
            CanExecuteFunction = null;
            CommandManager.RequerySuggested += this.CanExecuteChanged; 
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

        #region < Events >

        /// <inheritdoc cref="ICommand.CanExecuteChanged"/>
        public event EventHandler CanExecuteChanged;

        #endregion

        #region < Properties >

        /// <inheritdoc cref="IToolTipProvider.ToolTip"/>
        public string ToolTip
        {
            get => toolTip;
            set
            {
                base.SetProperty(ref toolTip, value, nameof(ToolTip));
                //toolTip = value; 
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ToolTip))); 
            }
        }
        private string toolTip;

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
        /// Gets queried after <see cref="CanExecuteChanged"/> is raised. 
        /// </remarks>
        /// <inheritdoc cref="ICommand.CanExecute(object)"/>
        public virtual bool CanExecute(object parameter)
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
        public virtual void Execute(object parameter)
        {
            if (parameter is T | parameter is null)
                ExecuteAction((T)parameter);
            else
                throw new ArgumentException($"Invalid object type passed to ICommand object.\n Expected: {typeof(T)}\nReceived: {parameter.GetType()}");
        }

        /// <summary>
        /// Raise the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public virtual void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public void NotifyCanExecuteChanged(object sender, EventArgs e) => this.NotifyCanExecuteChanged();

        #endregion
    }

    /// <summary>
    /// A Localization of the <see cref="RelayCmd"/> class that allows inheritance and extends functionality with <see cref="IButtonDefinition"/>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="RelayCmd" path="*"/>
    /// </remarks>
    public class RelayCommand : ObservableObject, IButtonDefinition, IRelayCommand
    {
        /// <inheritdoc cref="RelayCmd.RelayCommand(Action)"/>
        public RelayCommand(Action execute) { RelayCmd = new RelayCmd(execute); }

        /// <inheritdoc cref="RelayCmd.RelayCommand(Action, Func{bool})"/>
        public RelayCommand(Action execute, Func<bool> canExecute) { RelayCmd = new RelayCmd(execute, canExecute); }

        /// <summary>
        /// Initialize a new RelayCommand object that utilized the provided <paramref name="relayCommand"/>
        /// </summary>
        /// <param name="relayCommand"></param>
        public RelayCommand(RelayCmd relayCommand) { RelayCmd = relayCommand; }

        /// <summary>
        /// The underlying <see cref="Microsoft.Toolkit.Mvvm.Input.RelayCommand"/>
        /// </summary>
        protected RelayCmd RelayCmd { get; }

        /// <inheritdoc cref="IToolTipProvider.ToolTip"/>
        public string ToolTip
        {
            get => toolTip;
            set => base.SetProperty(ref toolTip, value, nameof(ToolTip));
        }
        private string toolTip;
 
        #region < IRelayCommand >

        /// <inheritdoc cref="ICommand.CanExecuteChanged"/>
        public virtual event EventHandler CanExecuteChanged
        {
            add { ((ICommand)RelayCmd).CanExecuteChanged += value; }
            remove { ((ICommand)RelayCmd).CanExecuteChanged -= value; }
        }

        /// <inheritdoc cref="IRelayCommand.NotifyCanExecuteChanged"/>
        public virtual void NotifyCanExecuteChanged() => ((IRelayCommand)RelayCmd).NotifyCanExecuteChanged();

        /// <inheritdoc cref="ICommand.CanExecute(object)"/>
        public virtual bool CanExecute(object parameter) => ((ICommand)RelayCmd).CanExecute(parameter);

        /// <inheritdoc cref="ICommand.Execute(object)"/>        
        public virtual void Execute(object parameter) => ((ICommand)RelayCmd).Execute(parameter);

        #endregion
    }

}
