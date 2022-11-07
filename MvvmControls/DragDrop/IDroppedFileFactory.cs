using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace RFBCodeWorks.MvvmControls.DragDrop
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDroppedFileFactory
    {

        /// <summary>
        /// Evaluate the args and check if it contains a valid file object that can be converted using the factory
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsMultiFileDrop(this DragEventArgs e)
        {
            var fd = e.Data.GetData(DataFormats.FileDrop) as string[];   //This is a collection of files/folders
            if (fd.IsNull() || fd.Length == 0) return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string[] ImageFileExtensions { get; } = new string[] { ".bmp", ".jpg", ".jpeg", ".gif", ".png" };

        /// <summary>
        /// Convert the args into a new IDroppedFile object
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IDroppedFile[] GetDroppedFiles(this DragEventArgs e)
        {
            var fd = e.Data.GetData(DataFormats.FileDrop) as string[];   //This is a collection of files/folders
            
            if (fd.IsNull() || fd.Length == 0 ) return null;

            IDroppedFile[] files = Array.CreateInstance(typeof(IDroppedFile), fd.Length) as IDroppedFile[];
            foreach(string file in fd)
            {
                files[fd.IndexOf(file)] = CreateDroppedFile(file, e);
            }

            return files;
        }

        /// <summary>
        /// Get the first Dropped File from the list
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IDroppedFile GetDroppedFile(this DragEventArgs e)
        {
            return GetDroppedFiles(e)?[0];
        }

        /// <summary>
        /// Create a new DroppedFile from the filepath and the args
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IDroppedFile CreateDroppedFile(string filePath, DragEventArgs e)
        {
            string ext = Path.GetExtension(filePath);
            switch (true)
            {
                case true when ext == ".zip":
                    return new DroppedZipFile(filePath, e);
                case true when ext == ".txt" || ext == ".rtl" || ext.ToLower() == ".rstxt":
                    return new DroppedTextFile(filePath, e);
                case true when ImageFileExtensions.Contains(ext):
                    return new DroppedImageFile(filePath, e);
                case true when Directory.Exists(filePath):
                    return new DroppedFolder(filePath, e);
                case true when ext == ".lnk":
                default:
                    return new DroppedFile(filePath, e);
            }
        }
    }
}
