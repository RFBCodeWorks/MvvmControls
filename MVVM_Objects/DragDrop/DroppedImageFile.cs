using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.MVVMObjects.DragDrop
{
    /// <summary>
    /// 
    /// </summary>
    public class DroppedImageFile : DroppedFile
    {
        /// <summary>
        /// 
        /// </summary>
        public DroppedImageFile(string filepath, DragEventArgs e) : base(filepath, e)
        {

        }
    }
}
