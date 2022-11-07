using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.MvvmControls.DragDrop
{
    /// <summary>
    /// Interface that combines both <see cref="IDragHandler"/> and <see cref="IFileDropTarget"/>
    /// </summary>
    public interface IFileDragDropTarget : IDragHandler, IFileDropTarget
    {

    }

    /// <summary>
    /// Interface that combines both <see cref="IDragHandler"/> and <see cref="IMultiFileDropTarget"/>
    /// </summary>
    public interface IMultiFileDragDropTarget : IDragHandler, IMultiFileDropTarget
    {

    }

    /// <summary>
    /// Interface for interacting with Drag &amp; Drop Handling
    /// </summary>
    public interface IDragHandler : INotifyPropertyChanged
    {
        /// <summary>
        /// TRUE when DragEnter, False after the DragLeave
        /// </summary>
        public bool IsDragging { get; }

        /// <summary> Occurs when the drag starts (after the preview finishes) </summary>
        /// <remarks>Bubbling ( Source Control -> Parent -> Window )</remarks>
        public void OnDragEnter(object sender, DragEventArgs e);

        /// <summary> Occurs when the cursor leaves the area ( after the Preview ) </summary>
        /// <remarks>Bubbling ( Source Control -> Parent -> Window )</remarks>
        public void OnDragLeave(object sender, DragEventArgs e);

        /// <summary> Occurs when the drag starts </summary>
        /// <remarks>TUNNELING ( Window -> Child -> Source Control )</remarks>
        public void OnPreviewDragEnter(object sender, DragEventArgs e);

        /// <summary> Occurs when the cursor leaves the area </summary>
        /// <remarks>TUNNELING ( Window -> Child -> Source Control )</remarks>
        public void OnPreviewDragLeave(object sender, DragEventArgs e);

        /// <summary>
        /// Occurs continuously while dragging an object across the control. <br/>
        /// This is where the <see cref="DragEventArgs.Effects"/> should be evaluated. <br/>
        /// Any resources displayed used should be cached to avoid excessive resourse consumption.
        /// </summary>
        /// <remarks>Bubbling ( Source Control -> Parent -> Window )</remarks>
        public void OnGiveFeedback(object sender, GiveFeedbackEventArgs e);

        /// <summary>
        /// Occurs continuously while dragging an object across the control. <br/>
        /// This is where the <see cref="DragEventArgs.Effects"/> should be set. <br/>
        /// This method should be short to ensure responsiveness of the UI
        /// </summary>
        /// <remarks>Bubbling ( Source Control -> Parent -> Window )</remarks>
        public void OnDragOver(object sender, DragEventArgs e);

        ///// <summary>
        ///// Occurs continuously while dragging an object across the control.
        ///// </summary>
        ///// <remarks>TUNNELING ( Window -> Child -> Source Control )</remarks>
        //public void OnPreviewGiveFeedback(object sender, DragEventArgs e);

        ///// <summary>
        ///// Occurs continuously while dragging an object across the control.
        ///// </summary>
        ///// <remarks>TUNNELING ( Window -> Child -> Source Control )</remarks>
        //public void OnPreviewDragOver(object sender, DragEventArgs e);
    }

    /// <summary>
    /// Target that will accept a file being dropped onto it
    /// </summary>
    public interface IDropTarget
    {
        /// <summary>
        /// Preview the Drop
        /// </summary>
        /// <remarks>TUNNELING ( Window -> Child -> Source Control )</remarks>
        public void OnPreviewFileDrop(object sender, DragEventArgs e);

        /// <summary>
        /// Handle the Drop
        /// </summary>
        /// <remarks>Bubbling ( Source Control -> Parent -> Window )</remarks>
        public void OnFileDrop(object sender, DragEventArgs e);
    }

    /// <summary>
    /// Target that will accept a file being dropped onto it
    /// </summary>
    public interface IFileDropTarget
    {
        /// <summary>
        /// Preview a file being dropped onto this target
        /// </summary>
        /// <remarks>TUNNELING ( Window -> Child -> Source Control )</remarks>
        public void OnPreviewFileDrop(object sender, IDroppedFile e);

        /// <summary>
        /// Handle a file being dropped onto this target
        /// </summary>
        /// <remarks>Bubbling ( Source Control -> Parent -> Window )</remarks>
        public void OnFileDrop(object sender, IDroppedFile e);
    }

    /// <summary>
    /// Target that will accept an multiple files being dropped onto it
    /// </summary>
    public interface IMultiFileDropTarget
    {
        /// <inheritdoc cref="IFileDropTarget.OnPreviewFileDrop"/>
        public void OnPreviewFileDrop(object sender, IDroppedFile[] e);

        /// <inheritdoc cref="IFileDropTarget.OnFileDrop"/>
        public void OnFileDrop(object sender, IDroppedFile[] e);

    }
}
