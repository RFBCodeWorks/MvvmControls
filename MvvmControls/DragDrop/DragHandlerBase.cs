using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.MvvmControls.DragDrop
{
    /// <summary>
    /// Base class that consumers can utilize for custom implementations
    /// </summary>
    public class DragHandlerBase : ObservableObject, IDragHandler
    {

        /// <inheritdoc/>
        /// <remarks>
        /// Set a control's IsHitTestVisible property when dragging
        /// </remarks>
        public bool IsDragging
        {
            get => IsDraggingField;
            protected set
            {
                if (IsDraggingField != value)
                {
                    OnPropertyChanging(IsDraggingArg);
                    IsDraggingField = value;
                    OnPropertyChanged(IsDraggingArg);
                }
            }
        }
        private bool IsDraggingField;
        private static readonly INotifySingletons.INotifyArgSet IsDraggingArg = new INotifySingletons.INotifyArgSet(nameof(IsDragging));

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
