using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm.Primitives
{
    /// <typeparam name="TSelectedValue">The SelectedValue Type obtained by the SelectedValuePath</typeparam>
    /// <inheritdoc cref="ItemSource{T, E}"/>
    /// <typeparam name="TList"/><typeparam name="T"/>
    public partial class SelectorDefinition<T, TList, TSelectedValue> : ItemSource<T, TList>, ISelector, ISelector<T>
        where TList : IList<T>
    {
        /// <summary>
        /// Crete a new Selector
        /// </summary>
        public SelectorDefinition() { }

        /// <summary>
        /// Crete a new Selector
        /// </summary>
        public SelectorDefinition(TList collection) : base(collection) { }

        /// <summary>
        /// Create a new selector
        /// </summary>
        /// <param name="collection">The collection or null</param>
        /// <param name="onSelectionChanged">An action to be inoked when the selected item changes (such as calling IRelayCommand.CanExecuteChanged)</param>
        /// <inheritdoc cref="ItemSource{T, TList}.ItemSource(Action, TList)"/>
        /// <param name="onCollectionChanged"/>
        public SelectorDefinition(Action? onCollectionChanged = null, Action? onSelectionChanged = null, TList? collection = default) : base(onCollectionChanged, collection)
        {
            _onSelectionChanged = onSelectionChanged;
        }

        private readonly Action? _onSelectionChanged;
        private TSelectedValue? _selectedValue;
        private int? _selectedIndex = null;
        private bool _indexChanging;
        private bool _selectedValueChanging;
        private bool _collectionChanging = false;

        /// <summary>
        /// Occurs after the <see cref="SelectedItem"/> has been updated
        /// </summary>
        public event PropertyOfTypeChangedEventHandler<T> SelectedItemChanged;

        /// <summary>
        /// Occurs when the <see cref="SelectedValue"/> is updated ( after <see cref="SelectedItem"/> changes )
        /// </summary>
        public event PropertyOfTypeChangedEventHandler<TSelectedValue> SelectedValueChanged;

        /// <summary>
        /// Value indicating when the <see langword="SelectedItem"/> property is not null and that the <see langword="SelectedIndex"/> falls within the valid range.
        /// </summary>
        /// <remarks>
        /// - <see cref="SelectedItem"/>
        /// <br/> - <see cref="SelectedIndex"/>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(SelectedItem))]
        public bool IsItemSelected => Items is not null && SelectedIndex >= 0 && SelectedIndex < Items.Count;

        /// <summary>
        /// Specify the property name/path to use as the <see cref="SelectedValue"/> of the <see cref="SelectedItem"/>
        /// </summary>
        /// <remarks>
        /// Example: If the <see cref="SelectedItem"/> is type Person, and this value is 'Name', then the <see cref="SelectedValue"/> = Person.Name
        /// </remarks>
        [ObservableProperty]
        private string _selectedValuePath = DefaultDisplayMemberPath;

        /// <summary>
        /// The Currently Selected item within the user control - May be null!
        /// </summary>
        /// <remarks>
        /// Binding for the 'item' property of controls such as ComboBox or ListBox
        /// </remarks>
        [ObservableProperty]
        private T? _selectedItem;

        /// <summary>
        /// If bound to the control, the control will set this property according to the property defined by SelectedValuePath
        /// </summary>
        /// <remarks>
        /// Example: If the item = Person(Joe, Schmoe), and SelectedValuePath = "FirstName", then SelectedValue = "Joe"
        /// </remarks>
        public TSelectedValue SelectedValue
        {
            get
            {
                return _selectedValue ??= GetSelectedValue(SelectedItem, SelectedValuePath);
            }
            set
            {
                if (Items is null) throw new InvalidOperationException("Items collection is null");
                if (Items.Count == 0) throw new InvalidOperationException("Items collection is empty");
                if (_selectedValue is null && value is null) return;
                if (_selectedValue is not null && value is not null && _selectedValue.Equals(value)) return;

                _selectedValueChanging = true;
                OnPropertyChanging(EventArgSingletons.SelectedValue);
                OnPropertyChanging(EventArgSingletons.SelectedIndex);
                
                var oldSelectedValue = _selectedValue;
                var pInfo = GetPropertyInfo();
                TrySelectItem(item => value.Equals(GetSelectedValue(pInfo, item)));
                _selectedValue = value;
                OnPropertyChanged(EventArgSingletons.SelectedIndex);
                OnPropertyChanged(EventArgSingletons.SelectedValue);
                _selectedValueChanging = false;

                RaiseSelectedValueChanged(oldSelectedValue);
            }
        }

        /// <summary>
        /// The index of the currently selected item within the ItemSource.
        /// </summary>
        /// <remarks>
        /// If bound, updates the selected item.
        /// </remarks>
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex ??= getIndex(SelectedItem, Items);
                static int getIndex(T? selection, TList list)
                {
                    if (typeof(T).IsByRef is false && selection is null)
                        return -1;
                    return list.IndexOf(selection);
                }
            }
            set
            {

                if (!_collectionChanging && _selectedIndex == value)
                {
                    return; // no change;
                }
                else if (value >= 0 && (Items?.Count ?? 0) == 0) // throw if greater than collection count
                {
                    // No items in collection
                    throw new ArgumentOutOfRangeException(nameof(value), $"SelectedIndex property can not be set when the collection has no items");
                }
                else if (value < Items.Count)
                {
                    _indexChanging = true;
                    var oldSelectedValue = _selectedValue;
                    if (_selectedValueChanging is false)
                    {
                        OnPropertyChanging(EventArgSingletons.SelectedIndex);
                        OnPropertyChanging(EventArgSingletons.SelectedValue);
                    }
                    if (value < 0)
                    {
                        _selectedIndex = -1;
                        SelectedItem = default;
                        _selectedValue = default;
                    }
                    else
                    {
                        _selectedIndex = value;
                        SelectedItem = Items[value];
                        _selectedValue = default;
                    }
                    if (_selectedValueChanging is false)
                    {
                        OnPropertyChanged(EventArgSingletons.SelectedValue);
                        OnPropertyChanged(EventArgSingletons.SelectedIndex);
                        RaiseSelectedValueChanged(oldSelectedValue);
                    }
                    _indexChanging = false;
                    _collectionChanging = false;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"SelectedIndex property was set to a value outside the valid range (expected value between -1 and number of items in the collection ( currently: {Items.Count} )");
                }
            }
        }

        /// <summary>
        /// Flag that the collection is changing, 
        /// so that the SelectedIndex and SelectedItem can be updated accordingly in <see cref="OnItemsChanged"/>
        /// </summary>
        protected override void OnItemsChanging()
        {
            _collectionChanging = true;
        }


        /// <summary>
        /// Sets the SelectedIndex to -1 if the collection has changed and the SelectedIndex or SelectedItem has not been updated during that collection change.
        /// Also sets the SelectedIndex to -1 if the collection is cleared ( count == 0 ) or if the SelectedItem is null.
        /// </summary>
        protected override void OnItemsChanged()
        {
            if (_collectionChanging || SelectedItem is null || Items.Count == 0)
            {
                SelectedIndex = -1;
            }
            _collectionChanging = false;
        }

        /// <summary>
        /// <see cref="SelectedItem"/> is Changing
        /// </summary>
        partial void OnSelectedItemChanging(T value)
        {
            // reset the flag that would reset the SelectedIndex to -1
            // this accommodates scenarios where OnCollectionChanged is used to select an item from the new collection.
            _collectionChanging = false;

            if (_indexChanging || _selectedValueChanging) return; // changing event raised in index setter
            OnPropertyChanging(EventArgSingletons.SelectedIndex);
            OnPropertyChanging(EventArgSingletons.SelectedValue);
        }

        /// <summary>
        /// <see cref="SelectedItem"/> has been changed
        /// </summary>
        partial void OnSelectedItemChanged(T? oldValue, T newValue)
        {
            // invoke the events if needed
            if (SelectedItemChanged != null || ISelectorEvent != null)
            {
                var e = new PropertyOfTypeChangedEventArgs<T>(oldValue, newValue, nameof(SelectedItem));
                SelectedItemChanged?.Invoke(this, e);
                ISelectorEvent?.Invoke(this, e);
            }

            if (_indexChanging is false && _selectedValueChanging is false)
            {
                var oldSelectedValue = _selectedValue;
                _selectedIndex = null;
                _selectedValue = default;
                OnPropertyChanged(EventArgSingletons.SelectedIndex);
                OnPropertyChanged(EventArgSingletons.SelectedValue);
                RaiseSelectedValueChanged(oldSelectedValue);
            }

            // invoke the constructor supplied action
            _onSelectionChanged?.Invoke();
        }

        /// <summary> </summary>
        private void RaiseSelectedValueChanged(TSelectedValue? old)
        {
            if (SelectedValueChanged != null)
            {
                SelectedValueChanged?.Invoke(this, new(old, SelectedValue, nameof(SelectedValue)));
            }
        }

        /// <summary>
        /// Helper to get a named property via reflection
        /// </summary>
        private static TSelectedValue GetSelectedValue(T item, string selectedValuePath)
        {
            if (item is null) return default;
            if (string.IsNullOrEmpty(selectedValuePath) && item is TSelectedValue ts) return ts;
            var pInfo = item.GetType().GetProperty(selectedValuePath);
            if (pInfo?.GetValue(item) is TSelectedValue value) return value;
            return default;
        }
        private static TSelectedValue GetSelectedValue(PropertyInfo pInfo, T item)
        {
            if (item is null || pInfo is null) return default;
            object result = pInfo.GetValue((object)item);
            return result is TSelectedValue value ? value : default;
        }
        private PropertyInfo GetPropertyInfo() => string.IsNullOrWhiteSpace(SelectedValuePath) ? null : typeof(T).GetProperty(SelectedValuePath);

        /// <summary> 
        /// Searches the ItemSource for the first match to the <paramref name="predicate"/>, then sets the SelectedValue to the result. 
        /// <br/> If no match is found, the SelectedIndex is set to the <paramref name="defaultIndex"/>
        /// </summary>
        /// <param name="defaultIndex">The index of the item to select if no match exists within the ItemSource</param>
        /// <param name="predicate">A function used to determine if the item should be selected.</param>
        public virtual void TrySelectItem(Func<T, bool> predicate, int defaultIndex = -1)
        {
            var item = Items.FirstOrDefault(predicate);
            if (item is null)
            {
                // this will set the selected item & reset selectedValue
                SelectedIndex = defaultIndex;
            }
            else
            {
                // SelectedValue will be reset if needed
                _selectedIndex = null;
                SelectedItem = item;
            }
        }

        #region < ISelector Implementation >

        object ISelector.SelectedItem
        {
            get => SelectedItem; 
            set 
            { 
                if (value is T val)
                    SelectedItem = val;
            }
        }

        object ISelector.SelectedValue
        {
            get => SelectedValue;
            set
            {
                if (value is TSelectedValue val)
                    SelectedValue = val;
            }
        }

        event EventHandler ISelector.SelectedItemChanged
        {
            add { ISelectorEvent += value; }
            remove { ISelectorEvent -= value; }
        }
        private event EventHandler ISelectorEvent;

        #endregion

    }
}
