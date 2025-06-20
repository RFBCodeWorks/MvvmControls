using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        /// <inheritdoc cref="ListBox.SelectionMode" path="*"/>
        SelectionMode SelectionMode { get; set; }

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
        public ListBoxDefinition(Action onCollectionChanged = null, Action onSelectionChanged = null, TList collection = default) : base(onCollectionChanged, onSelectionChanged, collection) { }

        /// <summary>
        /// Occurs after the <see cref="SelectedItems"/> has been updated
        /// </summary>
        public event EventHandler SelectedItemsChanged;

        /// <inheritdoc/>
        public bool HasItems => Items?.Any() ?? false;

        /// <inheritdoc/>
        public bool HasItemsSelected => SelectedItems?.Any() ?? false;

        /// <summary>
        /// Check if the <see cref="SelectionMode"/> is not <see cref="SelectionMode.Single"/>
        /// </summary>
        public bool IsMultiSelect => SelectionMode != SelectionMode.Single;

        /// <summary>
        /// Allows binding to the SelectedItems property of a listbox via the <see cref="RFBCodeWorks.WPF.Behaviors.Base.MultiItemSelectionBehavior{T}"/> behavior
        /// </summary>
        [ObservableProperty]
        private IList<T> _selectedItems;

        /// <inheritdoc/>
        [ObservableProperty]
        private SelectionMode _selectionMode = SelectionMode.Single;
        
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

    /// <summary>
    /// A Simple ListBox/ListView Definition that only ListBox/ListView the enumerated type
    /// </summary>
    /// <inheritdoc cref="ListBoxDefinition{T, TList,  TSelectedValue}"/>
    public class ListBoxDefinition<T> : ListBoxDefinition<T, IList<T>, object>
    {
        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition()"/>
        public ListBoxDefinition() { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ListBoxDefinition(IList<T> collection) : this(null, null, collection) { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ListBoxDefinition(params T[] collection) : this(null, null, collection) { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ListBoxDefinition(Action onCollectionChanged = null, Action onSelectionChanged = null, params T[] collection) : base(onCollectionChanged, onSelectionChanged, collection ?? Array.Empty<T>()) { }


        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ListBoxDefinition(Action onCollectionChanged = null, Action onSelectionChanged = null, IList<T> collection = default) : base(onCollectionChanged, onSelectionChanged, collection ?? Array.Empty<T>()) { }
    }

    /// <summary>
    /// A generic definition for a ListBox/ListView whose ItemSource is any IEnumerable object, that expects a SelectedValue of a specific type
    /// </summary>
    /// <inheritdoc cref="ListBoxDefinition{T, TList,  TSelectedValue}"/>
    public class ListBoxDefinition<T, TSelectedValue> : ListBoxDefinition<T, IList<T>, TSelectedValue>
    {
        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition()"/>
        public ListBoxDefinition() { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ListBoxDefinition(IList<T> collection) : this(null, null, collection) { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ListBoxDefinition(params T[] collection) : this(null, null, collection) { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ListBoxDefinition(Action onCollectionChanged = null, Action onSelectionChanged = null, params T[] collection) : base(onCollectionChanged, onSelectionChanged, collection ?? Array.Empty<T>()) { }

        /// <summary>Create a new ComboBox </summary>
        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectorDefinition(Action, Action, TList)"/>
        public ListBoxDefinition(Action onCollectionChanged = null, Action onSelectionChanged = null, IList<T> collection = default) : base(onCollectionChanged, onSelectionChanged, collection ?? Array.Empty<T>()) { }
    }

}
