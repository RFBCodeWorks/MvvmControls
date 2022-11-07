using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.MvvmControls.DragDrop
{
    /// <summary>
    /// Represents a folder that was dropped
    /// </summary>
    public class DroppedFolder : AbstractDroppedFile
    {
        const string ext = "Folder";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="e"></param>
        public DroppedFolder(string path, DragEventArgs e) : base(path, e)
        {
            DirInfo = new DirectoryInfo(path);
        }

        /// <summary>
        /// The DirectoryInfo object
        /// </summary>
        public DirectoryInfo DirInfo { get; }

        /// <summary>
        /// This class is a folder, so no file extension exists
        /// </summary>
        public sealed override string FileExtension => DirInfo.Extension;

        /// <summary>
        /// This method is not implemented for <see cref="DroppedFolder"/> objects.
        /// </summary>
        public override void CopyTo(string destination, bool overWrite)
        {
            
        }
    }
}
