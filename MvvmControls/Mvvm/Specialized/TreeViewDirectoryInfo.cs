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
    /// A <see cref="IFileOrDirectoryTreeViewItem"/> that represents a <see cref="System.IO.DirectoryInfo"/> object
    /// </summary>
    public class TreeViewDirectoryInfo : TreeViewItemBase<DirectoryInfo, TreeViewDirectoryInfo>, IFileOrDirectoryTreeViewItem, ITreeViewItem<IFileOrDirectoryTreeViewItem>
    {
        /// <inheritdoc/>
        public TreeViewDirectoryInfo(DirectoryInfo directoryInfo) : base(directoryInfo)
        {
            LazyChildren = new Lazy<IFileOrDirectoryTreeViewItem[]>(GetChildren);
            LazyIcon = new Lazy<ImageSource>(GetIcon);
        }


        /// <param name="directoryInfo"></param>
        /// <param name="iconProvider">An IconDictionay used when creating child <see cref="TreeViewFileInfo"/></param>
        public TreeViewDirectoryInfo(DirectoryInfo directoryInfo, IIconProvider iconProvider) : this(directoryInfo)
        {
            IconProvider = iconProvider;
        }

        private readonly IIconProvider IconProvider;
        private readonly Lazy<ImageSource> LazyIcon;
        private Lazy<IFileOrDirectoryTreeViewItem[]> LazyChildren;
        

        /// <summary> The <see cref="System.IO.DirectoryInfo"/> object this <see cref="ITreeViewItem"/> represents </summary>
        public override DirectoryInfo Item => base.Item;

        /// <inheritdoc/>
        public override string Name => Item.Name;

        /// <summary> An image to show on the WPF window - Must be specified by consumer if not created by <see cref="FileTreeViewModel"/> </summary>
        public ImageSource Icon => LazyIcon.Value;


        /// <inheritdoc/>
        public IFileOrDirectoryTreeViewItem[] Children => LazyChildren.Value;

        /// <inheritdoc/>
        protected override IEnumerable<ITreeViewItem> ITreeViewChildren => Children;

        protected virtual IFileOrDirectoryTreeViewItem[] GetChildren()
        {
            IFileOrDirectoryTreeViewItem[] collection =
                GetSubdirectories()
                .Concat(GetFiles())
                .ToArray();

            foreach (IFileOrDirectoryTreeViewItem f in collection)
            {
                SubscribeToChild(f);
            }
            return collection;
        }

        /// <summary>
        /// Get this directory's subdirectories
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<IFileOrDirectoryTreeViewItem> GetSubdirectories()
        {
            return Item.EnumerateDirectories().Select(CreateChild);
        }

        /// <summary>
        /// Get this directory's files
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<IFileOrDirectoryTreeViewItem> GetFiles()
        {
            return Item.EnumerateFiles().Select(CreateChild);
        }

        /// <summary> Apply an icon from the specified dictionary to this object's Icon property. </summary>
        /// <inheritdoc cref="IconDictionary.TryAddIcon(FileInfo, out ImageSource)"/>
        protected virtual ImageSource GetIcon()
        {
            return IconProvider.GetDirectoryIcon(Item.FullName);
        }

        /// <summary>
        /// Refresh the DirectoryInfo object and its children
        /// </summary>
        public void Refresh()
        {
            Item.Refresh();
            if (LazyChildren.IsValueCreated)
            {
                foreach (var child in Children)
                    UnsubscribeFromChild(child);
            }
            LazyChildren = new Lazy<IFileOrDirectoryTreeViewItem[]>(GetChildren);
            //if (LazyIcon.IsValueCreated) LazyIcon = new Lazy<ImageSource>(GetIcon);
            OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Create a child directory object
        /// </summary>
        protected virtual IFileOrDirectoryTreeViewItem CreateChild(DirectoryInfo directory) => new TreeViewDirectoryInfo(directory, IconProvider) { Parent = this };

        /// <summary>
        /// Create a child File object
        /// </summary>
        protected virtual IFileOrDirectoryTreeViewItem CreateChild(FileInfo file) => new TreeViewFileInfo(file, IconProvider) { Parent = this };

        bool IFileOrDirectoryTreeViewItem.IsDirectory => true;
        bool IFileOrDirectoryTreeViewItem.IsFile => false;
        FileSystemInfo IFileOrDirectoryTreeViewItem.Item => Item;
        IEnumerable<IFileOrDirectoryTreeViewItem> ITreeViewItem<IFileOrDirectoryTreeViewItem>.Children => Children;
    }

}
