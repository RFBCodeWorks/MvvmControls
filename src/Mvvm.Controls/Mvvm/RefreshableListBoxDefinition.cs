using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// An <see cref="IListBox"/> that can be refreshed.
    /// </summary>
    /// <inheritdoc cref="ListBoxDefinition{T, TList,  TSelectedValue}"/>
    public partial class RefreshableListBoxDefinition<T, TList, TSelectedValue> : RefreshableSelector<T, TList, TSelectedValue>, 
        IRefreshableItemSource, IRefreshableItemSource<T>, 
        IItemSource, IItemSource<T>,
        IListBox, IListBox<T>
        where TList : IList<T>
    {
        /// <summary>Create a new RefreshableListBox </summary>
        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{CancellationToken, Task{TList}}, Func{bool}, Action, Action, bool)"/>
        /// <param name="refresh">A function to invoke that will refresh the collection</param>
        /// <param name="canRefresh">a function that is used to determine if the collection can be refreshed</param>
        /// <param name="onCollectionChanged"/><param name="onSelectionChanged"/><param name="refreshOnFirstCollectionRequest"/>
        public RefreshableListBoxDefinition(Func<TList> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest)
        { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<TList> refresh) : this(refresh, null, null, null) { }

        /* Async CTORs */

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<Task<TList>> refreshAsync, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refreshAsync, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest)
        { }
    
        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<Task<TList>> refreshAsync) : this(refreshAsync, null, null, null) { }

        /* Cancellable CTORs */

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<CancellationToken, Task<TList>> refreshAsyncCancellable, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refreshAsyncCancellable, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest)
        { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<CancellationToken, Task<TList>> refreshAsyncCancellable) : this(refreshAsyncCancellable, null, null, null) { }

        /* Events */

        /// <summary>
        /// Occurs after the <see cref="SelectedItems"/> has been updated
        /// </summary>
        public event EventHandler SelectedItemsChanged;

        /// <inheritdoc/>
        public bool HasItems => Items?.Any() ?? false;

        /// <inheritdoc/>
        public bool HasItemsSelected => SelectedItems?.Any() ?? false;

        /// <summary>
        /// Check if the <see cref="SelectionMode"/> is not <see cref="System.Windows.Controls.SelectionMode.Single"/>
        /// </summary>
        public bool IsMultiSelect => SelectionMode != System.Windows.Controls.SelectionMode.Single;

        /// <summary>
        /// Allows binding to the SelectedItems property of a listbox via the <see cref="RFBCodeWorks.WPF.Behaviors.Base.MultiItemSelectionBehavior{T}"/> behavior
        /// </summary>
        [ObservableProperty]
        private IList<T> _selectedItems;

        /// <inheritdoc/>
        [ObservableProperty]
        private System.Windows.Controls.SelectionMode _selectionMode = System.Windows.Controls.SelectionMode.Single;
        
        IList IListBox.SelectedItems
        {
            get => (IList)SelectedItems;
            set
            {
                if (value is IList<T> list)
                {
                    SelectedItems = list;
                }
                else if (value is IReadOnlyList<T> rd)
                {
                    SelectedItems = rd.ToList();
                }
                else
                {
                    throw new ArgumentException($"IList passed to the setter is not an IList<{typeof(T)}>");
                }
            }
        }

        partial void OnSelectedItemsChanged(IList<T> value) => OnSelectedItemsChanged();

        /// <summary> Raises the SelectedItemsChanged event </summary>
        protected virtual void OnSelectedItemsChanged()
        {
            SelectedItemsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}"/>
    public sealed class RefreshableListBoxDefinition<T> : RefreshableListBoxDefinition<T, IList<T>, object>
    {
        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<IList<T>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<Task<IList<T>>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<CancellationToken, Task<IList<T>>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<IList<T>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<Task<IList<T>>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<CancellationToken, Task<IList<T>>> refresh) : base(refresh) { }
    }

    /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}"/>
    public sealed class RefreshableListBoxDefinition<T, TSelectedValue> : RefreshableListBoxDefinition<T, IList<T>, TSelectedValue>
    {
        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<IList<T>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<Task<IList<T>>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<CancellationToken, Task<IList<T>>> refresh, Func<bool> canRefresh = null, Action onCollectionChanged = null, Action onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<IList<T>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<Task<IList<T>>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool)"/>
        public RefreshableListBoxDefinition(Func<CancellationToken, Task<IList<T>>> refresh) : base(refresh) { }
    }
}
