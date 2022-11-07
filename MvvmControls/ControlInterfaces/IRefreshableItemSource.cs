using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.MvvmControls.ControlInterfaces
{
    /// <summary>
    /// Represents an <see cref="ItemSourceDefinition{T, E}"/> that is an array of type T, 
    /// whose collection can be refreshed on demand via the <see cref="RefreshFunc"/>
    /// </summary>
    /// <inheritdoc cref="ItemSourceDefinition{T, E}"/>
    public interface IRefreshableItemSource<T> : IItemSource<T, T[]>
    { 

        /// <summary>
        /// A Function that will be invoked by the Refresh() method to refresh the ItemSource
        /// </summary>
        public Func<T[]> RefreshFunc { get; init; }

        /// <summary>
        /// Function to determine if the collection can refresh
        /// </summary>
        public Func<bool> CanRefresh { get; init; }

        /// <summary>
        /// A RelayCommand that calls the <see cref="Refresh()"/> method
        /// </summary>
        public IButtonDefinition RefreshCommand { get; }


#if NETCOREAPP3_0_OR_GREATER

        /// <summary>
        /// Check if the <see cref="RefreshFunc"/> is not null
        /// </summary>
        /// <returns>TRUE if GetItemsFunc is not null, FALSE if it is null</returns>
        protected bool CanRefreshDefaultFunc() => RefreshFunc != null;

        /// <summary>
        /// Update the ItemSource
        /// </summary>
        public virtual void Refresh()
        {
            if (RefreshFunc != null)
                ItemSource = RefreshFunc();
        }

        /// <summary>
        /// Public EventHandler method to allow triggering the refresh via another object's event
        /// </summary>
        public virtual void Refresh(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// Public EventHandler to allow triggering the refresh via a routed event
        /// </summary>
        public virtual void Refresh(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
#else

        /// <summary>
        /// Update the ItemSource
        /// </summary>
        public void Refresh();
        /// <summary>
        /// Public EventHandler method to allow triggering the refresh via another object's event
        /// </summary>
        public void Refresh(object sender, EventArgs e);

        /// <summary>
        /// Public EventHandler to allow triggering the refresh via a routed event
        /// </summary>
        public void Refresh(object sender, RoutedEventArgs e);

#endif

    }
}
