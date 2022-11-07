using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.MvvmControls.DragDrop
{
    /// <summary>
    /// Contains extension methods for DragEventArgs
    /// </summary>
    public static class DragEventArgsExtensions
    {
        /// <summary>
        /// Checks the args for <see cref="DataFormats.FileDrop"/>
        /// </summary>
        public static bool IsFileDrop(this DragEventArgs e)
        {
            return e.Data.GetDataPresent(DataFormats.FileDrop);
        }

        /// <summary>
        /// Checks the args for <see cref="DataFormats.StringFormat"/>
        /// </summary>
        public static bool IsString(this DragEventArgs e)
        {
            return e.Data.GetDataPresent(DataFormats.StringFormat) || IsAnsiText(e);
        }

        /// <summary>
        /// Checks the args for <see cref="DataFormats.Text"/>
        /// </summary>
        public static bool IsAnsiText(this DragEventArgs e)
        {
            return e.Data.GetDataPresent(DataFormats.Text);
        }

        /// <summary>
        /// Checks the args for <see cref="DataFormats.UnicodeText"/>
        /// </summary>
        public static bool IsUniCodeText(this DragEventArgs e)
        {
            return e.Data.GetDataPresent(DataFormats.UnicodeText);
        }

        /// <summary>
        /// Checks the args for <see cref="DataFormats.Rtf"/>
        /// </summary>
        public static bool IsRichText(this DragEventArgs e)
        {
            return e.Data.GetDataPresent(DataFormats.Rtf);
        }

        /// <summary>
        /// Checks the args for <see cref="DataFormats.Html"/>
        /// </summary>
        public static bool IsHTML(this DragEventArgs e)
        {
            return e.Data.GetDataPresent(DataFormats.Html);
        }

        /// <summary>
        /// Attempts to retrieve the string data from the event via <see cref="DataFormats.StringFormat"/>
        /// </summary>
        public static string GetStringData(this DragEventArgs e)
        {
            return e.Data.GetData(DataFormats.StringFormat) as string;
        }

        /// <summary>
        /// Attempts to retrieve the string data from the event via <see cref="DataFormats.Rtf"/>
        /// </summary>
        public static string GetRichText(this DragEventArgs e)
        {
            return e.Data.GetData(DataFormats.Rtf) as string;
        }

        /// <summary>
        /// Attempts to retrieve the string data from the event via <see cref="DataFormats.Html"/>
        /// </summary>
        public static string GetHtmlString(this DragEventArgs e)
        {
            return e.Data.GetData(DataFormats.Html) as string;
        }

        /// <summary>
        /// Attempts to retrieve the string data from the event via <see cref="DataFormats.Text"/>
        /// </summary>
        public static string GetAnsiText(this DragEventArgs e)
        {
            return e.Data.GetData(DataFormats.Text) as string;
        }

    }
}
