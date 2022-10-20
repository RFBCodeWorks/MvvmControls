using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFBCodeWorks.MVVMObjects.ControlInterfaces;

namespace RFBCodeWorks.MVVMObjects.BaseControlDefinitions
{
    /// <typeparam name="V">The SelectedValue Type obtained by the SelectedValuePath</typeparam>
    /// <inheritdoc cref="ItemSourceDefinition{T, E}"/>
    /// <typeparam name="E"/><typeparam name="T"/>
    public class SelectorDefinition<T, E, V> : ItemSourceDefinition<T,E>, ISelector, ISelector<T>, ISelector<T, E>
        where E : IList<T>
    {
        #region < SelectionChangedEvent >

        /// <summary>
        /// Occurs after the <see cref="SelectedItem"/> has been updated
        /// </summary>
        public event PropertyOfTypeChangedEventHandler<T> SelectedItemChanged;

        /// <summary> Raises the SelectionChanged event </summary>
        protected virtual void OnSelectedItemChanged(PropertyOfTypeChangedEventArgs<T> e)
        {
            SelectedItemChanged?.Invoke(this, e);
            ISelectorEvent?.Invoke(this, e);
            OnPropertyChanged(e);
        }

        #endregion

        #region < SelectionValueChanged >

        /// <summary>
        /// Occurs when the <see cref="SelectedValue"/> is updated ( after <see cref="SelectedItem"/> changes )
        /// </summary>
        public event PropertyOfTypeChangedEventHandler<V> SelectedValueChanged;

        /// <summary> Raises the SelectionChanged event </summary>
        protected virtual void OnSelectedValueChanged(PropertyOfTypeChangedEventArgs<V> e)
        {
            SelectedValueChanged?.Invoke(this, e);
            OnPropertyChanged(e);
        }

        #endregion

        #region < Properties >

        private bool UpdatingSelectedItem;

        /// <summary>
        /// Enable/Disable auto-updating of SelectedItem and SelectedIndex if bound by the ItemSource binding definition attached property
        /// </summary>
        internal bool IsBoundByBehavior { get; set; }

        /// <summary>
        /// The Currently Selected item within the user control - May be null!
        /// </summary>
        /// <remarks>
        /// Binding for the 'SelectedItem' property of controls such as ComboBox or ListBox
        /// </remarks>
        public T SelectedItem
        {
            get { return SelectedItemField; }
            set {
                T oldValue = SelectedItemField;
                if (SetProperty(ref SelectedItemField, value, nameof(SelectedItem)))
                {
                    UpdatingSelectedItem = true;
                    SelectedIndex = ItemSource.IndexOf(value);
                    UpdatingSelectedItem = false;
                    OnSelectedItemChanged(new(oldValue, value, nameof(SelectedItem)));
                }
            }
        }
        private T SelectedItemField;

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
                if (UpdatingSelectedItem)
                    SelectedIndexField = value;
                else
                {
                    int newValue = ItemSource != null ? value : -1;
                    bool changed = SetProperty(ref SelectedIndexField, newValue, nameof(SelectedIndex));
                    if (!changed || ItemSource is null) return;         // Check if was updated, and if there is an itemsource
                    if (newValue >= 0 && newValue < ItemSource.Count)   // check if within acceptable range
                    {
                        SelectedItem = ItemSource[value];
                    }
                    else
                    {
                        SelectedItem = default;
                    }
                }
            }

        }
        private int SelectedIndexField = -1;

        /// <summary>
        /// If bound to the control, the control will set this property according to the property defined by SelectedValuePath
        /// </summary>
        /// <remarks>
        /// Example: If the SelectedItem = Person(Joe, Schmoe), and SelectedValuePath = "FirstName", then SelectedValue = "Joe"
        /// </remarks>
        public V SelectedValue
        {
            get { return SelectedValueField; }
            set
            {
                V oldValue = SelectedValueField;
                if (SetProperty(ref SelectedValueField, value, nameof(SelectedValue)))
                {
                    OnSelectedValueChanged(new(oldValue, value, nameof(SelectedValue)));
                }
            }
        }
        private V SelectedValueField;


        /// <summary>
        /// Specify the property name/path to use as the <see cref="SelectedValue"/> of the <see cref="SelectedItem"/>
        /// </summary>
        /// <remarks>
        /// Example: If the <see cref="SelectedItem"/> is type Person, and this value is 'Name', then the <see cref="SelectedValue"/> = Person.Name
        /// </remarks>
        public string SelectedValuePath
        {
            get { return SelectedValuePathField; }
            set { SetProperty(ref SelectedValuePathField, value ?? "", nameof(SelectedValuePath)); }
        }
        private string SelectedValuePathField = DefaultDisplayMemberPath;

        #endregion

        /// <summary> 
        /// Searches the ItemSource for the first match to the <paramref name="predicate"/>, then sets the SelectedValue to the result. 
        /// <br/> If no match is found, the SelectedIndex is set to the <paramref name="defaultIndex"/>
        /// </summary>
        /// <param name="defaultIndex">The index of the item to select if no match exists within the ItemSource</param>
        /// <param name="predicate">A function used to determine if the item should be selected.</param>
        public virtual void TrySelectItem(Func<T, bool> predicate, int defaultIndex = -1)
        {
            var item = ItemSource.FirstOrDefault(o => predicate(o));
            var index = item is null ? -1 : ItemSource.IndexOf(item);
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
                if (value is V val)
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
