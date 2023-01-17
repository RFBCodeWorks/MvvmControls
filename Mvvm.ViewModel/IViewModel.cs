using System.ComponentModel;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Interface for ViewModels that may have an associated <see cref="ParentViewModel"/> for purposes of finding owner windows
    /// </summary>
    /// <remarks>Inherits from <see cref="INotifyPropertyChanged"/></remarks>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Reference to the ViewModel that owns this viewmodel.
        /// </summary>
        /// <remarks>
        /// This would be used by a DialogService to locate the viewmodel registered to the window's DataContext.
        /// </remarks>
        IViewModel ParentViewModel { get; }
    }
}
