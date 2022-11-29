using System.ComponentModel;
using BaseObj = Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject;

namespace RFBCodeWorks.MvvmControls
{
    /// <inheritdoc cref="BaseObj"/>
    public abstract class ObservableObject: BaseObj 
    {
        /// <summary> <see cref="PropertyChangedEventArgs"/> that provide <see cref="string.Empty"/> </summary>
        public static readonly PropertyChangedEventArgs AllPropertiesChangedArgs = new(string.Empty);
        
        /// <summary> <see cref="PropertyChangingEventArgs"/> that provide <see cref="string.Empty"/> </summary>
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
