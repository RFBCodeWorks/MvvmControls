using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.MVVMObjects.DragDrop
{
    /// <summary>
    /// Represents a string that was dropped onto the target
    /// </summary>
    public class DroppedTextFile : DroppedFile
    {
        /// <summary>
        /// 
        /// </summary>
        public DroppedTextFile(string filepath, DragEventArgs e) : base(filepath, e)
        {

        }

        
        /// <summary>
        /// 
        /// </summary>
        public string[] Lines { get
            {
                if (linesField is null)
                    linesField = System.IO.File.ReadLines(base.Source).ToArray();
                return linesField;
            }
        }
        private string[] linesField;


        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get
            {
                if (textField is null)
                    textField = System.IO.File.ReadAllText(base.Source);
                return textField;
            }
        }
        private string textField;

    }
}
