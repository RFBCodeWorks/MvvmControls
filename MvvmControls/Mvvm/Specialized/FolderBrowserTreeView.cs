using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Specialized
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileOrDirectoryTreeViewItem : ITreeViewItem
    {
        /// <summary> TRUE if the item represents a directory </summary>
        bool IsDirectory { get; }
        /// <summary> TRUE if the item represents a File </summary>
        bool IsFile { get; }

        /// <summary>
        /// Children files or directories
        /// </summary>
        new IEnumerable<IFileOrDirectoryTreeViewItem> Children { get; }
    }

    /// <summary>
    /// A <see cref="IFileOrDirectoryTreeViewItem"/> that represents a <see cref="System.IO.FileInfo"/> object
    /// </summary>
    public class FileInfoTreeViewItem : ITreeViewItem, IFileOrDirectoryTreeViewItem
    {
        /// <inheritdoc/>
        public FileInfoTreeViewItem(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        /// <summary> The <see cref="System.IO.FileInfo"/> object this <see cref="ITreeViewItem"/> represents </summary>
        public FileInfo FileInfo { get; }

        /// <inheritdoc/>
        public string Name => FileInfo.Name;

        /// <inheritdoc/>
        public bool IsSelected { get; set; }

        /// <summary>Delegate for LINQ</summary>
        public static FileInfoTreeViewItem Create(FileInfo file) => new FileInfoTreeViewItem(file);

        /// <summary>Delegate for LINQ</summary>
        public static IFileOrDirectoryTreeViewItem CreateAsInterface(FileInfo file) => new FileInfoTreeViewItem(file);

        IEnumerable<ITreeViewItem> ITreeViewItem.Children => Array.Empty<ITreeViewItem>();
        IEnumerable<IFileOrDirectoryTreeViewItem> IFileOrDirectoryTreeViewItem.Children => Array.Empty<IFileOrDirectoryTreeViewItem>();
        bool IFileOrDirectoryTreeViewItem.IsDirectory => false;
        bool IFileOrDirectoryTreeViewItem.IsFile => true;
    }

    /// <summary>
    /// A <see cref="IFileOrDirectoryTreeViewItem"/> that represents a <see cref="System.IO.DirectoryInfo"/> object
    /// </summary>
    public class DirectoryInfoTreeViewItem : ITreeViewItem, IFileOrDirectoryTreeViewItem
    {
        /// <inheritdoc/>
        public DirectoryInfoTreeViewItem(DirectoryInfo directoryInfo)
        {
            DirectoryInfo = directoryInfo;
        }

        /// <summary> The <see cref="System.IO.DirectoryInfo"/> object this <see cref="ITreeViewItem"/> represents </summary>
        public DirectoryInfo DirectoryInfo { get; }

        /// <inheritdoc/>
        public string Name => DirectoryInfo.Name;

        /// <inheritdoc/>
        public bool IsSelected { get; set; }

        /// <inheritdoc/>
        public IEnumerable<IFileOrDirectoryTreeViewItem> Children => GetChildren(DirectoryInfo);

        /// <summary>
        /// Enumerate the Directories and Files as <see cref="IFileOrDirectoryTreeViewItem"/>s
        /// </summary>
        public static IEnumerable<IFileOrDirectoryTreeViewItem> GetChildren(DirectoryInfo dir)
        {
            var dirs = dir.EnumerateDirectories().Select(DirectoryInfoTreeViewItem.CreateAsInterface);
            var files = dir.EnumerateFiles().Select(FileInfoTreeViewItem.CreateAsInterface);
            return dirs.Concat(files);
        }

        /// <summary>Delegate for LINQ</summary>
        public static DirectoryInfoTreeViewItem Create(DirectoryInfo dir) => new DirectoryInfoTreeViewItem(dir);

        /// <summary>Delegate for LINQ</summary>
        public static IFileOrDirectoryTreeViewItem CreateAsInterface(DirectoryInfo dir) => new DirectoryInfoTreeViewItem(dir);

        bool IFileOrDirectoryTreeViewItem.IsDirectory => true;
        bool IFileOrDirectoryTreeViewItem.IsFile => false;
        IEnumerable<ITreeViewItem> ITreeViewItem.Children => GetChildren(DirectoryInfo);
        
    }

    /// <summary>
    /// 
    /// </summary>
    public class FolderBrowserTreeView : Primitives.AbstractTreeView<IFileOrDirectoryTreeViewItem>
    {

        public void SetRootDirectory(string path) => SetRootDirectory(new DirectoryInfo(path));
        public void SetRootDirectory(DirectoryInfo dir)
        {
            Items.Clear();
            DirectoryInfoTreeViewItem.GetChildren(dir).ToList().ForEach(Items.Add);
        }
    }
}
