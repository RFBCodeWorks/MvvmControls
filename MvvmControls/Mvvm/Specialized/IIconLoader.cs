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
    /// Interface used by the <see cref="IconDictionary"/> to load ImageSources for file/folders
    /// </summary>
    public interface IIconLoader
    {
        /// <summary>
        /// Get an icon as an image source for the specified <paramref name="file"/>
        /// </summary>
        /// <param name="file">The file to get an icon for</param>
        /// <returns>An ImageSource object</returns>
        #if NETCOREAPP3_0_OR_GREATER
        ImageSource GetIcon(FileInfo file) => GetIcon(file.FullName);
        #else
        ImageSource GetIcon(FileInfo file);
        #endif

        /// <summary>
        /// Get an icon as an image source for the specified <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">The file to get an icon for</param>
        /// <returns>An ImageSource object</returns>
        ImageSource GetIcon(string filePath);

        /// <summary>
        /// Gets the default icon that represents a directory.
        /// </summary>
        /// <returns></returns>
        ImageSource GetDirectoryIcon();
    }
}
