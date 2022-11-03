using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects
{
    /// <summary>
    /// EventArgs object that provides the value that caused the event to be raised
    /// </summary>
    public class ValueEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new ValueEventArgs object with a null <see cref="Value"/>
        /// </summary>
        public ValueEventArgs() { }

        /// <summary>
        /// Create a new ValueEventArgs object with the specified <see cref="Value"/>
        /// </summary>
        public ValueEventArgs(object value) { Value = value; }

        /// <summary>
        /// The value that was sent which resulted in the event being raised
        /// </summary>
        public object Value { get; init; }
    }

    /// <inheritdoc cref="ValueEventArgs"/>
    public class ValueEventArgs<T> : ValueEventArgs
    {
        /// <inheritdoc cref="ValueEventArgs.ValueEventArgs()"/>
        public ValueEventArgs() { }

        /// <inheritdoc cref="ValueEventArgs.ValueEventArgs(object)"/>
        public ValueEventArgs(T value) { Value = value; }

        /// <inheritdoc cref="ValueEventArgs.Value"/>
        new public T Value { get => (T)base.Value; init => base.Value = value; }
    }
}
