using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MvvmControls.ControlInterfaces
{
    /// <summary>
    /// Represents an <see cref="ItemSourceDefinition{T, E}"/> whose collection can be modified via Add/Remove/Clear commands
    /// </summary>
    public interface IUpdatableItemSource<T, E>: INotifyPropertyChanged, IItemSource<T, E>
        where E : IList<T>
    {

        /// <summary>
        /// Clear the collection and notify the UI
        /// </summary>
        public void Clear();

        /// <summary>
        /// Add an item to the collection and notify the UI
        /// </summary>
        public void Add(T item);

        /// <summary>
        /// Add items to the collection and notify the UI
        /// </summary>
        public void AddRange(params T[] items);

        /// <summary>
        /// Add items to the collection and notify the UI
        /// </summary>
        public void AddRange(IEnumerable<T> items);

        /// <summary>
        /// Remove an item from the collection and notify the UI
        /// </summary>
        public void Remove(T item);

        /// <summary>
        /// Remove an item from the collection by its index  and notify the UI
        /// </summary>
        public void RemoveAt(int index);

        /// <summary>
        /// Remove a set of items from the collection and notify the UI
        /// </summary>
        public void RemoveAll(params T[] items);

        /// <summary>
        /// Remove any matching items from the collection and notify the UI
        /// </summary>
        public void RemoveAll(Predicate<T> match);

    }
}
