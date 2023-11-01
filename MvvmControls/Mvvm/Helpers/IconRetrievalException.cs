using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Helpers
{
    /// <summary>
    /// An exception that occurs when an <see cref="Specialized.IIconProvider"/> encounters an exception when getting an icon
    /// </summary>
    public class IconRetrievalException : Exception
    {
        /// <inheritdoc/>
        public IconRetrievalException(string message, string path, Exception innerException)
            : base(message, innerException)
        {
            Path = path;
        }

        /// <summary>
        /// The path that was submitted when attempting to retrieve the icon
        /// </summary>
        public string Path { get; }
    }
}
