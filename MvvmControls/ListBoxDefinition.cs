﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RFBCodeWorks.MvvmControls
{
    /// <summary>
    /// Interface all ListBoxDefinitions should implement to be assignable via AttachedProperty
    /// </summary>
    public interface IListBox : ISelector
    {
        /// <inheritdoc cref="ListBox.SelectionMode" path="*"/>
        SelectionMode SelectionMode { get; }

        /// <inheritdoc cref="ListBoxDefinition{T, E, V}.IsMultiSelect"/>
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
        IList SelectedItems { get; }
    }

    /// <summary>
    /// The base ListBoxDefinition object
    /// </summary>
    /// <inheritdoc cref="Primitives.Selector{T, E, V}"/>
    public class ListBoxDefinition<T, E, V> : Primitives.Selector<T, E, V>, IListBox
        where E : IList<T>
    {
        #region < SelectionChangedEvent >

        /// <summary>
        /// Occurs after the <see cref="SelectedItems"/> has been updated
        /// </summary>
        public event EventHandler SelectedItemsChanged;

        /// <summary> Raises the SelectedItemsChanged event </summary>
        protected virtual void OnSelectedItemsChanged()
        {
            SelectedItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        /// <inheritdoc/>
        public bool HasItems => Items?.Any() ?? false;

        /// <inheritdoc/>
        public bool HasItemsSelected => SelectedItems?.Any() ?? false;

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
            set
            {
                var updated = SetProperty(ref SelectedItemsField, value, nameof(SelectedItems));
                if (updated) OnSelectedItemsChanged();
            }
        }
        private IList<T> SelectedItemsField;
        IList IListBox.SelectedItems => (IList)SelectedItems;

        /// <inheritdoc/>
        public SelectionMode SelectionMode
        {
            get { return SelectionModeField; }
            set { SetProperty(ref SelectionModeField, value, nameof(SelectionMode)); }
        }
        private SelectionMode SelectionModeField = SelectionMode.Single;

    }

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
    public class ListBoxDefinition<T, V> : ListBoxDefinition<T, IList<T>, V> { }

    #endregion

    #region < Refreshable >

    /// <summary>
    /// A listbox that contains only strings
    /// </summary>
    /// <inheritdoc cref="RefreshableListBoxDefinition{T, V}"/>
    public class RefreshableListBoxDefinition : RefreshableListBoxDefinition<string, object> { }

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
            RefreshCommand = new ButtonDefinition(Refresh, () => CanRefresh());
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
            if (RefreshFunc != null && (CanRefresh?.Invoke() ?? true))
                Items = RefreshFunc();
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

}