using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm.Primitives
{
    /// <typeparam name="TSelectedValue">The SelectedValue Type obtained by the SelectedValuePath</typeparam>
    /// <inheritdoc cref="ItemSource{T, E}"/>
    /// <typeparam name="TList"/><typeparam name="T"/>
    public partial class SelectorDefinition<T, TList, TSelectedValue> : ItemSource<T,TList>, ISelector, ISelector<T>
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
        /// <param name="onItemSourceChanged"/>
        public SelectorDefinition(Action onItemSourceChanged = null, Action onSelectionChanged = null, TList collection = default) : base(onItemSourceChanged, collection)
        {
            _onSelectionChanged = onSelectionChanged;
        }

        private readonly Action _onSelectionChanged;
        private int SelectedIndexField = -1;
        private bool UpdatingSelectedItem;

        /// <summary>
        /// Occurs after the <see cref="SelectedItem"/> has been updated
        /// </summary>
        public event PropertyOfTypeChangedEventHandler<T> SelectedItemChanged;
        
        /// <summary>
        /// Occurs when the <see cref="SelectedValue"/> is updated ( after <see cref="SelectedItem"/> changes )
        /// </summary>
        public event PropertyOfTypeChangedEventHandler<TSelectedValue> SelectedValueChanged;

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
        /// Binding for the 'SelectedItem' property of controls such as ComboBox or ListBox
        /// </remarks>
        [ObservableProperty]
        private T _selectedItem;

        /// <summary>
        /// If bound to the control, the control will set this property according to the property defined by SelectedValuePath
        /// </summary>
        /// <remarks>
        /// Example: If the SelectedItem = Person(Joe, Schmoe), and SelectedValuePath = "FirstName", then SelectedValue = "Joe"
        /// </remarks>

        [ObservableProperty]
        private TSelectedValue _selectedValue;

        /// <summary>
        /// Enable/Disable auto-updating of SelectedItem and SelectedIndex if bound by the ItemSource binding definition attached property
        /// </summary>
        internal bool IsBoundByBehavior { get; set; }

        /// <summary>
        /// The index of the currently selected item within the ItemSource
        /// </summary>
        /// <remarks>
        /// If bound, updates the selected item.
        /// </remarks>
        public int SelectedIndex
        {
            get { return SelectedIndexField; }
            set
            {
                if (SelectedIndexField == value)
                {
                    return; // no change;
                }
                else if (UpdatingSelectedItem) // value is retrieved while SelectedItem is being updated - this value should always be valid, and is the expected way for this to be set.
                {
                    OnPropertyChanging(EventArgSingletons.SelectedIndex);
                    SelectedIndexField = value;
                    OnPropertyChanged(EventArgSingletons.SelectedIndex);
                }
                else if (Items is null || !Items.Any())
                {
                    // No items in collection
                    if (value != -1) throw new ArgumentOutOfRangeException($"Cannot set the index of a selector when the collection has no items", nameof(SelectedIndex));
                }
                else
                {
                    //collection has items - check valid index
                    if (value == -1)
                    {
                        // This may be a bad choice if the <TValue> is a struct, and the default resides within the list. Ex: 5,4,3,2,1,0 -> index 5 will become selected.
                        // If this is an issue, a consumer can always use TValue = int? in place of it, allowing for actual null values.
                        SelectedItem = default; 
                    }
                    else if (value < Items.Count)
                    {
                        SelectedItem = Items[value];
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException($"SelectedIndex property was set to a value outside the valid range (expected value between -1 and number of items in the collection ( currently: {Items.Count} )", nameof(SelectedIndex));
                    }
                }
            }

        }

        partial void OnSelectedItemChanging(T value)
        {
            UpdatingSelectedItem = true;
            SelectedIndex = Items.IndexOf(value);
            UpdatingSelectedItem = false;
        }

        partial void OnSelectedItemChanged(T? oldValue, T newValue)
        {
            OnSelectedItemChanged(new PropertyOfTypeChangedEventArgs<T>(oldValue, newValue, nameof(SelectedItem)));
        }

        /// <summary> Raises the SelectionChanged event </summary>
        protected virtual void OnSelectedItemChanged(PropertyOfTypeChangedEventArgs<T> e)
        {
            SelectedItemChanged?.Invoke(this, e);
            ISelectorEvent?.Invoke(this, e);
            _onSelectionChanged?.Invoke();
        }

        partial void OnSelectedValueChanged(TSelectedValue? oldValue, TSelectedValue newValue)
        {
            OnSelectedValueChanged(new PropertyOfTypeChangedEventArgs<TSelectedValue>(oldValue, newValue, nameof(SelectedItem)));
        }

        /// <summary> Raises the SelectionChanged event </summary>
        protected virtual void OnSelectedValueChanged(PropertyOfTypeChangedEventArgs<TSelectedValue> e)
        {
            SelectedValueChanged?.Invoke(this, e);
            OnPropertyChanged(e);
        }

        /// <summary> 
        /// Searches the ItemSource for the first match to the <paramref name="predicate"/>, then sets the SelectedValue to the result. 
        /// <br/> If no match is found, the SelectedIndex is set to the <paramref name="defaultIndex"/>
        /// </summary>
        /// <param name="defaultIndex">The index of the item to select if no match exists within the ItemSource</param>
        /// <param name="predicate">A function used to determine if the item should be selected.</param>
        public virtual void TrySelectItem(Func<T, bool> predicate, int defaultIndex = -1)
        {
            var item = Items.FirstOrDefault(o => predicate(o));
            var index = item is null ? -1 : Items.IndexOf(item);
            SelectedIndex = index == -1 ? defaultIndex : index;
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
