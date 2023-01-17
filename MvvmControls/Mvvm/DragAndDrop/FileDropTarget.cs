using System.Windows;

namespace RFBCodeWorks.Mvvm.DragAndDrop
{

    /// <summary>
    /// Base class for a FileDrop Target portion of a ViewModel
    /// </summary>
    public abstract class FileDropTarget : DragHandler, IFileDragDropTarget, IFileDropTarget
    {
        /// <inheritdoc/>
        public virtual void OnPreviewFileDrop(object sender, IDroppedFile e)
        {

        }
        
        /// <inheritdoc/>
        public virtual void OnFileDrop(object sender, IDroppedFile e)
        {
            IsDragging = false;
        }
    }

    /// <summary>
    /// Base class for a FileDrop Target portion of a ViewModel
    /// </summary>
    public abstract class MultiFileDropTarget : DragHandler, IMultiFileDragDropTarget, IMultiFileDropTarget
    {
        /// <inheritdoc/>
        public virtual void OnPreviewFileDrop(object sender, IDroppedFile[] e)
        {

        }

        /// <inheritdoc/>
        public virtual void OnFileDrop(object sender, IDroppedFile[] e)
        {
            IsDragging = false;
        }
    }
}
