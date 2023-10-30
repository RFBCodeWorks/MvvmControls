using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RFBCodeWorks.Mvvm.Specialized
{


    /// <summary>
    /// Class that is used to look up the Icon files for associated File Types, and hold them in a dictionary.
    /// <br/> This class returns <see cref="ImageSource"/> objects for use with WPF Bindings
    /// </summary>
    public class IconDictionary : IValueConverter, IDictionary<string, ImageSource>
    {
        /// <inheritdoc/>
        public IconDictionary()
        {
#if WINDOWS || NETFRAMEWORK
            IconDict.Add(DirectoryIconKey, Helpers.IconUtilities.LargeFolderIcon);
#endif
        }

        /// <summary>
        /// The key associated with the Directory icon
        /// </summary>
        public const string DirectoryIconKey = nameof(DirectoryIconKey);

        private Dictionary<string, ImageSource> IconDict { get; } = new Dictionary<string, ImageSource>();

        
        /// <inheritdoc/>
        public int Count => IconDict.Count;

        /// <inheritdoc/>
        public ImageSource this[string key] { get => ((IDictionary<string, ImageSource>)IconDict)[key]; set => ((IDictionary<string, ImageSource>)IconDict)[key] = value; }

        /// <summary>
        /// Clear the dictionary of all items
        /// </summary>
        public void Clear() => IconDict.Clear();
        
        /// <inheritdoc/>
        public bool ContainsKey(string key) => IconDict.ContainsKey(key);

        /// <inheritdoc cref="Dictionary{TKey, TValue}.Add(TKey, TValue)"/>
        public void Add(string key, ImageSource value) => IconDict.Add(key, value);

        /// <inheritdoc cref="Dictionary{TKey, TValue}.Remove(TKey)"/>
        public bool Remove(string key) => IconDict.Remove(key);

        /// <summary>
        /// Attempt to get a icon that has already been added to the dictionary.
        /// </summary>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
        public bool TryGetValue(FileInfo file, out ImageSource value) => IconDict.TryGetValue(file.Extension, out value);

        /// <inheritdoc cref="TryGetValue(FileInfo, out ImageSource)"/>
        /// <remarks>To retreive a directory icon, use <see cref="DirectoryIconKey"/> </remarks>
        /// <inheritdoc/>
        /// <param name="key">A file name, file extension, or <see cref="DirectoryIconKey"/></param>
        /// <param name="value"/>
        public bool TryGetValue(string key, out ImageSource value) => IconDict.TryGetValue(key == DirectoryIconKey ? DirectoryIconKey : Path.GetExtension(key), out value);

        /// <inheritdoc cref="TryAddIcon(string, out ImageSource)"/>
        public bool TryAddIcon(FileInfo file, out ImageSource imageSource) => TryAddIcon(file.FullName, out imageSource);

        /// <returns>True if successfully added or an icon was already present in the dictionary, otherwise false</returns>
        /// <inheritdoc cref="LookupIcon(string, bool)"/>
        public bool TryAddIcon(string filePath, out ImageSource imageSource)
        {
            try
            {
                imageSource = LookupIcon(filePath, true);
                return true;
            }
            catch
            {
                imageSource = null;
                return false;
            }
        }

        /// <summary>
        /// Attempt to retrieve the icon associated with the specified file and add it to the dictionary.
        /// </summary>
        /// <returns>
        /// - If the dictionary already contains an associated icon, that value will be returned. <br/>
        /// - If the dictionary does not contain an icon, locate it and add it to dictionary, then return it.
        /// </returns>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.Add(TKey, TValue)"/>
        public virtual ImageSource LookupIcon(string filePath, bool addToDictionary)
        {
            string key = GetKey(filePath);
            if (IconDict.TryGetValue(key, out ImageSource img))
            {
                return img;
            }

#if WINDOWS || NETFRAMEWORK
            img = Helpers.IconUtilities.ExtractAssociatedIcon(filePath);
            if (key == DirectoryIconKey)
            {
                img = Helpers.IconUtilities.LargeFolderIcon; 
                addToDictionary = true;
            }
#endif

            if (addToDictionary || AutoCachedItems().Contains(key)) IconDict.Add(key, img);
            return img;
        }

        /// <inheritdoc cref="LookupIcon(string, bool)"/>
        /// <returns>True if successful, otherwise false</returns>
        public bool TryLookupIcon(string filePath, bool addToDictionary, out ImageSource image)
        {
            try
            {
                image = LookupIcon(filePath, addToDictionary);
                return image != null;
            }
            catch
            {
                image = null;
                return false;
            }
        }

        private static string GetKey(string filePath)
        {
            if (filePath == DirectoryIconKey) return DirectoryIconKey;
            string key = Path.GetExtension(filePath).ToLower(System.Globalization.CultureInfo.InvariantCulture);
            if (key == ".exe") key = Path.GetFileName(filePath);
            return key;
        }

        /// <summary>
        /// A collection of extensions whose icons will automatically cache on their first successfull lookup request
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<string> AutoCachedItems()
        {
            yield return ".txt";
            yield return ".pdf";
            yield return ".htm";
            yield return ".html";
            yield return ".xml";
            yield return ".rar";
            yield return ".zip";
            yield return ".7z";
            yield break;
        }

        /// <summary>
        /// Retrieve the ImageSource of an Icon for some file or folder
        /// </summary>
        /// <param name="value">The file name, file path or extension to retrieve an icon for. For directories, use <see cref="DirectoryIconKey"/></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">boolean true/false to determine if the item will be added to the dictionary. Default false.</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path;
            if (value is string p) path = p;
            else if (value is FileInfo f) path = f.FullName;
            else if (value is TreeViewFileInfo tf) path = tf.Item.FullName;
            else if (value is DirectoryInfo) path = DirectoryIconKey;
            else if (value is TreeViewDirectoryInfo) path = DirectoryIconKey;
            else return null;

            bool addToDict = false;
            if (parameter is null) addToDict = false;
            else if (parameter is bool b) addToDict = b;
            else if (parameter is string bs && bool.TryParse(bs, out b)) addToDict = b;

            if (TryLookupIcon(path, addToDict, out ImageSource img))
            {
                return img;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Does nothing - returns the <paramref name="value"/>
        /// </summary>
        /// <returns><paramref name="value"/></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;

        ICollection<string> IDictionary<string, ImageSource>.Keys => IconDict.Keys;
        ICollection<ImageSource> IDictionary<string, ImageSource>.Values => IconDict.Values;
        bool ICollection<KeyValuePair<string, ImageSource>>.IsReadOnly => ((ICollection<KeyValuePair<string, ImageSource>>)IconDict).IsReadOnly;
        void ICollection<KeyValuePair<string, ImageSource>>.Add(KeyValuePair<string, ImageSource> item) => ((ICollection<KeyValuePair<string, ImageSource>>)IconDict).Add(item);
        bool ICollection<KeyValuePair<string, ImageSource>>.Contains(KeyValuePair<string, ImageSource> item) => ((ICollection<KeyValuePair<string, ImageSource>>)IconDict).Contains(item);
        void ICollection<KeyValuePair<string, ImageSource>>.CopyTo(KeyValuePair<string, ImageSource>[] array, int arrayIndex) => ((ICollection<KeyValuePair<string, ImageSource>>)IconDict).CopyTo(array, arrayIndex);
        bool ICollection<KeyValuePair<string, ImageSource>>.Remove(KeyValuePair<string, ImageSource> item) => ((ICollection<KeyValuePair<string, ImageSource>>)IconDict).Remove(item);
        IEnumerator<KeyValuePair<string, ImageSource>> IEnumerable<KeyValuePair<string, ImageSource>>.GetEnumerator() => ((IEnumerable<KeyValuePair<string, ImageSource>>)IconDict).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)IconDict).GetEnumerator();
    }
}
