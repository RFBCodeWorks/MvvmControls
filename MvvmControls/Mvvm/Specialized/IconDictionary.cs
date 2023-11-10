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
    public class IconDictionary : IValueConverter, IDictionary<string, ImageSource>, IIconProvider
    {
        /// <inheritdoc/>
        public IconDictionary()
        {
#if WINDOWS || NETFRAMEWORK
            IconLoader = Helpers.IconUtilities.IconLoader;
#endif
        }

        /// <summary>
        /// The key associated with the Directory icon
        /// </summary>
        public const string DirectoryIconKey = nameof(DirectoryIconKey);

        private Dictionary<string, ImageSource> IconDict { get; } = new Dictionary<string, ImageSource>();

        /// <summary>
        /// The object that can locate icons.
        /// </summary>
        public IIconProvider IconLoader { get; set; }
        
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
        /// Attempt to get a icon that has already been added to the dictionary (based on File Extension)
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
        /// <inheritdoc cref="GetFileIconFromPath(string, bool)"/>
        public bool TryAddIcon(string filePath, out ImageSource imageSource)
        {
            try
            {
                imageSource = GetFileIconFromPath(filePath, true);
                return true;
            }
            catch
            {
                imageSource = null;
                return false;
            }
        }

        /// <summary>
        /// Attempt to retrieve the icon associated with the specified file and optionally add it to the dictionary.
        /// </summary>
        /// <returns>
        /// - If the dictionary already contains an associated icon, that value will be returned. <br/>
        /// - If the dictionary does not contain an icon, try to use the <see cref="IconLoader"/> to locate the icon.
        /// </returns>
        /// <inheritdoc cref="Dictionary{TKey, TValue}.Add(TKey, TValue)"/>
        public ImageSource GetFileIconFromPath(string filePath, bool addToDictionary)
        {
            string key = GetKey(filePath);
            if (IconDict.TryGetValue(key, out ImageSource img))
            {
                return img;
            }
            if (IconLoader is not null)
            {
                if (key == DirectoryIconKey)
                {
                    img = IconLoader.GetDirectoryIcon();
                    addToDictionary = true;
                }
                else
                {
                    img = IconLoader.GetFileIcon(filePath);
                }
            }
            if (img is not null && (addToDictionary || ShouldAutoCacheIcon(key)))
            {
                IconDict.Add(key, img);
            }
            return img;
        }

        /// <inheritdoc cref="GetFileIconFromPath(string, bool)"/>
        /// <returns>True if successful, otherwise false</returns>
        public bool TryGetFileIconFromPath(string filePath, bool addToDictionary, out ImageSource image)
        {
            try
            {
                image = GetFileIconFromPath(filePath, addToDictionary);
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
            string key = filePath;
            if (Path.HasExtension(filePath))
            {
                key = Path.GetExtension(filePath).ToLower(System.Globalization.CultureInfo.InvariantCulture);
                if (key == ".exe") return filePath;
            }
            return key;
        }

        /// <summary>
        /// Determine if the key should be automatically cached into the dictionary.
        /// </summary>
        /// <returns>True if the key should be automatically added to the dictionary, otherwise false.</returns>
        /// <remarks>
        /// Default implementation returns true if key matches any of the following:
        /// <br/> - .txt
        /// <br/> - .pdf
        /// <br/> - .htm | .html | .xml
        /// <br/> - .zip | .7z | .rar ( archive files )
        /// <br/> - .cfg
        /// <br/> - .py
        /// <br/> - .doc | .docx ( Word Docs )
        /// <br/> - .xlms | .xltm | .xls | .xlts ( Excel Workbooks )
        /// </remarks>
        protected virtual bool ShouldAutoCacheIcon(string key)
        {
            return DefaultExtensions().Contains(key);

            static IEnumerable<string> DefaultExtensions()
            {
                yield return DirectoryIconKey;
                yield return ".txt";
                yield return ".pdf";
                yield return ".htm";
                yield return ".html";
                yield return ".xml";
                yield return ".rar";
                yield return ".zip";
                yield return ".7z";
                yield return ".cfg";
                yield return ".py";
                yield return ".doc";
                yield return ".docx";
                yield return ".xls";
                yield return ".xlsm";
                yield return ".xltm";
                yield return ".xlts";
                yield break;
            }
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
            else if (value is FileInfoTreeViewItem tf) path = tf.Item.FullName;
            else if (value is DirectoryInfo) path = DirectoryIconKey;
            else if (value is DirectoryInfoTreeViewItem) path = DirectoryIconKey;
            else return null;

            bool addToDict = false;
            if (parameter is null) addToDict = false;
            else if (parameter is bool b) addToDict = b;
            else if (parameter is string bs && bool.TryParse(bs, out b)) addToDict = b;

            if (TryGetFileIconFromPath(path, addToDict, out ImageSource img))
            {
                return img;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <exception cref="NotImplementedException"/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICollection<string> Keys => IconDict.Keys;
        
        /// <inheritdoc cref="Dictionary{TKey, TValue}.Values"/>
        public ICollection<ImageSource> Values => IconDict.Values;

        /// <inheritdoc cref="Dictionary{TKey, TValue}.GetEnumerator"/>
        public IEnumerator<KeyValuePair<string, ImageSource>> GetEnumerator() => IconDict.GetEnumerator();

        bool ICollection<KeyValuePair<string, ImageSource>>.IsReadOnly => ((ICollection<KeyValuePair<string, ImageSource>>)IconDict).IsReadOnly;
        void ICollection<KeyValuePair<string, ImageSource>>.Add(KeyValuePair<string, ImageSource> item) => ((ICollection<KeyValuePair<string, ImageSource>>)IconDict).Add(item);
        bool ICollection<KeyValuePair<string, ImageSource>>.Contains(KeyValuePair<string, ImageSource> item) => ((ICollection<KeyValuePair<string, ImageSource>>)IconDict).Contains(item);
        void ICollection<KeyValuePair<string, ImageSource>>.CopyTo(KeyValuePair<string, ImageSource>[] array, int arrayIndex) => ((ICollection<KeyValuePair<string, ImageSource>>)IconDict).CopyTo(array, arrayIndex);
        bool ICollection<KeyValuePair<string, ImageSource>>.Remove(KeyValuePair<string, ImageSource> item) => ((ICollection<KeyValuePair<string, ImageSource>>)IconDict).Remove(item);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)IconDict).GetEnumerator();

        ImageSource IIconProvider.GetFileIcon(string filePath) => GetKeyImage(filePath);
        ImageSource IIconProvider.GetDirectoryIcon(string directoryPath) => IconLoader.GetDirectoryIcon(directoryPath);
        ImageSource IIconProvider.GetDirectoryIcon() => GetKeyImage(DirectoryIconKey);
        private ImageSource GetKeyImage(string path)
        {
            if (TryGetFileIconFromPath(path, false, out ImageSource image))
                return image;
            return null;
        }
    }
}
