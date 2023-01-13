using System.ComponentModel;
using BaseObj = Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject;

namespace RFBCodeWorks.Mvvvm
{
    /// <inheritdoc cref="BaseObj"/>
    public abstract class ObservableObject: BaseObj 
    {
        /// <summary>
        /// Event Args for PropertyChangedEvent that indicates all properties have changed.
        /// </summary>
        public static readonly PropertyChangedEventArgs AllPropertiesChangedArgs = new(string.Empty);

        /// <summary>
        /// Event Args for PropertyChangingEvent that indicates all properties have changed.
        /// </summary>
        public static readonly PropertyChangingEventArgs AllPropertiesChangingArgs = new(string.Empty);

        /// <inheritdoc cref="BaseObj.OnPropertyChanging(string?)"/>
        new protected void OnPropertyChanging(string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                base.OnPropertyChanging(AllPropertiesChangingArgs);
            }
            else
            {
                base.OnPropertyChanging(propertyName);
            }
        }

        /// <inheritdoc cref="BaseObj.OnPropertyChanged(string?)"/>
        new protected void OnPropertyChanged(string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                base.OnPropertyChanged(AllPropertiesChangedArgs);
            }
            else
            {
                base.OnPropertyChanged(propertyName);
            }
        }
    }
}
