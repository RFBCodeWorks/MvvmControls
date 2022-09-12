using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMObjects
{
    /// <summary>
    /// Base class for an <see cref="ObservableCollection{T}"/> of objects of some time
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionViewModel<T> : ViewModelBase
    {
        /// <summary>
        /// The collection of objects to be observed
        /// </summary>
        public ObservableCollection<T> Collection { get; set; }
    }
}
