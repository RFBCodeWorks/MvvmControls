using MvvmDialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RFBCodeWorks.Mvvm.DialogServices
{
    /// <summary>
    /// <inheritdoc cref="DefaultLocator"/>
    /// </summary>
    public class DialogServiceLocator : IDialogServiceLocator
    {
        
        /// <summary>
        /// The constructor used for derived classes.
        /// </summary>
        protected DialogServiceLocator() { }

        /// <summary>
        /// Gets the Default DialogServiceLocator built in with the library.
        /// </summary>
        private static DialogServiceLocator Default { get; } = new DialogServices.DialogServiceLocator();

        /// <summary>
        /// Represents a service that can be used to locate an appropriate <see cref="IDialogService"/> 
        /// </summary>
        /// <remarks>
        /// This static property can only be set once! Subsequent attempts to set the value will result in an <see cref="System.InvalidOperationException"/> being thrown.
        /// <br/>If a custom DialogServiceLocator is required, best practice would be to create the DialogService and the Locator at program startup, then register the locator here.
        /// <br/>If this property is not set by the consumer, then this will default to using a default static locator provided by the library. The default provider will return the first IDialogService that was registered.
        /// </remarks>
        public static IDialogServiceLocator DefaultLocator
        {
            get => locator ?? Default;
            set
            {
                if (locator is null)
                    locator = value;
                else
                    throw new System.InvalidOperationException("The DialogServiceLocator.DefaultLocator property can only be set once, ideally at the start of the program. " +
                        "Setting to a new value during runtime could result in an inconsistent state or broken program, so this operation is prevented.");
            }
        }
        private static IDialogServiceLocator locator;
        private static List<MvvmDialogs.IDialogService> services = new List<MvvmDialogs.IDialogService>();

        /// <summary>
        /// The ReadOnly collection of registered Dialog Services
        /// </summary>
        public static IReadOnlyCollection<MvvmDialogs.IDialogService> RegisteredServices { get => services.AsReadOnly(); }
        
        /// <summary>
        /// Add a new <paramref name="service"/> to the collection of <see cref="RegisteredServices"/>
        /// </summary>
        /// <param name="service">The service to register</param>
        public static void RegisterDialogService(MvvmDialogs.IDialogService service)
        {
            if (service != null && !services.Contains(service))
                services.Add(service);
        }

        /// <summary>
        /// remove a <paramref name="service"/> from the collection of <see cref="RegisteredServices"/>
        /// </summary>
        /// <param name="service">The service to unregister</param>
        public static void UnregisterDialogService(MvvmDialogs.IDialogService service)
        {
            services.Remove(service);
        }


        /// <inheritdoc/>
        public virtual IDialogService GetDialogService(INotifyPropertyChanged ownerViewModel = null)
        {
            return services.FirstOrDefault() ?? throw new System.Exception("No MvvmDialogs.IDialogServices have been registered to the locator.");
        }

    }
}
