using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects.ViewModels
{
    /// <summary>
    /// Base class for an <see cref="ObservableCollection{T}"/> of objects of some type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionViewModel<T> : ViewModelBase
    {
        /// <summary>
        /// The thread-safe collection of objects to be observed
        /// </summary>
        public ConcurrentObservableCollection<T> Collection { get; set; }

        /// <summary>
        /// The currently selected Item
        /// </summary>
        public T SelectedItem
        {
            get { return SelectedItemField; }
            set { SetProperty(ref SelectedItemField, value, nameof(SelectedItem)); }
        }
        private T SelectedItemField;



        /// <summary>
        /// The currently selected items - likely requires 
        /// </summary>
        public IList<T> SelectedItems
        {
            get { return SelectedItemsField; }
            set { SetProperty(ref SelectedItemsField, value, nameof(SelectedItems)); }
        }
        private IList<T> SelectedItemsField;

    }
}
