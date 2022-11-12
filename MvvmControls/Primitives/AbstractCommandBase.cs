using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace RFBCodeWorks.MvvmControls.Primitives
{
    /// <summary>
    /// Abstract base class for ICommand objects
    /// </summary>
    /// <remarks>
    /// Does NOT implement any interfaces, but provides the event logic for derived classes to utilize
    /// </remarks>
    public abstract class AbstractCommandBase : ObservableObject
    {
        /// <summary>
        /// Static method to be used in place of Func{bool} methods to always return true
        /// </summary>
        /// <returns><see langword="true"/></returns>
        public static bool ReturnTrue() => true;

        /// <summary>
        /// Static method to be used in place of Func{bool} methods to always return true
        /// </summary>
        /// <returns><see langword="true"/></returns>
        public static bool ReturnTrue<T>(T parameter) => true;

        /// <summary>
        /// Throw an exception if the parameter is not of the expected type, otherwise return the parameter
        /// </summary>
        /// <typeparam name="T">the expected type</typeparam>
        /// <param name="parameter">the object to assess</param>
        /// <param name="returnDefaultIfNull">Set false to throw if the parameter is null</param>
        /// <returns>Casts parameter to <typeparamref name="T"/>, or returns default if parameter is null</returns>
        public static T ThrowExceptionIfInvalidType<T>(object parameter, bool returnDefaultIfNull = true)
        {
            if (parameter is null && returnDefaultIfNull) return default;
            if (parameter is null) throw new ArgumentNullException(nameof(parameter));
            if (parameter is not T) throw new ArgumentException($"{nameof(parameter)} is not of type {typeof(T)}");
            return (T)parameter;
        }

        /// <summary>
        /// Evaluate a collection of tasks to determine if any exist within the collection
        /// </summary>
        /// <returns>TRUE if the collection is empty, otherwise false</returns>
        protected static bool ReturnTrueIfNoTasksRunning(IEnumerable<Task> tasks) => !tasks.Any();

        /// <summary>
        /// Initialize the Command base, subscribing the the <see cref="CommandManager.RequerySuggested"/> event
        /// </summary>
        protected AbstractCommandBase() : this(true) { }

        /// <summary>
        /// Initialize the Command base, optionally subscribing the <see cref="CommandManager.RequerySuggested"/> event
        /// </summary>
        /// <param name="subscribeToCommandManager"><inheritdoc cref="SubscribeToCommandManager" path="*" /></param>
        protected AbstractCommandBase(bool subscribeToCommandManager) : base()
        {
            SubscribeToCommandManager = subscribeToCommandManager;
        }

        /// <inheritdoc cref="ICommand.CanExecuteChanged"/>
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

        /// <inheritdoc cref="IRelayCommand.NotifyCanExecuteChanged(object, EventArgs)"/>
        public void NotifyCanExecuteChanged(object sender, EventArgs e)
        {
            NotifyCanExecuteChanged();
        }

        /// <inheritdoc cref="Microsoft.Toolkit.Mvvm.Input.RelayCommand.NotifyCanExecuteChanged"/>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
