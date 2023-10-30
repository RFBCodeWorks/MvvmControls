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
        /// <param name="iconDictionary">An IconDictionay used when creating child <see cref="TreeViewFileInfo"/></param>
        public TreeViewDirectoryInfo(DirectoryInfo directoryInfo, IconDictionary iconDictionary) : this(directoryInfo)
        {
            IconDictionary = iconDictionary;
        }

        private readonly IconDictionary IconDictionary;
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

        private IFileOrDirectoryTreeViewItem[] GetChildren()
        {
            IFileOrDirectoryTreeViewItem[] collection =
                Item.EnumerateDirectories().Select(CreateChild)
                .Concat(Item.EnumerateFiles().Select(CreateChild))
                .ToArray();

            foreach (IFileOrDirectoryTreeViewItem f in collection)
            {
                SubscribeToChild(f);
            }
            return collection;
        }

        /// <summary> Apply an icon from the specified dictionary to this object's Icon property. </summary>
        /// <inheritdoc cref="IconDictionary.TryAddIcon(FileInfo, out ImageSource)"/>
        private ImageSource GetIcon()
        {
            if (IconDictionary.TryLookupIcon(IconDictionary.DirectoryIconKey, false, out ImageSource img)) return img;
            return null;
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

        /// <summary>Delegate for LINQ</summary>
        public static TreeViewDirectoryInfo Create(DirectoryInfo dir) => new TreeViewDirectoryInfo(dir);

        /// <summary>Delegate for LINQ</summary>
        public static IFileOrDirectoryTreeViewItem CreateAsInterface(DirectoryInfo dir) => new TreeViewDirectoryInfo(dir);

        private IFileOrDirectoryTreeViewItem CreateChild(DirectoryInfo directory) => new TreeViewDirectoryInfo(directory, IconDictionary) { Parent = this };
        private IFileOrDirectoryTreeViewItem CreateChild(FileInfo file) => new TreeViewFileInfo(file, IconDictionary) { Parent = this };

        bool IFileOrDirectoryTreeViewItem.IsDirectory => true;
        bool IFileOrDirectoryTreeViewItem.IsFile => false;
        FileSystemInfo IFileOrDirectoryTreeViewItem.Item => Item;
        IEnumerable<IFileOrDirectoryTreeViewItem> ITreeViewItem<IFileOrDirectoryTreeViewItem>.Children => Children;
    }

}
