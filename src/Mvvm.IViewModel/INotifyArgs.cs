using System.Collections.Generic;
using System.ComponentModel;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Represents a pair of arguments that can be used for the <see cref="System.ComponentModel.INotifyPropertyChanged"/> interface.
    /// <br/> Implicitly converts to:
    /// <br/> - <see cref="System.ComponentModel.PropertyChangedEventArgs"/>
    /// <br/> - <see cref="System.ComponentModel.PropertyChangingEventArgs"/>
    /// </summary>
    public sealed class INotifyArgs
    {
        /// <summary>
        /// The Empty args signal to <see cref="System.ComponentModel.INotifyPropertyChanged"/> that ALL properties are changing.
        /// </summary>
        public static readonly INotifyArgs Empty = new INotifyArgs();
        private static readonly Dictionary<string, INotifyArgs> RegisteredArgs = new Dictionary<string, INotifyArgs>();

        private INotifyArgs()
        {
            PropertyName = string.Empty;
            PropertyChangedArgs = new PropertyChangedEventArgs(string.Empty);
            PropertyChangingArgs = new PropertyChangingEventArgs(string.Empty);
        }

        /// <inheritdoc cref="INotifyArgs.INotifyArgs(string, bool)"/>
        public INotifyArgs(string propertyName) : this(propertyName, false) { }


        /// <summary>
        /// Create a new pair of event args
        /// </summary>
        /// <param name="propertyName">The property name that is changing / has changed</param>
        /// <param name="registerArgs"></param>
        /////When TRUE, registers the args to recall later via the <see cref="GetArgs(string)"/> method</param>
        private INotifyArgs(string propertyName, bool registerArgs)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                PropertyName = Empty.PropertyName;
                PropertyChangedArgs = Empty.PropertyChangedArgs;
                PropertyChangingArgs = Empty.PropertyChangingArgs;
            }
            else if (registerArgs)
            {
                lock (RegisteredArgs)
                    if (RegisteredArgs.TryGetValue(propertyName, out var value))
                    {
                        this.PropertyName = value.PropertyName;
                        this.PropertyChangedArgs = value.PropertyChangedArgs;
                        this.PropertyChangingArgs = value.PropertyChangingArgs;
                    }
                    else
                    {
                        PropertyName = propertyName;
                        PropertyChangedArgs = new PropertyChangedEventArgs(propertyName);
                        PropertyChangingArgs = new PropertyChangingEventArgs(propertyName);
                        RegisteredArgs.Add(propertyName, this);
                    }
            }
            else
            {
                PropertyName = propertyName;
                PropertyChangedArgs = new PropertyChangedEventArgs(propertyName);
                PropertyChangingArgs = new PropertyChangingEventArgs(propertyName);
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public readonly string PropertyName;
        public readonly PropertyChangedEventArgs PropertyChangedArgs;
        public readonly PropertyChangingEventArgs PropertyChangingArgs;
        public static implicit operator INotifyArgs(string propertyName) => string.IsNullOrWhiteSpace(propertyName) ? Empty : new INotifyArgs(propertyName);
        public static implicit operator PropertyChangedEventArgs(INotifyArgs args) => args.PropertyChangedArgs;
        public static implicit operator PropertyChangingEventArgs(INotifyArgs args) => args.PropertyChangingArgs;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        ///// <summary>
        ///// Get an <see cref="INotifyArgs"/> object from the collection of registered objects. 
        ///// <br/> If a matching object has not been created yet, create it, register it, then return it.
        ///// </summary>
        ///// <param name="propertyName">The name of the property to generate arguments for</param>
        ///// <returns>An <see cref="INotifyArgs"/> object for the specified <paramref name="propertyName"/></returns>
        //public static INotifyArgs GetArgs(string propertyName)
        //{
        //    if (string.IsNullOrWhiteSpace(propertyName)) return Empty;
        //    lock (RegisteredArgs)
        //    {
        //        if (RegisteredArgs.TryGetValue(propertyName, out var value))
        //            return value;
        //        else
        //        {
        //            value = new INotifyArgs(propertyName, false);
        //            RegisteredArgs.Add(propertyName, value);
        //            return value;

        //        }
        //    }
        //}

        ///// <summary>
        ///// Searches the registered arguments. If a matching argument is located, removes it from the collection (allowing for garbage collection)
        ///// </summary>
        ///// <param name="propertyName"></param>
        //public static void DisposeArgs(string propertyName)
        //{
        //    if (propertyName is null) return;
        //    lock(RegisteredArgs)
        //        RegisteredArgs?.Remove(propertyName);
        //}
    }
}
