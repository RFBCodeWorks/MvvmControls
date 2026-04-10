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
        /*
         * CTORs that are shorthand for others
         */

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool, Action)"/>
        public RefreshableListBoxDefinition(Func<TList> refresh) : this(refresh: refresh, canRefresh: ReturnTrue) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool, Action)"/>
        public RefreshableListBoxDefinition(Func<Task<TList>> refreshAsync) : this(refreshAsync: refreshAsync, canRefresh: ReturnTrue) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool, Action)"/>
        public RefreshableListBoxDefinition(Func<CancellationToken, Task<TList>> refreshAsyncCancellable) : this(refreshAsyncCancellable : refreshAsyncCancellable, canRefresh: ReturnTrue) { }


        /// <summary>
        /// Create a new RefreshableListBox that refreshes synchronously 
        /// </summary>
        /// <inheritdoc cref="RefreshableSelector{T, TList, TSelectedValue}.RefreshableSelector(Func{CancellationToken, Task{TList}}, Func{bool}, Action, Action, bool)"/>
        /// <param name="refresh">A function to invoke that will refresh the collection</param>
        /// <param name="canRefresh">a function that is used to determine if the collection can be refreshed</param>
        /// <param name="onCollectionChanged"/><param name="onSelectionChanged"/><param name="refreshOnFirstCollectionRequest"/>
        /// <param name="onSelectedItemsChanged">An <see cref="Action"/> to be invoked when the <see cref="SelectedItems"/> property is changed</param>
        public RefreshableListBoxDefinition(
            Func<TList> refresh, 
            Func<bool> canRefresh = null, 
            Action? onCollectionChanged = null, 
            Action? onSelectionChanged = null, 
            bool refreshOnFirstCollectionRequest = true,
            Action? onSelectedItemsChanged = null
            )
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest)
        {
            this.onSelectedItemsChanged = onSelectedItemsChanged;
        }



        /// <summary>
        /// Create a new RefreshableListBox that refreshes asynchronously
        /// </summary>
        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool, Action)"/>
        public RefreshableListBoxDefinition(
            Func<Task<TList>> refreshAsync, 
            Func<bool> canRefresh = null, 
            Action? onCollectionChanged = null, 
            Action? onSelectionChanged = null, 
            bool refreshOnFirstCollectionRequest = true,
            Action? onSelectedItemsChanged = null
            )
            : base(refreshAsync, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest)
        {
            this.onSelectedItemsChanged = onSelectedItemsChanged;
        }


        /// <summary>
        /// Create a new RefreshableListBox that refreshes asynchronously and allows for cancellation of the resfresh 
        /// </summary>
        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList}, Func{bool}, Action, Action, bool, Action)"/>
        public RefreshableListBoxDefinition(
            Func<CancellationToken, Task<TList>> refreshAsyncCancellable, 
            Func<bool> canRefresh = null, 
            Action? onCollectionChanged = null, 
            Action? onSelectionChanged = null, 
            bool refreshOnFirstCollectionRequest = true,
            Action? onSelectedItemsChanged = null
            )
            : base(refreshAsyncCancellable, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest)
        {
            this.onSelectedItemsChanged = onSelectedItemsChanged;
        }

        

        /* Events */

        private readonly Action? onSelectedItemsChanged = null;

        /// <summary>
        /// Occurs after the <see cref="SelectedItems"/> has been updated
        /// </summary>
        public event EventHandler SelectedItemsChanged;

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(SelectedItems))]
        public bool HasItemsSelected => SelectedItems is not null && SelectedItems.Count > 0;

        /// <summary>
        /// Check if the <see cref="SelectionMode"/> is not <see cref="System.Windows.Controls.SelectionMode.Single"/>
        /// </summary>
        public bool IsMultiSelect => SelectionMode != System.Windows.Controls.SelectionMode.Single;

        /// <summary>
        /// Allows binding to the SelectedItems property of a listbox via the <see cref="RFBCodeWorks.WPF.Behaviors.Base.MultiItemSelectionBehavior{T}"/> behavior
        /// </summary>
        /// <remarks>
        /// Use <see cref="HasItemsSelected"/> as a guard statement before iterating.
        /// </remarks>
        [ObservableProperty]
        private IList<T>? _selectedItems;

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
            onSelectedItemsChanged?.Invoke();
        }
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------
     * RefreshableListBoxDefinition<T>
     *--------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}"/>
    public sealed class RefreshableListBoxDefinition<T> : RefreshableListBoxDefinition<T, IList<T>, object>
    {
        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList})"/>
        public RefreshableListBoxDefinition(Func<IList<T>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList})"/>
        public RefreshableListBoxDefinition(Func<IList<T>> refresh, Func<bool> canRefresh = null, Action? onCollectionChanged = null, Action? onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{Task{TList}})"/>
        public RefreshableListBoxDefinition(Func<Task<IList<T>>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{Task{TList}})"/>
        public RefreshableListBoxDefinition(Func<Task<IList<T>>> refresh, Func<bool> canRefresh = null, Action? onCollectionChanged = null, Action? onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{CancellationToken, Task{TList}})"/>
        public RefreshableListBoxDefinition(Func<CancellationToken, Task<IList<T>>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{CancellationToken, Task{TList}})"/>
        public RefreshableListBoxDefinition(Func<CancellationToken, Task<IList<T>>> refresh, Func<bool> canRefresh = null, Action? onCollectionChanged = null, Action? onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }
    }


    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------
     * RefreshableListBoxDefinition<T, TSelectedValue>
     *--------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}"/>
    public sealed class RefreshableListBoxDefinition<T, TSelectedValue> : RefreshableListBoxDefinition<T, IList<T>, TSelectedValue>
    {

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList})"/>
        public RefreshableListBoxDefinition(Func<IList<T>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{TList})"/>
        public RefreshableListBoxDefinition(Func<IList<T>> refresh, Func<bool> canRefresh = null, Action? onCollectionChanged = null, Action? onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{Task{TList}})"/>
        public RefreshableListBoxDefinition(Func<Task<IList<T>>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{Task{TList}})"/>
        public RefreshableListBoxDefinition(Func<Task<IList<T>>> refresh, Func<bool> canRefresh = null, Action? onCollectionChanged = null, Action? onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{CancellationToken, Task{TList}})"/>
        public RefreshableListBoxDefinition(Func<CancellationToken, Task<IList<T>>> refresh) : base(refresh) { }

        /// <inheritdoc cref="RefreshableListBoxDefinition{T, TList, TSelectedValue}.RefreshableListBoxDefinition(Func{CancellationToken, Task{TList}})"/>
        public RefreshableListBoxDefinition(Func<CancellationToken, Task<IList<T>>> refresh, Func<bool> canRefresh = null, Action? onCollectionChanged = null, Action? onSelectionChanged = null, bool refreshOnFirstCollectionRequest = true)
            : base(refresh, canRefresh, onCollectionChanged, onSelectionChanged, refreshOnFirstCollectionRequest) { }
    }
}
