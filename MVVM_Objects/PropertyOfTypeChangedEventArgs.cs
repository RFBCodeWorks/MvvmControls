using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects
{
    /// <summary>
    /// Event Handler Delegate
    /// </summary>
    public delegate void PropertyOfTypeChangedEventHandler<T>(object sender, PropertyOfTypeChangedEventArgs<T> e);

    /// <summary>
    /// Event Args that are produced when a property of some type is modified.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyOfTypeChangedEventArgs<T> : PropertyChangedEventArgs
    {

        /// <summary>
        /// Declare a new set of args
        /// </summary>
        /// <param name="oldValue"><inheritdoc cref="OldValue" path="*"/></param>
        /// <param name="newValue"><inheritdoc cref="NewValue" path="*"/></param>
        /// <param name="propertyName"><inheritdoc cref="PropertyChangedEventArgs.PropertyName" path="*"/></param>
        public PropertyOfTypeChangedEventArgs(T oldValue, T newValue, string propertyName): base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        private readonly static Type propType = typeof(T);

        /// <summary>
        /// The object type
        /// </summary>
        public Type Type => propType; 

        /// <summary>
        /// The old value
        /// </summary>
        public T OldValue { get; init; }

        /// <summary>
        /// The new Value
        /// </summary>
        public T NewValue { get; init; }

        /// <summary>
        /// Evaluate if the two values are the same
        /// </summary>
        public bool IsSame => OldValue?.Equals(NewValue) ?? NewValue is null;

    }
}
