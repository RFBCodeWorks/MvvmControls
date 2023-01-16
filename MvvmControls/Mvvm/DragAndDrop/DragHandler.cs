using RFBCodeWorks.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.Mvvm.DragAndDrop
{
    /// <summary>
    /// Base class that consumers can utilize for custom implementations
    /// </summary>
    public class DragHandler : ObservableObject, IDragHandler
    {
        private static readonly INotifyArgs IsDraggingArg = new(nameof(IsDragging));

        /// <inheritdoc/>
        /// <remarks>
        /// Can be used to set a rectangle's IsHitTestVisible property when dragging to have a rectangle overlay represent the drop zone
        /// </remarks>
        public bool IsDragging
        {
            get => IsDraggingField;
            protected set
            {
                if (IsDraggingField != value)
                {
                    OnPropertyChanging(IsDraggingArg.PropertyChangingArgs);
                    IsDraggingField = value;
                    OnPropertyChanged(IsDraggingArg.PropertyChangedArgs);
                }
            }
        }
        private bool IsDraggingField;

        /// <inheritdoc/>
        public virtual void OnDragEnter(object sender, DragEventArgs e)
        {
            IsDragging = true;
        }

        /// <inheritdoc/>
        public void OnDragLeave(object sender, DragEventArgs e)
        {
            // Default does nothing
        }

        /// <inheritdoc/>
        public virtual void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            IsDragging = true;
        }

        /// <inheritdoc/>
        public virtual void OnPreviewDragLeave(object sender, DragEventArgs e)
        {
            IsDragging = false;
        }

        /// <inheritdoc/>
        public virtual void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            // Default does nothing
        }

        /// <inheritdoc/>
        public virtual void OnDragOver(object sender, DragEventArgs e)
        {
            // Default does nothing
        }
    }
}
