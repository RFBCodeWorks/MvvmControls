using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RFBCodeWorks.Mvvm.Primitives
{
    /// <summary>
    /// Abstract Base class all of the Abstract*Command classes are derived from that stores common functionality between the implementations
    /// </summary>
    /// <remarks>Explicitly implements <see cref="ICommand"/></remarks> methods. Derived classes should provide their own implementation.
    public abstract class CommandBase : ObservableObject, ICommand, CommunityToolkit.Mvvm.Input.IRelayCommand, IRelayCommand
    {
        /// <summary> Static method that can be used as the default Func{T, bool}</summary>
        /// <returns><see langword="true"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReturnTrue<T>(T ignoredParameter) => true;

        /// <summary> Static method that can be used as the default Func{bool}</summary>
        /// <returns><see langword="true"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReturnTrue() => true;

        /// <summary>
        /// Subscribe (or not) to the <see cref="CommandManager.RequerySuggested"/> event
        /// </summary>
        /// <param name="subscribeToCommandManager"><inheritdoc cref="SubscribeToCommandManager" path="*"/></param>
        protected CommandBase(bool subscribeToCommandManager = true)
        {
            SubscribeToCommandManager = subscribeToCommandManager;
        }

        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged;

        private static readonly INotifyArgs SubscribedArgs = new(nameof(SubscribeToCommandManager));

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
                if (value == SubscribeToCommandManagerField) return;
                OnPropertyChanging(SubscribedArgs);
                if (value)
                    CommandManager.RequerySuggested += NotifyCanExecuteChanged;
                else
                    CommandManager.RequerySuggested -= NotifyCanExecuteChanged;
                SubscribeToCommandManagerField = value;
                OnPropertyChanged(SubscribedArgs);
            }
        }
        private bool SubscribeToCommandManagerField;

        /// <inheritdoc cref="IRelayCommand.NotifyCanExecuteChanged(object, EventArgs)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NotifyCanExecuteChanged(object sender, EventArgs e) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        /// <inheritdoc cref="CommunityToolkit.Mvvm.Input.IRelayCommand.NotifyCanExecuteChanged"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        bool ICommand.CanExecute(object parameter) => true;
        void ICommand.Execute(object parameter) { }
    }
}
