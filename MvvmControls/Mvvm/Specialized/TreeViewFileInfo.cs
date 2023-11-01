using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RFBCodeWorks.Mvvm.Specialized
{
    /// <summary>
    /// A <see cref="IFileOrDirectoryTreeViewItem"/> that represents a <see cref="System.IO.FileInfo"/> object
    /// </summary>
    public class TreeViewFileInfo : TreeViewItemBase<FileInfo, TreeViewDirectoryInfo>, IFileOrDirectoryTreeViewItem
    {
        /// <inheritdoc/>
        public TreeViewFileInfo(FileInfo fileInfo) : base(fileInfo)
        {
            LazyIcon = new Lazy<ImageSource>(GetIcon);
        }

        /// <summary>
        /// Create the object, applying the icon from the <paramref name="iconProvider"/>
        /// </summary>
        public TreeViewFileInfo(FileInfo fileInfo, IIconProvider iconProvider) : this(fileInfo)
        {
            IconProvider = iconProvider;
        }

        private Lazy<ImageSource> LazyIcon;
        private readonly IIconProvider IconProvider;

        /// <summary> The <see cref="System.IO.FileInfo"/> object this <see cref="ITreeViewItem"/> represents </summary>
        public override FileInfo Item => base.Item;

        /// <inheritdoc/>
        public override string Name => Item.Name;

        /// <summary> An image to show on the WPF window - Must be specified by consumer if not created by <see cref="FileTreeViewModel"/> </summary>
        public ImageSource Icon => LazyIcon.Value;

        /// <summary> Apply an icon from the specified dictionary to this object's Icon property. </summary>
        /// <inheritdoc cref="IconDictionary.TryAddIcon(FileInfo, out ImageSource)"/>
        private ImageSource GetIcon()
        {
            return Item.Exists ? IconProvider?.GetFileIcon(Item.FullName) : null;
        }

        /// <summary>
        /// Refresh this object's properties
        /// </summary>
        public virtual void Refresh()
        {
            Item.Refresh();
            if (LazyIcon.IsValueCreated) LazyIcon = new Lazy<ImageSource>(GetIcon);
            OnPropertyChanged(string.Empty);
        }

        /// <summary>Delegate for LINQ</summary>
        public static TreeViewFileInfo Create(FileInfo file) => new TreeViewFileInfo(file);

        /// <summary>Delegate for LINQ</summary>
        public static IFileOrDirectoryTreeViewItem CreateAsInterface(FileInfo file) => new TreeViewFileInfo(file);

        IFileOrDirectoryTreeViewItem[] IFileOrDirectoryTreeViewItem.Children => Array.Empty<IFileOrDirectoryTreeViewItem>();
        bool IFileOrDirectoryTreeViewItem.IsDirectory => false;
        bool IFileOrDirectoryTreeViewItem.IsFile => true;
        FileSystemInfo IFileOrDirectoryTreeViewItem.Item => Item;
    }
}
