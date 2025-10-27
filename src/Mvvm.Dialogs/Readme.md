## RFBCodeWorks.Mvvm.DialogServices

This dll relies on MvvmDialogs.IDialogService from https://github.com/FantasticFiasco/mvvm-dialogs

This dll provides the following:

DialogServiceLocator / IDialogServiceLocator 
- Provides a way to locate an appropriate DialogService for a given owner window
- Allows registering / unregistering dialogservices to a collection. 
- Allows specifying a default IDialogServiceLocator to select the appropriate DialogService from the collection.
- If a customer locator is not specified, this library will use a default implementation that selects the first registered IDialogService.
- The extension methods will use this locator to open a window if those methods are called.

DialogService - Derived from MvvmDialogs.DialogService
- Overrides the FindOwner method to allow traversing up a RFBCodeWorks.Mvvm.IViewModel chain until an owner window is located.
- All instances are registered automatically to the DialogServiceLocator.RegisteredServices collection.

DialogServiceExtensions
- Various extension methods to avoid having to dive into the full namespace hierarchy of MvvmDialogs to generate the various objects, such as OpenFileDialog

FileFilter
- This class allows creation of file filters for the Open/Save file dialogs.

MessageBoxSettings - Derived from MvvmDialogs.FrameworkDialogs.MessageBox.MessageBoxSettings
- Provides several static methods for easier creation of common-use dialogs
- Provides additional constructors
- Provides a virtual 'Show(IViewModel)' dialog that can be used to show the MessageBox
