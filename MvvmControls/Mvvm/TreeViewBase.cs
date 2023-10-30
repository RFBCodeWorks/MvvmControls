using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Shorthand TreeView type
    /// </summary>
    /// <inheritdoc/>
    public class TreeViewBase<TRootItem> : TreeViewBase<TRootItem, ITreeViewItem> where TRootItem : ITreeViewItem<ITreeViewItem> { }

    /// <summary>
    /// Class that represents a TreeView
    /// </summary>
    /// <typeparam name="TRootItem">The type of item that represents the root of the tree</typeparam>
    /// <typeparam name="TSelectedItem">The <see cref="SelectedItem"/> type</typeparam>
    public class TreeViewBase<TRootItem, TSelectedItem> : Primitives.ControlBase, IItemSource, ISelector
        where TRootItem : ITreeViewItem<TSelectedItem>
        where TSelectedItem : ITreeViewItem
    {

        /// <inheritdoc/>
        public event EventHandler SelectedItemChanged;

        /// <inheritdoc/>
        public event EventHandler ItemSourceChanged;

        private TSelectedItem SelectedItemField;
        private TRootItem TreeRootField;
        private ReadOnlyCollection<TRootItem> LazyItems;

        /// <summary>
        /// The Root item of the tree
        /// </summary>
        public TRootItem TreeRoot
        {
            get { return TreeRootField; }
            set
            {
                TRootItem oldVal = TreeRootField;
                if (SetProperty(ref TreeRootField, value, nameof(TreeRoot)))
                {
                    if (oldVal is TRootItem)
                    {
                        oldVal.ChildSelected -= OnChildSelected;
                        oldVal.Selected -= OnChildSelected;
                    }
                    if (value is TRootItem)
                    {
                        value.ChildSelected += OnChildSelected;
                        value.Selected += OnChildSelected;
                    }
                    LazyItems = null;
                    OnPropertyChanged(nameof(Items));
                    ItemSourceChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// The <see cref="TreeRoot"/> wrapped in a ReadOnlyCollection
        /// </summary>
        public ReadOnlyCollection<TRootItem> Items
        {
            get
            {
                if (LazyItems is null)
                    LazyItems = new ReadOnlyCollection<TRootItem>(new TRootItem[] { TreeRoot });
                return LazyItems;
            }
        }

        /// <inheritdoc/>
        public TSelectedItem SelectedItem
        {
            get { return SelectedItemField; }
            private set
            {
                //TSelectedItem oldVal = SelectedItemField;
                if (SetProperty(ref SelectedItemField, value, nameof(SelectedItem)))
                {
                    //if (oldVal is TSelectedItem) oldVal.IsSelected = false;
                    //value.IsSelected = true;
                    SelectedItemChanged?.Invoke(this, EventArgs.Empty);
                    ISelectorEvent?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #region < IItemSource Implementation >

        IList IItemSource.Items => TreeRoot.Children is IList t ? t : TreeRoot.Children.ToArray();

        string IItemSource.DisplayMemberPath => nameof(ITreeViewItem.Name);

        #endregion

        #region < ISelector Implementation >

        int ISelector.SelectedIndex
        {
            get => ((IItemSource)this).Items.IndexOf(SelectedItem);
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
            get => SelectedItem;
            set
            {
                if (value is TSelectedItem val)
                    SelectedItem = val;
            }
        }

        string ISelector.SelectedValuePath => SelectedItem.Name;

        event EventHandler ISelector.SelectedItemChanged
        {
            add { ISelectorEvent += value; }
            remove { ISelectorEvent -= value; }
        }
        private event EventHandler ISelectorEvent;
        

        #endregion

        /// <summary> Sets the <see cref="SelectedItem"/> to the <paramref name="sender"/> </summary>
        protected virtual void OnChildSelected(object sender, EventArgs e)
        {
            SelectedItem = (TSelectedItem)sender;
        }
    }
}
