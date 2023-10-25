using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Primitives
{

    /// <summary>
    /// Abstract class that represents a TreeView whose collection type is an <see cref="ObservableCollection{T}"/>
    /// </summary>
    /// <inheritdoc cref="AbstractTreeView{TSelectedItem, TEnumerable, TSelectedValue}"/>
    public class AbstractTreeView<TSelectedItem> : AbstractTreeView<TSelectedItem, ObservableCollection<TSelectedItem>, object>
        where TSelectedItem : ITreeViewItem
    { }

    /// <inheritdoc cref="AbstractTreeView{TSelectedItem, TEnumerable, TSelectedValue}"/>
    public class AbstractTreeView<TSelectedItem, TEnumerable> : AbstractTreeView<TSelectedItem, TEnumerable, object>
        where TSelectedItem : ITreeViewItem
        where TEnumerable : IList<TSelectedItem>
    { }

    /// <summary>
    /// Abstract class that represents a TreeView
    /// </summary>
    /// <typeparam name="TSelectedItem">The <see cref="SelectedItem"/> type</typeparam>
    /// <typeparam name="TEnumerable">The type of collection</typeparam>
    /// <typeparam name="TSelectedValue">The <see cref="SelectedValue"/> type</typeparam>
    public class AbstractTreeView<TSelectedItem, TEnumerable, TSelectedValue> :  ItemSource<TSelectedItem,TEnumerable>, ISelector, ISelector<TSelectedItem, TEnumerable>
        where TSelectedItem : ITreeViewItem
        where TEnumerable : IList<TSelectedItem>
    {

        /// <inheritdoc/>
        public event EventHandler SelectedItemChanged;

        private bool IsExpandedField;
        private TSelectedItem SelectedItemField;
        private TSelectedValue SelectedValueField;
        private string SelectedValuePathField = DefaultDisplayMemberPath;


        /// <inheritdoc cref="System.Windows.Controls.TreeView"/>
        public bool IsExpanded
        {
            get { return IsExpandedField; }
            set { SetProperty(ref IsExpandedField, value, nameof(IsExpanded)); }
        }
        

        /// <inheritdoc/>
        public TSelectedItem SelectedItem
        {
            get { return SelectedItemField; }
            set { SetProperty(ref SelectedItemField, value, nameof(SelectedItem)); }
        }
        

        /// <inheritdoc/>
        public TSelectedValue SelectedValue
        {
            get { return SelectedValueField; }
            set { SetProperty(ref SelectedValueField, value, nameof(SelectedValue)); }
        }

        /// <inheritdoc cref="SelectorDefinition{T, E, V}.SelectedValuePath"/>
        public string SelectedValuePath
        {
            get { return SelectedValuePathField; }
            set { SetProperty(ref SelectedValuePathField, value ?? "", nameof(SelectedValuePath)); }
        }

        #region < ISelector Implementation >

        int ISelector.SelectedIndex 
        {
            get => Items.IndexOf(SelectedItem);
            set { }
        }

        object ISelector.SelectedItem
        {
            get => SelectedItem;
            set
            {
                if (value is TSelectedItem val)
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

        /// <summary> Raises the SelectionChanged event </summary>
        protected virtual void OnSelectedItemChanged(PropertyOfTypeChangedEventArgs<TSelectedItem> e)
        {
            SelectedItemChanged?.Invoke(this, e);
            ISelectorEvent?.Invoke(this, e);
        }
    }
}
