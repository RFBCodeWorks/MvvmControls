using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm
{
    /// <inheritdoc/>
    public class TreeViewItemBase<TItem> : TreeViewItemBase<TItem, ITreeViewItem>
    {
        /// <inheritdoc/>
        protected TreeViewItemBase(TItem item) : base(item) { }

        /// <inheritdoc cref="TreeViewItemBase(string, ITreeViewItem)"/>
        protected TreeViewItemBase(TItem item, ITreeViewItem parent) : base(item, parent) { }

        /// <inheritdoc/>
        public TreeViewItemBase(TItem item, string name) : base(item, name) { }

        /// <inheritdoc/>
        public TreeViewItemBase(TItem item, string name, ITreeViewItem parent) : base(item, name, parent) { }

    }

    /// <summary>A TreeViewItem that provides details about a specified <typeparamref name="TItem"/></summary>
    /// <typeparam name="TItem">The type of Item contained within this TreeViewItem</typeparam>
    /// <typeparam name="TParent">The type of Parent item</typeparam>
    public class TreeViewItemBase<TItem, TParent> : TreeViewItemBase where TParent : ITreeViewItem
    {
        /// <inheritdoc cref="TreeViewItemBase()"/>
        protected TreeViewItemBase(TItem item) : base() { Item = item; }

        /// <inheritdoc cref="TreeViewItemBase(ITreeViewItem)"/>
        protected TreeViewItemBase(TItem item, TParent parent) : base(parent) { Item = item; }

        /// <inheritdoc cref="TreeViewItemBase(string)"/>
        public TreeViewItemBase(TItem item, string name) : base(name) { Item = item; }
        
        /// <inheritdoc cref="TreeViewItemBase(string, ITreeViewItem)"/>
        public TreeViewItemBase(TItem item, string name, TParent parent) : base(name, parent) { Item = item; }


        /// <summary>
        /// The Item represented by this TreeViewItem
        /// </summary>
        public virtual TItem Item { get; }

        /// <inheritdoc cref="TreeViewItemBase.Parent"/>
        public new TParent Parent
        {
            get => (TParent)base.Parent;
            init => base.Parent = value;
        }
    }

    /// <summary>
    /// Base implementation of <see cref="ITreeViewItem"/>
    /// </summary>
    public class TreeViewItemBase : ObservableObject, ITreeViewItem
    {
        /// <summary>
        /// Instantiate the base class - Name will be <see cref="String.Empty"/> unless overridden.
        /// </summary>
        protected TreeViewItemBase() : this(string.Empty) { }
        
        internal TreeViewItemBase(ITreeViewItem parent) { Parent = parent; }

        /// <summary>
        /// Create a new <see cref="TreeViewItemBase"/> with the specified name
        /// </summary>
        public TreeViewItemBase(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Create a new <see cref="TreeViewItemBase"/> with the specified name and parent
        /// </summary>
        public TreeViewItemBase(string name, ITreeViewItem parent)
        {
            Name = name;
            Parent = parent;
        }

        /// <inheritdoc/>
        public event EventHandler Selected;

        /// <inheritdoc/>
        public event EventHandler Deselected;

        /// <inheritdoc/>
        /// <remarks>Raised automatically if subscribed to children via <see cref="SubscribeToChild(ITreeViewItem)"/></remarks>
        public event EventHandler ChildSelected;

        /// <inheritdoc/>        
        public event EventHandler Expanded;

        /// <inheritdoc/>        
        public event EventHandler Collapsed;

        private bool IsExpandedField;
        private bool IsSelectedField;

        /// <summary>
        /// The Parent item
        /// </summary>
        public ITreeViewItem Parent { get; init; }

        /// <inheritdoc/>
        public virtual string Name { get; }

        /// <inheritdoc/>
        public bool IsSelected
        {
            get => IsSelectedField;
            set
            {
                if (SetProperty(ref IsSelectedField, value, nameof(IsSelected)))
                {
                    if (value)
                        Selected?.Invoke(this, EventArgs.Empty);
                    else
                        Deselected?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <inheritdoc/>
        public bool IsExpanded
        {
            get => IsExpandedField;
            set
            {
                if (SetProperty(ref IsExpandedField, value, nameof(IsExpanded)))
                {
                    if (value) 
                        Expanded?.Invoke(this, EventArgs.Empty);
                    else
                        Collapsed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>The protected property through which <see cref="ITreeViewItem.Children"/> is explcitly implemented. If not overridden, returns the empty array.</summary>
        protected virtual IEnumerable<ITreeViewItem> ITreeViewChildren => Array.Empty<ITreeViewItem>();

        IEnumerable<ITreeViewItem> ITreeViewItem.Children => ITreeViewChildren;

        /// <summary>
        /// Raise <see cref="ChildSelected"/>
        /// </summary>
        /// <param name="sender">The child that has been selected</param>
        /// <param name="e">The event args</param>
        protected virtual void OnChildSelected(object sender, EventArgs e)
        {
            ChildSelected?.Invoke(sender, e);
        }

        /// <summary>
        /// Subscribe to the item's Selected and ChildSelected events
        /// </summary>
        protected virtual void SubscribeToChild(ITreeViewItem item)
        {
            item.Selected += OnChildSelected;
            item.ChildSelected += OnChildSelected;
        }

        /// <summary>
        /// Unsubscribe from the item's Selected and ChildSelected events
        /// </summary>
        protected virtual void UnsubscribeFromChild(ITreeViewItem item)
        {
            item.Selected -= OnChildSelected;
            item.ChildSelected -= OnChildSelected;
        }

        /// <summary>
        /// Expand this item, then work up the tree to expand all <see cref="Parent"/> items
        /// </summary>
        public virtual void ExpandParents()
        {
            IsExpanded = true;
            if (Parent is TreeViewItemBase tv)
                tv.ExpandParents();
            else if (Parent is ITreeViewItem)
                Parent.IsExpanded = true;
        }

        /// <summary>
        /// Call <see cref="ExpandParents"/>, then set <see cref="IsSelected"/> to <see langword="true"/>
        /// </summary>
        public void SelectAndExpand()
        {
            ExpandParents();
            IsSelected = true;
        }
    }
}
