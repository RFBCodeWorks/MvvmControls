using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RFBCodeWorks.Mvvm.Specialized
{
    /// <summary>An ITreeViewItem that represents a FileInfo or DirectoryInfo object</summary>
    public interface IFileOrDirectoryTreeViewItem : ITreeViewItem
    {
        /// <summary> TRUE if the item represents a directory </summary>
        bool IsDirectory { get; }

        /// <summary> TRUE if the item represents a File </summary>
        bool IsFile { get; }

        /// <summary> The underlying FileInfo or DirectoryInfo object </summary>
        FileSystemInfo Item{ get; }

        /// <summary>
        /// Children files or directories
        /// </summary>
        new IFileOrDirectoryTreeViewItem[] Children { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class FileTreeViewModel : TreeViewBase<TreeViewDirectoryInfo, IFileOrDirectoryTreeViewItem>
    {

        /// <inheritdoc/>
        public FileTreeViewModel() : this(new IconDictionary()) { }

        /// <inheritdoc/>
        public FileTreeViewModel(IconDictionary iconDictionary)
        {
            IconDictionary = iconDictionary;
        }
        
        /// <summary>
        /// The IconDictionary that can be used to provides icons to the UI
        /// </summary>
        public IconDictionary IconDictionary { get; }

        /// <inheritdoc cref="SetRootDirectory(DirectoryInfo)"/>
        /// <inheritdoc cref="DirectoryInfo.DirectoryInfo(string)"/>
        public void SetRootDirectory(string path) => SetRootDirectory(new DirectoryInfo(path));

        /// <summary>
        /// Update the Root Directory of the Folder Tree View
        /// </summary>
        /// <param name="dir"></param>
        public void SetRootDirectory(DirectoryInfo dir)
        {
            TreeRoot = new TreeViewDirectoryInfo(dir, IconDictionary);
        }

        /// <summary>
        /// Set a flag to clear the collection and re-populate it. Raises the PropertyChanged event to trigger a UI update.
        /// </summary>
        public void Refresh()
        {
            TreeRoot.Refresh();
        }
    }
}
