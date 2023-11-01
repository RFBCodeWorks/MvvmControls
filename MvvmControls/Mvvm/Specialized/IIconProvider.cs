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
    public interface IIconProvider
    {

        /// <summary>
        /// Get an icon as an image source for the specified <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">The file to get an icon for</param>
        /// <returns>An ImageSource object</returns>
        ImageSource GetFileIcon(string filePath);

        /// <summary>
        /// Gets the icon associated with the specified <paramref name="directoryPath"/>
        /// </summary>
        /// <returns></returns>
        ImageSource GetDirectoryIcon(string directoryPath);

        /// <summary>
        /// Gets the default icon that represents a directory.
        /// </summary>
        /// <returns></returns>
        ImageSource GetDirectoryIcon();
    }
}
