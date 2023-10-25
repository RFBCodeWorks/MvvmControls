using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Interface for TreeViewItems to be used with <see cref="Primitives.AbstractTreeView{TSelectedItem, TEnumerable, TSelectedValue}"/>
    /// </summary>
    public interface ITreeViewItem
    {
        /// <summary>
        /// Get the name of the item - Used to display the item within the TreeView
        /// </summary>
        string Name { get; }

        /// <summary>
        /// This will be set True/False by the WPF control if bound
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Any child TreeViewItems
        /// </summary>
        IEnumerable<ITreeViewItem> Children { get; }

    }
}
