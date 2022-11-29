using System.Windows;

namespace RFBCodeWorks.DragAndDrop
{

    /// <summary>
    /// Base class for a FileDrop Target portion of a ViewModel
    /// </summary>
    public abstract class FileDropTarget : DragHandlerBase, IFileDragDropTarget, IFileDropTarget
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
    public abstract class MultiFileDropTarget : DragHandlerBase, IMultiFileDragDropTarget, IMultiFileDropTarget
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
