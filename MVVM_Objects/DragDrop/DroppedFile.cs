using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.MVVMObjects.DragDrop
{
    /// <summary>
    /// Base class all <see cref="IDroppedFile"/> objects should derive from
    /// </summary>
    public class DroppedFile : AbstractDroppedFile, IDroppedFile
    {
        /// <summary>
        /// 
        /// </summary>
        public DroppedFile(string source, DragEventArgs args) : base(source, args)
        {
            FileInfo = new FileInfo(source);
        }
        
        /// <summary>
        /// The FileInfo object
        /// </summary>
        public FileInfo FileInfo { get; }

        /// <inheritdoc />
        public override string FileExtension => FileInfo.Extension;

        /// <inheritdoc />
        public override string Source => FileInfo.FullName;

        /// <inheritdoc />
        public override void CopyTo(string destination, bool overWrite)
        {
            FileInfo.CopyTo(destination, overWrite);
        }
    }
}
