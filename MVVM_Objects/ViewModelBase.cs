using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMObjects
{
    /// <summary>
    /// Abstract Base class for ViewModels. Inherits <see cref="ObservableObject"/>
    /// </summary>
    public abstract class ViewModelBase : ObservableObject, IDisposable
    {
        
        /// <summary>
        /// The static <see cref="RelayCommandFactory"/> provided to the viewmodels. If not set by consumer, provides <see cref="RelayCommandFactory.Factory"/>
        /// </summary>
        public static RelayCommandFactory DefaultRelayCommandFactory { get; set; } = RelayCommandFactory.Factory;

        /// <summary>
        /// Create a new instance of the ViewModelBase
        /// </summary>
        public ViewModelBase() : base() { }
        
        /// <summary>
        /// Create a new instance of the ViewModelBase
        /// </summary>
        /// <param name="parent"></param>
        public ViewModelBase(ViewModelBase parent) : base() 
        {
            ParentViewModel = parent;
        }

        /// <summary>
        /// Reference back to the parent view model - Can be used by the messaging bus in this case the view model is being provided as a property within another ViewModel.
        /// </summary>
        public ViewModelBase ParentViewModel { get; init; }

        /// <summary>
        /// <inheritdoc cref="MVVMObjects.RelayCommandFactory"/>
        /// </summary>
        protected RelayCommandFactory RelayCommandFactory
        {
            get => factory ?? DefaultRelayCommandFactory;
            set => factory = value;
        }
        private RelayCommandFactory factory;
        private bool disposedValue;

        /// <summary>
        /// Overridable Property that controls can have their 'IsReadOnly' property bound to
        /// </summary>
        /// <remarks>Default returns FALSE</remarks>
        public virtual bool IsReadOnly { get => _IsReadOnly; protected set => SetProperty(ref _IsReadOnly, value); }
        private bool _IsReadOnly;
        
        /// <summary>
        /// Method that allows public setting of the <see cref="IsReadOnly"/> property if needed
        /// </summary>
        /// <param name="isReadOnly"></param>
        public void SetReadOnly(bool isReadOnly) => IsReadOnly = isReadOnly;


        /// <inheritdoc cref="IDisposable.Dispose"/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Dispose_UnsubscribeFromEvents();
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Dispose_ManagedObjects();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        /// <summary>
        /// This method is called by the <see cref="ViewModelBase.Dispose(bool)"/> method. Override for unsubscribing of the events.
        /// </summary>
        protected virtual void Dispose_UnsubscribeFromEvents() { }
        
        /// <summary>
        /// This method is called by the <see cref="ViewModelBase.Dispose(bool)"/> method. Override to dispose any managed objects.
        /// </summary>
        protected virtual void Dispose_ManagedObjects() { }


        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ViewModelBase()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
