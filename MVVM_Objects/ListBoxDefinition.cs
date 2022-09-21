using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RFBCodeWorks.MVVMObjects.BaseControlDefinitions;

namespace RFBCodeWorks.MVVMObjects
{
    #region < ListBox Definitions >

    /// <summary>
    /// A Simple ListBox/ListView Definition that only ListBox/ListView the enumerated type
    /// </summary>
    /// <inheritdoc cref="ListBoxDefinition{T, E, V}"/>
    public class ListBoxDefinition<T> : ListBoxDefinition<T, object> { }

    /// <summary>
    /// A generic definition for a ListBox/ListView whose ItemSource is any IEnumerable object, that expects a SelectedValue of a specific type
    /// </summary>
    /// <inheritdoc cref="ListBoxDefinition{T, E, V}"/>
    public class ListBoxDefinition<T, V> : ListBoxDefinition<T, ConcurrentObservableCollection<T>, V> { }

    /// <summary>
    /// A definition for a ListBox/ListView where the ItemSource is a specific type of <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <inheritdoc cref="ListBoxDefinition{T, E, V}"/>
    public class ListBoxDefinition2<T, E> : ListBoxDefinition<T, E, object> where E : IList<T> { }

    /// <summary>
    /// A generic definition for a ListBox/ListView control
    /// </summary>
    /// <inheritdoc cref="SelectorDefinition{T, E, V}"/>
    public class ListBoxDefinition<T, E, V> : SelectorDefinition<T, E, V> where E : IList<T>
    {

        /// <summary>
        /// Check if the ItemSource has any items
        /// </summary>
        /// <returns>TRUE if the ItemSource has atleast 1 item, otherwise false</returns>
        public bool HasItems => ItemSource?.Any() ?? false;

        /// <summary>
        /// Check if the <see cref="SelectionMode"/> is not <see cref="SelectionMode.Single"/>
        /// </summary>
        public bool IsMultiSelect => SelectionMode != SelectionMode.Single;

        /// <summary>
        /// Allows binding to the SelectedItems property of a listbox via the <see cref="Behaviors.Base.MultiItemSelectionBehavior{T}"/> behavior
        /// </summary>
        public IList<T> SelectedItems
        {
            get { return SelectedItemsField; }
            set { SetProperty(ref SelectedItemsField, value, nameof(SelectedItems)); }
        }
        private IList<T> SelectedItemsField;


        /// <inheritdoc cref="ListBox.SelectionMode" path="*"/>
        public SelectionMode SelectionMode
        {
            get { return SelectionModeField; }
            set { SetProperty(ref SelectionModeField, value, nameof(SelectionMode)); }
        }
        private SelectionMode SelectionModeField = SelectionMode.Single;

    }

    #endregion

    #region < Refreshable >

    /// <inheritdoc cref="RefreshableListBoxDefinition{T, V}"/>
    public class RefreshableListBoxDefinition<T> : RefreshableListBoxDefinition<T, object> { }

    /// <summary>
    /// A ComboBoxDefinition whose collection is an array of <typeparamref name="T"/> objects, that can be refreshed on demand via the <see cref="RefreshFunc"/>
    /// </summary>
    /// <inheritdoc cref="ListBoxDefinition{T, E, V}"/>
    public class RefreshableListBoxDefinition<T, V> : ListBoxDefinition<T, T[], V>, IRefreshableItemSource<T>
    {
        /// <summary>
        /// Create a new RefreshableComboBox object
        /// </summary>
        public RefreshableListBoxDefinition()
        {
            RefreshCommand = new RelayCommand(Refresh, () => CanRefresh());
        }

        /// <inheritdoc/>
        public Func<T[]> RefreshFunc { get; init; }

        /// <inheritdoc/>
        public Func<bool> CanRefresh { get; init; }

        /// <inheritdoc/>
        public IButtonDefinition RefreshCommand { get; }

        /// <summary>
        /// Check if the <see cref="RefreshFunc"/> is null and return the opposite
        /// </summary>
        /// <returns>TRUE if not null, FALSE is null</returns>
        protected bool HasRefreshFunc()
        {
            return RefreshFunc != null;
        }

