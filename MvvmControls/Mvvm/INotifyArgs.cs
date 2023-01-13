using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Represents a pair of arguments that can be used for the <see cref="System.ComponentModel.INotifyPropertyChanged"/> interface.
    /// <br/> Implicitly converts to:
    /// <br/> - <see cref="System.ComponentModel.PropertyChangedEventArgs"/>
    /// <br/> - <see cref="System.ComponentModel.PropertyChangingEventArgs"/>
    /// </summary>
    public class INotifyArgs
    {
        /// <summary>
        /// The Empty args signal to <see cref="System.ComponentModel.INotifyPropertyChanged"/> that ALL properties are changing.
        /// </summary>
        public static readonly INotifyArgs Empty = new INotifyArgs(string.Empty);

        /// <summary>
        /// Create a new pair of event args
        /// </summary>
        /// <param name="propertyName">The property name that is changing / has changed</param>
        public INotifyArgs(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                PropertyName = Empty.PropertyName;
                PropertyChangedArgs = Empty.PropertyChangedArgs;
                PropertyChangingArgs = Empty.PropertyChangingArgs;
            }
            else
            {
                PropertyName = propertyName;
                PropertyChangedArgs = new(propertyName);
                PropertyChangingArgs = new(propertyName);
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public readonly string PropertyName;
        public readonly System.ComponentModel.PropertyChangedEventArgs PropertyChangedArgs;
        public readonly System.ComponentModel.PropertyChangingEventArgs PropertyChangingArgs;
        public static implicit operator INotifyArgs(string propertyName) => string.IsNullOrWhiteSpace(propertyName) ? Empty : new INotifyArgs(propertyName);
        public static implicit operator System.ComponentModel.PropertyChangedEventArgs(INotifyArgs args) => args.PropertyChangedArgs;
        public static implicit operator System.ComponentModel.PropertyChangingEventArgs(INotifyArgs args) => args.PropertyChangingArgs;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
