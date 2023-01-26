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
    /// Abstract base class for IRelayCommand objects that accept a parameter of a specified type
    /// </summary>
    /// <inheritdoc cref="CommunityToolkit.Mvvm.Input.RelayCommand{T}"/>
    public abstract class AbstractCommand<T> : CommandBase, IRelayCommand<T>
    {

        #region < Static Methods >

        /// <summary>
        /// Static method to be used in place of Func{bool} methods to always return true
        /// </summary>
        /// <returns><see langword="true"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReturnTrue(T parameter) => true;

        /// <summary>
        /// Evaluates the <paramref name="parameter"/> to check if it is a valid object of type <typeparamref name="T"/>
        /// </summary>
        /// <param name="parameter">The input parameter.</param>
        /// <returns>
        /// If it is a valid parameter: returns the <paramref name="parameter"/> as <typeparamref name="T"/> - this may include returning <see langword="null"/> if that is valid and the <paramref name="parameter"/> is null.
        /// <br/> If not valid: throw an exception.
        /// </returns>
        /// <exception cref="ArgumentNullException"/> // throws if null object but type is not nullable (such as value type)
        /// <exception cref="ArgumentException"/> // throws if object is incompatible type
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfInvalidParameter(object parameter)
        {
            //adapted from: 
            //https://github.com/CommunityToolkit/dotnet/blob/e8969781afe537ea41a964a15b4ccfee32e095df/src/CommunityToolkit.Mvvm/Input/RelayCommand%7BT%7D.cs#L125

            if (parameter is null && default(T) is null) return default;
            if (parameter is T arg) return arg;
            if (parameter is null) throw new ArgumentNullException($"Parameter \"{nameof(parameter)}\" (object) must not be null, as the command type requires an argument of type {typeof(T)}.", nameof(parameter));
            throw new ArgumentException($"Parameter \"{nameof(parameter)}\" (object) cannot be of type {parameter.GetType()}, as the command type requires an argument of type {typeof(T)}.", nameof(parameter));
        }

        #endregion

        #region < Constructors >

        /// <inheritdoc/>
        protected AbstractCommand() : this(true) { }

        /// <summary> Initialize the object </summary>
        /// <inheritdoc/>
        protected AbstractCommand(bool subscribeToCommandManager) : base(subscribeToCommandManager) { }

        #endregion

        /// <inheritdoc/>
        public abstract bool CanExecute(T parameter);

        /// <inheritdoc/>
        public abstract void Execute(T parameter);

        #region < Interface Implementations >

        bool ICommand.CanExecute(object parameter)
        {
            // Special case a null value for a value type argument type.
            // This ensures that no exceptions are thrown during initialization.
            //https://github.com/CommunityToolkit/dotnet/blob/e8969781afe537ea41a964a15b4ccfee32e095df/src/CommunityToolkit.Mvvm/Input/RelayCommand%7BT%7D.cs#L87
            if (parameter is null && default(T) is not null)
            {
                return false;
            }
            return CanExecute(ThrowIfInvalidParameter(parameter));
        }

        void ICommand.Execute(object parameter) => Execute(ThrowIfInvalidParameter(parameter));
        
        #endregion
    }
}
