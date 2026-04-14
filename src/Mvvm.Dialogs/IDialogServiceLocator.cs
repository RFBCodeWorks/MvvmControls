using System.ComponentModel;

namespace RFBCodeWorks.Mvvm.DialogServices
{
    /// <summary>
    /// Interface that allows a class to provide a way to locate an <see cref="MvvmDialogs.IDialogService"/> that will be used to open up various types of windows
    /// </summary>
    public interface IDialogServiceLocator
    {
        /// <summary>
        /// Method that will return an appropriate DialogService for a given <paramref name="ownerViewModel"/>
        /// </summary>
        /// <param name="ownerViewModel">
        /// <inheritdoc cref="MvvmDialogs.IDialogService.Show(INotifyPropertyChanged, INotifyPropertyChanged)" path="/param[@name='ownerViewModel']" />
        /// </param>
        /// <returns>An <see cref="MvvmDialogs.IDialogService"/> object that can be used to open up a new window</returns>
        MvvmDialogs.IDialogService GetDialogService(INotifyPropertyChanged ownerViewModel);
    }
}
