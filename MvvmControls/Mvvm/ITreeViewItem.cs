using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// A strongly typed <see cref="ITreeViewItem"/> interface
    /// </summary>
    /// <typeparam name="TChild"></typeparam>
    public interface ITreeViewItem<TChild> : ITreeViewItem where TChild : ITreeViewItem
    {
        /// <summary>
        /// Any child TreeViewItems
        /// </summary>
        new IEnumerable<TChild> Children { get; }
    }

    /// <summary>
    /// Interface for TreeViewItems to be used with <see cref="TreeViewBase{TSelectedItem, TEnumerable}"/>
    /// </summary>
    public interface ITreeViewItem
    {
        /// <summary>
        /// Event that occurs when <see cref="IsSelected"/> gets set to TRUE
        /// </summary>
        event EventHandler Selected;

        /// <summary>
        /// Event that occurs when <see cref="IsSelected"/> gets changes from TRUE to FALSE
        /// </summary>
        event EventHandler Deselected;

        /// <summary>
        /// Event that can bubble up when a child has been selected
        /// </summary>
        event EventHandler ChildSelected;

        /// <summary>
        /// Event that occurs when <see cref="IsExpanded"/> gets changes to TRUE
        /// </summary>
        public event EventHandler Expanded;

        /// <summary>
        /// Event that occurs when <see cref="IsExpanded"/> gets changes to FALSE
        /// </summary>
        public event EventHandler Collapsed;

        /// <summary>
        /// Get the name of the item - Used to display the item within the TreeView
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets/Sets if the TreeViewItem is currently selected
        /// </summary>
        /// <remarks>For ViewModel Set functionality, Binding must be set to TwoWay</remarks>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets/Sets if the TreeViewItem is expanded
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Any child TreeViewItems
        /// </summary>
        IEnumerable<ITreeViewItem> Children { get; }

    }
}
