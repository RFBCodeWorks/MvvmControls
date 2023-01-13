using System.ComponentModel;

namespace RFBCodeWorks.Mvvvm
{
    /// <summary>
    /// Interface implemented by the <see cref="ViewModelBase"/> class
    /// </summary>
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
