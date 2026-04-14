using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Interface all ListBoxDefinitions should implement to be assignable via AttachedProperty
    /// <para/>Inherits:
    /// <br/> - <see cref="ISelector"/>
    /// </summary>
    public interface IListBox : ISelector
    {
        /// <inheritdoc cref="System.Windows.Controls.ListBox.SelectionMode" path="*"/>
        System.Windows.Controls.SelectionMode SelectionMode { get; set; }

        /// <inheritdoc cref="ListBoxDefinition{T, TList,  TSelectedValue}.IsMultiSelect"/>
        bool IsMultiSelect { get; }

        /// <summary>
        /// Check if the ItemSource has any items
        /// </summary>
        /// <returns>TRUE if the ItemSource has atleast 1 item, otherwise false</returns>
        bool HasItems { get; }

        /// <summary>
        /// Check if the SelectedItems has any items
        /// </summary>
        /// <returns>TRUE if the SelectedItems has atleast 1 item, otherwise false</returns>
        bool HasItemsSelected { get; }

        /// <summary>
        /// Enumerate through the Selected Items
        /// </summary>
        IList SelectedItems { get; set; }

    }

    /// <summary>
    /// Interface all ComboBoxDefinitions of some type implement
    /// <para/>Inherits: 
    /// <br/>  - <see cref="IListBox"/>
    /// <br/>  - <see cref="ISelector"/>
    /// <br/>  - <see cref="ISelector{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IListBox<T> : ISelector<T>, IListBox{ }

    /// <summary>
    /// The base ListBoxDefinition object
    /// </summary>
    /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}"/>
    public partial class ListBoxDefinition<T, TList,  TSelectedValue> : Primitives.SelectorDefinition<T, TList,  TSelectedValue>, IListBox, IListBox<T>
        where TList : IList<T>
    {
        /// <summary>Create a new ListBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition()"/>
        public ListBoxDefinition() { }

        /// <summary>Create a new ListBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition()"/>
        public ListBoxDefinition(TList collection) : base(collection) { }

        /// <summary>Create a new ListBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        /// <param name="collection"/>
        /// <param name="onCollectionChanged"/>
        /// <param name="onSelectionChanged"/>
        /// <param name="onSelectedItemsChanged">
        /// A action to run when the <see cref="SelectedItems"/> collection is swapped out for another collection.
        /// </param>
        public ListBoxDefinition(Action onCollectionChanged = null, Action onSelectionChanged = null, Action onSelectedItemsChanged = null, TList collection = default) 
            : base(onCollectionChanged, onSelectionChanged, collection) 
        {
            this.onSelectedItemsChanged = onSelectedItemsChanged;
        }

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
                if (value is null)
                {
                    SelectedItems = null;
                }
                else if (value is IList<T> list)
                {
                    SelectedItems = list;
                }
                else if (value is IReadOnlyList<T> rd)
                {
                    SelectedItems = [.. rd];
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

    /// <summary>
    /// A Simple ListBox/ListView Definition that only ListBox/ListView the enumerated type
    /// </summary>
    /// <inheritdoc cref="ListBoxDefinition{T, TList,  TSelectedValue}"/>
    public class ListBoxDefinition<T> : ListBoxDefinition<T, IList<T>, object>
    {
        /// <inheritdoc cref="ListBoxDefinition{T, TList, TSelectedValue}.ListBoxDefinition()"/>
        public ListBoxDefinition() { }

        /// <inheritdoc cref="ListBoxDefinition{T, TList, TSelectedValue}.ListBoxDefinition(TList)"/>
        public ListBoxDefinition(IList<T> collection) : base(collection) { }

        /// <inheritdoc cref="ListBoxDefinition{T, TList, TSelectedValue}.ListBoxDefinition(TList)"/>
        public ListBoxDefinition(params T[] collection) : base(collection) { }

        /// <inheritdoc cref="ListBoxDefinition{T, TList, TSelectedValue}.ListBoxDefinition(Action, Action, Action, TList)"/>
        public ListBoxDefinition(Action onCollectionChanged = null, Action onSelectionChanged = null, Action onSelectedItemsChanged = null, params T[] collection) : base(onCollectionChanged, onSelectionChanged, onSelectedItemsChanged, collection ?? Array.Empty<T>()) { }


        /// <inheritdoc cref="ListBoxDefinition{T, TList, TSelectedValue}.ListBoxDefinition(Action, Action, Action, TList)"/>
        public ListBoxDefinition(Action onCollectionChanged = null, Action onSelectionChanged = null, Action onSelectedItemsChanged = null, IList<T> collection = default) : base(onCollectionChanged, onSelectionChanged, onSelectedItemsChanged, collection ?? Array.Empty<T>()) { }
    }

    /// <summary>
    /// A generic definition for a ListBox/ListView whose ItemSource is any IEnumerable object, that expects a SelectedValue of a specific type
    /// </summary>
    /// <inheritdoc cref="ListBoxDefinition{T, TList,  TSelectedValue}"/>
    public class ListBoxDefinition<T, TSelectedValue> : ListBoxDefinition<T, IList<T>, TSelectedValue>
    {
        /// <inheritdoc cref="ListBoxDefinition{T, TList, TSelectedValue}.ListBoxDefinition()"/>
        public ListBoxDefinition() { }

        /// <inheritdoc cref="ListBoxDefinition{T, TList, TSelectedValue}.ListBoxDefinition(TList)"/>
        public ListBoxDefinition(IList<T> collection) : base(collection) { }

        /// <inheritdoc cref="ListBoxDefinition{T, TList, TSelectedValue}.ListBoxDefinition(TList)"/>
        public ListBoxDefinition(params T[] collection) : base(collection) { }

        /// <inheritdoc cref="ListBoxDefinition{T, TList, TSelectedValue}.ListBoxDefinition(Action, Action, Action, TList)"/>
        public ListBoxDefinition(Action onCollectionChanged = null, Action onSelectionChanged = null, Action onSelectedItemsChanged = null, params T[] collection) : base(onCollectionChanged, onSelectionChanged, onSelectedItemsChanged, collection ?? Array.Empty<T>()) { }

        /// <inheritdoc cref="ListBoxDefinition{T, TList, TSelectedValue}.ListBoxDefinition(Action, Action, Action, TList)"/>
        public ListBoxDefinition(Action onCollectionChanged = null, Action onSelectionChanged = null, Action onSelectedItemsChanged = null, IList<T> collection = default) : base(onCollectionChanged, onSelectionChanged, onSelectedItemsChanged, collection ?? Array.Empty<T>()) { }
    }

}
