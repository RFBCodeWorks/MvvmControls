using RFBCodeWorks.Mvvm.XmlLinq;

namespace RFBCodeWorks.Mvvm.Specialized
{
    /// <summary>
    /// shared base class for Xml.Linq TreeViewItems
    /// </summary>
    public abstract class XNodeTreeViewItem<TXNode> : XNodeTreeViewItem
        where TXNode : IXObjectProvider
    {
        /// <inheritdoc cref="TreeViewItemBase{T, TParent}.TreeViewItemBase(T)"/>
        protected XNodeTreeViewItem(TXNode item) : base(item) { }

        /// <inheritdoc cref="TreeViewItemBase{T, TParent}.TreeViewItemBase(T, TParent)"/>
        protected XNodeTreeViewItem(TXNode item, XElementTreeViewItem parent) : base(item, parent) { }

        /// <inheritdoc cref="TreeViewItemBase{T, TParent}.Item"/>
        public new TXNode Item => (TXNode)base.Item;
    }

    /// <summary>
    /// Shared base class for Xml.Linq TreeViewItems. <br/>
    /// Cannot be instantiated by consumers. Must use <see cref="XNodeTreeViewItem{TXNode}"/>
    /// </summary>
    public abstract class XNodeTreeViewItem : TreeViewItemBase<IXObjectProvider, XElementTreeViewItem>
    {
        internal XNodeTreeViewItem(IXObjectProvider item) : base(item) { }
        internal XNodeTreeViewItem(IXObjectProvider item, XElementTreeViewItem parent) : base(item, parent) { }

        /// <inheritdoc/>
        public override string Name => base.Item?.XObject?.GetName();
    }
}
