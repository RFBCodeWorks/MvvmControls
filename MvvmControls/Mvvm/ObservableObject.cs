using System.ComponentModel;
using BaseObj = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace RFBCodeWorks.Mvvm
{
    /// <inheritdoc cref="BaseObj"/>
    /// <remarks>This class is directly derived from <see cref="BaseObj"/> to localize it to this namespace.</remarks>
    public abstract class ObservableObject : BaseObj
    { 
        /// <summary>
        /// Event Args for PropertyChangedEvent that indicates all properties have changed / are changing.
        /// </summary>
        public static INotifyArgs INotifyAllProperties => INotifyArgs.Empty;

        /// <summary>
        /// Raise <see cref="BaseObj.PropertyChanging"/> to notify that all properties are changing.
        /// </summary>
        protected void OnPropertyChanging() => base.OnPropertyChanging(INotifyArgs.Empty);

        /// <summary>
        /// Raise <see cref="BaseObj.PropertyChanged"/> to notify that all properties have been updated.
        /// </summary>
        protected void OnPropertyChanged() => base.OnPropertyChanged(INotifyArgs.Empty);

        /// <inheritdoc cref="BaseObj.OnPropertyChanging(string?)"/>
        new protected void OnPropertyChanging(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                base.OnPropertyChanging(INotifyArgs.Empty);
            }
            else
            {
                base.OnPropertyChanging(propertyName);
            }
        }

        /// <inheritdoc cref="BaseObj.OnPropertyChanged(string?)"/>
        new protected void OnPropertyChanged(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                base.OnPropertyChanged(INotifyArgs.Empty);
            }
            else
            {
                base.OnPropertyChanged(propertyName);
            }
        }
    }
}