        /// <inheritdoc/>
        public virtual void Refresh()
        {
            if (CanRefresh())
                ItemSource = RefreshFunc();
        }

        /// <inheritdoc/>
        public void Refresh(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <inheritdoc/>
        public void Refresh(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

    }

    #endregion

    #region < Concurrent >

    /// <inheritdoc cref="ConcurrentComboBoxDefinition{T, V}"/>
    public class ConcurrentListBoxDefinition<T> : ConcurrentListBoxDefinition<T, object> { }

    /// <summary>
    /// A ComboBox that uses a thread-safe list to update its contents
    /// </summary>
    /// <inheritdoc cref="ListBoxDefinition{T, E, V}"/>
    public class ConcurrentListBoxDefinition<T, V> : ListBoxDefinition<T, ConcurrentObservableCollection<T>, V>, IConcurrentItemSource<T>
    {
        /// <summary>
        /// Initialize the definition
        /// </summary>
        public ConcurrentListBoxDefinition()
        {
            ItemSource = new();
            ClearCommand = new RelayCommand(Clear, () =>  HasItems);
            RemoveSelectedItemsCommand = new RelayCommand(RemoveSelectedItems, () => SelectedItems?.Any() ?? false);
        }

        /// <summary>
        /// A RelayCommand that executes the <see cref="Clear"/> method
        /// </summary>
        /// <remarks>
        /// Default implementation will call the <see cref="Clear"/> action, which then performs <see cref="RemoveItemAction"/> against all items in the collection.
        /// <br/> Specify a new RelayCommand if other functionality is required.
        /// </remarks>
        public IButtonDefinition ClearCommand { get; init; }

        /// <summary>
        /// An action to perform when removing an item from the list
        /// </summary>
        /// <remarks>
        /// This action will be performed whenever an item is removed from the collection. <br/>
        /// If Clearing the collection, the action will be performed on each item, and the list will be updated as the actions complete.
        /// </remarks>
        public Action<T> RemoveItemAction { get; init; }

        /// <summary>
        /// A RelayCommand that calls the <see cref="RemoveSelectedItems()"/> method, which will run the <see cref="RemoveItemAction"/> against all items within the collection
        /// </summary>
        public IButtonDefinition RemoveSelectedItemsCommand { get; init; }

        /// <summary>
        /// Passes the SelectedItems into the RemoveAll method
        /// </summary>
        public virtual void RemoveSelectedItems()
        {
            RemoveAll(SelectedItems);
        }

        #region < IConcurrentItemSource Implementation >


        /// <inheritdoc/>
        public new ConcurrentObservableCollection<T> ItemSource { get => base.ItemSource; init => base.ItemSource = value; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public virtual void Add(T item)
        {
            ItemSource.Add(item);
        }

        public virtual void AddRange(params T[] items)
        {
            ItemSource.AddRange(items);
        }

        public virtual void AddRange(IEnumerable<T> items)
        {
            ItemSource.AddRange(items);
        }

        public virtual void Clear()
        {
            RemoveAll(ItemSource.ToArray());
            SelectedItems = new List<T> { };
        }

        public virtual void Remove(T item)
        {
            if (ItemSource.Contains(item))
            {
                ItemSource.Remove(item);
                RemoveItemAction?.Invoke(item);
            }
        }

        public virtual void RemoveAt(int index)
        {
            if (ItemSource.Count > index)
            {
                RemoveItemAction?.Invoke(ItemSource[index]);
                ItemSource.RemoveAt(index);
            }
        }

        public virtual void RemoveAll(params T[] items)
        {
            RemoveAll(items);
        }

        public virtual void RemoveAll(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Remove(item);
            }
        }

        public virtual void RemoveAll(Predicate<T> match)
        {
            var Items = ItemSource.Where(o => match(o)).ToArray();
            foreach (var item in Items)
            {
                Remove(item);
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion

    }

    #endregion

}
