using System.Windows;


namespace RFBCodeWorks.Mvvm.DragAndDrop
{
    /// <summary>
    /// Defines the Drag Source Object
    /// </summary>
    public interface IDroppedFile
    {
        /// <summary>
        /// The DragEventArgs this object was created from
        /// </summary>
        public DragEventArgs DragEventArgs { get; }
        
        /// <summary>
        /// Get the FileExtension of the file, if any
        /// </summary>
        public string FileExtension { get; }

        /// <summary>
        /// The File/Folder Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Get the File Source (path)
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Copy the file to some location
        /// </summary>
        public void CopyTo(string destination);

        /// <summary>
        /// Copy the file to some location
        /// </summary>
        public void CopyTo(string destination, bool overWrite);

    }
}
