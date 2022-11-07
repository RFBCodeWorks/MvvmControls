using System.Windows;

namespace RFBCodeWorks.MvvmControls.DragDrop
{

    /// <summary>
    /// Base class for a FileDrop Target portion of a ViewModel
    /// </summary>
    public abstract class DragHandler : RFBCodeWorks.MvvmControls.ViewModelBase, IDragHandler
    {
        /// <inheritdoc/>
        public bool IsDragging
        {
            get { return IsDraggingField; }
            set { SetProperty(ref IsDraggingField, value, nameof(IsDragging)); }
        }
        private bool IsDraggingField;


        /// <inheritdoc/>
        public virtual void OnDragEnter(object sender, DragEventArgs e)
        {
            IsDragging = true;
        }

        /// <inheritdoc/>
        public virtual void OnDragLeave(object sender, DragEventArgs e)
        {
            IsDragging = false;
        }

        /// <inheritdoc/>
        public virtual void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            IsDragging = true;
        }

        /// <inheritdoc/>
        public virtual void OnPreviewDragLeave(object sender, DragEventArgs e)
        {

        }

        /// <inheritdoc/>
        public virtual void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }

        /// <inheritdoc/>
        public virtual void OnDragOver(object sender, DragEventArgs e)
        {

        }
    }

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
