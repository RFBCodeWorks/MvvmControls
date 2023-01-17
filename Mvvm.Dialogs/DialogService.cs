using MvvmDialogs;
using System.Windows;

namespace RFBCodeWorks.Mvvm.DialogServices
{
    /// <summary>
    /// <inheritdoc/>
    /// <para/>If the viewmodel is an <see cref="IViewModel"/>, it will traverse up the <see cref="IViewModel.ParentViewModel"/> tree until a valid view is located.
    /// </summary>
    public class DialogService : MvvmDialogs.DialogService
    {

        /// <summary>
        /// Create the Dialog Service, and register it to the <see cref="DialogServiceLocator"/>
        /// </summary>
        /// <inheritdoc cref="MvvmDialogs.DialogService.DialogService(MvvmDialogs.DialogFactories.IDialogFactory?, MvvmDialogs.DialogTypeLocators.IDialogTypeLocator?, MvvmDialogs.FrameworkDialogs.IFrameworkDialogFactory?)" />
        public DialogService(
            MvvmDialogs.DialogTypeLocators.IDialogTypeLocator dialogTypeLocator = null,
            MvvmDialogs.DialogFactories.IDialogFactory dialogFactory = null,
            MvvmDialogs.FrameworkDialogs.IFrameworkDialogFactory frameworkDialogFactory = null)
            : base(dialogFactory, dialogTypeLocator, frameworkDialogFactory)
        {
            DialogServiceLocator.RegisterDialogService(this);
        }

        /// <inheritdoc cref="DialogService.DialogService(MvvmDialogs.DialogTypeLocators.IDialogTypeLocator, MvvmDialogs.DialogFactories.IDialogFactory, MvvmDialogs.FrameworkDialogs.IFrameworkDialogFactory, string)"/>
        public DialogService(string name = null) : this(null, null, null) { Name = name ?? string.Empty; }

        /// <inheritdoc cref="DialogService.DialogService(MvvmDialogs.DialogTypeLocators.IDialogTypeLocator, MvvmDialogs.DialogFactories.IDialogFactory, MvvmDialogs.FrameworkDialogs.IFrameworkDialogFactory)"/>
        /// <param name="name"><inheritdoc cref="Name" path="*"/></param>
        /// <param name="dialogFactory"/><param name="dialogTypeLocator"/><param name="frameworkDialogFactory"/>
        public DialogService(
            MvvmDialogs.DialogTypeLocators.IDialogTypeLocator dialogTypeLocator = null,
            MvvmDialogs.DialogFactories.IDialogFactory dialogFactory = null,
            MvvmDialogs.FrameworkDialogs.IFrameworkDialogFactory frameworkDialogFactory = null,
            string name = null) 
            : this(dialogTypeLocator, dialogFactory, frameworkDialogFactory)
        {
            Name = name ?? string.Empty;
        }

        /// <summary>
        /// A name for the DialogService to be identified by. 
        /// <br/>This property is optional, but is present to allow for potentially easier integration with custom <see cref="IDialogServiceLocator"/> objects.
        /// </summary>
        public string Name { get; }


        /// <inheritdoc/>
        protected override Window FindOwnerWindow(System.ComponentModel.INotifyPropertyChanged viewModel)
        {
            if (viewModel is IViewModel vm)
            {
                try
                {
                    return base.FindOwnerWindow(viewModel);
                }
                catch (ViewNotRegisteredException) when (vm.ParentViewModel != null)
                {
                    return FindOwnerWindow(vm.ParentViewModel); // Recurse up the ParentViewModels until the registered view is found
                }
            }
            else
                return base.FindOwnerWindow(viewModel);
        }
    }
}
