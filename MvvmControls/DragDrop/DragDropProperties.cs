using System;
using System.Windows;

namespace RFBCodeWorks.MvvmControls.DragDrop
{
    /// <summary>
    /// Static Methods and properties for this implementation
    /// </summary>
    public static class DragDropProperties
    {

        #region < IsFileDropTarget >

        /// <summary> </summary>
        public static bool GetIsFileDropTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFileDropTargetProperty);
        }

        /// <summary> </summary>
        public static void SetIsFileDropTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFileDropTargetProperty, value);
        }

        /// <summary>
        /// Enable/Disable the File Drop functionality
        /// </summary>
        // Using a DependencyProperty as the backing store for IsFileDropTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFileDropTargetProperty =
            DependencyProperty.RegisterAttached(nameof(IsFileDropTargetProperty), typeof(bool), typeof(DragDropProperties), new PropertyMetadata(false,OnIsFileDropTargetChanged));

        private static void OnIsFileDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && d is UIElement uiElement)
            {
                uiElement.AllowDrop = (bool)e.NewValue;

                UnregisterDragDropEvents(uiElement);

                if ((bool)e.NewValue)
                {
                    RegisterDragDropEvents(uiElement);
                }
            }
        }

        #endregion

        #region < Drop Target Assignment >


        /// <summary> </summary>
        public static IDragHandler GetDragHandler(DependencyObject obj)
        {
            return (IDragHandler)obj.GetValue(DragHandlerProperty);
        }
        /// <summary> </summary>
        public static void SetDragHandler(DependencyObject obj, IDragHandler value)
        {
            obj.SetValue(DragHandlerProperty, value);
        }

        /// <summary> 
        /// 
        /// </summary>
        // Using a DependencyProperty as the backing store for FileDropTarget.  This enables animation, styling, binding, etc...
        public static DependencyProperty DragHandlerProperty=
            DependencyProperty.RegisterAttached("DragHandler", typeof(IDragHandler), typeof(DragDropProperties), new PropertyMetadata(null, OnDragHandlerChanged));

        private static void OnDragHandlerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && d is UIElement uiElement)
            {
                uiElement.AllowDrop = e.NewValue != null;

                UnregisterDragDropEvents(uiElement);

                if (e.NewValue.IsNotNull())
                {
                    RegisterDragDropEvents(uiElement);
                }
            }
        }

        #endregion

        #region < Drop Target Assignment >


        /// <summary> </summary>
        public static IFileDropTarget GetFileDropHandler(DependencyObject obj)
        {
            return (IFileDropTarget)obj.GetValue(FileDropHandlerProperty);
        }
        /// <summary> </summary>
        public static void SetFileDropHandler(DependencyObject obj, IFileDropTarget value)
        {
            obj.SetValue(FileDropHandlerProperty, value);
        }

        /// <summary> 
        /// 
        /// </summary>
        // Using a DependencyProperty as the backing store for FileDropTarget.  This enables animation, styling, binding, etc...
        public static DependencyProperty FileDropHandlerProperty =
            DependencyProperty.RegisterAttached("FileDropHandler", typeof(IFileDropTarget), typeof(DragDropProperties), new PropertyMetadata(null, OnFileDropHandlerChanged));

        private static void OnFileDropHandlerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && d is UIElement uiElement)
            {
                uiElement.AllowDrop = e.NewValue != null;
                SetIsFileDropTarget(uiElement, uiElement.AllowDrop);

                UnregisterDragDropEvents(uiElement);

                if (e.NewValue.IsNotNull())
                {
                    RegisterDragDropEvents(uiElement);
                }
            }
        }

        #endregion

        #region < Multi-Drop Target Assignment >


        /// <summary> </summary>
        public static IMultiFileDropTarget GetMultiFileDropHandler(DependencyObject obj)
        {
            return (IMultiFileDropTarget)obj.GetValue(MultiFileDropHandlerProperty);
        }
        /// <summary> </summary>
        public static void SetMultiFileDropHandler(DependencyObject obj, IMultiFileDropTarget value)
        {
            obj.SetValue(MultiFileDropHandlerProperty, value);
        }

        /// <summary> 
        /// 
        /// </summary>
        // Using a DependencyProperty as the backing store for FileDropTarget.  This enables animation, styling, binding, etc...
        public static DependencyProperty MultiFileDropHandlerProperty =
            DependencyProperty.RegisterAttached("MultiFileDropHandler", typeof(IMultiFileDropTarget), typeof(DragDropProperties), new PropertyMetadata(null, OnMultiFileDropHandlerChanged));

        private static void OnMultiFileDropHandlerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && d is UIElement uiElement)
            {
                uiElement.AllowDrop = e.NewValue != null;

                UnregisterDragDropEvents(uiElement);

                if (e.NewValue.IsNotNull())
                {
                    RegisterDragDropEvents(uiElement);
                }
            }
        }

        #endregion

        #region < Register / Unregister Event >

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uiElement"></param>
        /// <see href="https://github.com/punker76/gong-wpf-dragdrop/blob/44eedd7acb2a4d200bb13ba782afa375552587d2/src/GongSolutions.WPF.DragDrop/DragDrop.cs"/>
        private static void RegisterDragDropEvents(UIElement uiElement)
        {
            if (GetDragHandler(uiElement).IsNotNull())
            {
                uiElement.PreviewGiveFeedback += OnPreviewGiveFeedback;
                uiElement.GiveFeedback += OnGiveFeedback;

                uiElement.PreviewDragEnter += OnPreviewDragEnter;
                uiElement.DragEnter += OnDragEnter;

                uiElement.PreviewDragLeave += OnPreviewDragLeave;
                uiElement.DragLeave += OnDragLeave;
            }

            if (GetFileDropHandler(uiElement).IsNotNull() || GetMultiFileDropHandler(uiElement).IsNotNull())
            {
                uiElement.PreviewDrop += OnDropPreview;
                uiElement.Drop += OnDrop;
            }
        }

        private static void UnregisterDragDropEvents(UIElement uiElement)
        {
            uiElement.PreviewGiveFeedback-= OnPreviewGiveFeedback;
            uiElement.GiveFeedback -= OnGiveFeedback;

            uiElement.PreviewDragEnter -= OnPreviewDragEnter;
            uiElement.DragEnter -= OnDragEnter;

            uiElement.PreviewDragLeave -= OnPreviewDragLeave;
            uiElement.DragLeave -= OnDragLeave;

            uiElement.PreviewDrop -= OnDropPreview;
            uiElement.Drop -= OnDrop;
        }

        #endregion


        #region < Event Handling >

        /// <summary>
        /// Get the Drop Target and the DroppedFile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>A tuple if able to fire off the event, otherwise null</returns>
        private static Tuple<IFileDropTarget, IDroppedFile> GetDropTarget(object sender, DragEventArgs e)
        {
            if (e.Handled) return null;
            
            var s = sender as DependencyObject;
            if (s is null) return null;
            
            var h = GetFileDropHandler(sender as DependencyObject);
            if (h is null) return null;
            
            var o = IDroppedFileFactory.GetDroppedFile(e);
            if (o is null) return null;
            
            return new(h, o);
        }

        /// <summary>
        /// Get the Drop Target and the DroppedFile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>A tuple if able to fire off the event, otherwise null</returns>
        private static Tuple<IMultiFileDropTarget, IDroppedFile[]> GetMultiDropTarget(object sender, DragEventArgs e)
        {
            if (e.Handled) return null;

            var s = sender as DependencyObject;
            if (s is null) return null;

            var h = GetMultiFileDropHandler(sender as DependencyObject);
            if (h is null) return null;

            var o = IDroppedFileFactory.GetDroppedFiles(e);
            if (o is null) return null;

            return new(h, o);
        }

        /// <summary> Determine if the event should be passed onto the <see cref="IDroppedFile"/> recipient </summary>
        private static void OnDropPreview(object sender, DragEventArgs e)
        {
            var multiDrop = GetMultiDropTarget(sender, e);
            if (multiDrop.IsNotNull())
            {
                multiDrop.Item1.OnPreviewFileDrop(sender, multiDrop.Item2);
                //return;
            }

            var drop = GetDropTarget(sender, e);
            if (drop.IsNotNull())
            {
                drop.Item1.OnPreviewFileDrop(sender, drop.Item2);
                //return;
            }
        }

        /// <summary> Determine if the event should be passed onto the <see cref="IDroppedFile"/> recipient </summary>
        private static void OnDrop(object sender, DragEventArgs e)
        {
            var multiDrop = GetMultiDropTarget(sender, e);
            if (multiDrop.IsNotNull())
            {
                UIServices.SetBusyState();
                multiDrop.Item1.OnFileDrop(sender, multiDrop.Item2);
                //return;
            }

            var drop = GetDropTarget(sender, e);
            if (drop.IsNotNull())
            {
                UIServices.SetBusyState();
                drop.Item1.OnFileDrop(sender, drop.Item2);
                //return;
            }
        }

        /// <summary> Determine if the event should be passed onto the <see cref="IDroppedFile"/> recipient </summary>
        private static void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            var handler = GetDragHandler(sender as DependencyObject);
            handler?.OnPreviewDragEnter(sender, e);
        }

        /// <summary> Determine if the event should be passed onto the <see cref="IDroppedFile"/> recipient </summary>
        private static void OnDragEnter(object sender, DragEventArgs e)
        {
            var handler = GetDragHandler(sender as DependencyObject);
            handler?.OnDragEnter(sender, e);
        }

        /// <summary> Determine if the event should be passed onto the <see cref="IDroppedFile"/> recipient </summary>
        private static void OnPreviewDragOver(object sender, DragEventArgs e)
        {
            //var handler = GetDragHandler(sender as DependencyObject);
            //handler?(sender, e);
        }

        /// <summary> Determine if the event should be passed onto the <see cref="IDroppedFile"/> recipient </summary>
        private static void OnDragOver(object sender, DragEventArgs e)
        {
            var handler = GetDragHandler(sender as DependencyObject);
            handler?.OnDragOver(sender, e);
        }

        /// <summary> Determine if the event should be passed onto the <see cref="IDroppedFile"/> recipient </summary>
        private static void OnPreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            //var handler = GetDragHandler(sender as DependencyObject);
            //handler?.OnDragEnter(sender, e);
        }

        /// <summary> Determine if the event should be passed onto the <see cref="IDroppedFile"/> recipient </summary>
        private static void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            var handler = GetDragHandler(sender as DependencyObject);
            handler?.OnGiveFeedback(sender, e);
        }

        /// <summary> Determine if the event should be passed onto the <see cref="IDroppedFile"/> recipient </summary>
        private static void OnPreviewDragLeave(object sender, DragEventArgs e)
        {
            var handler = GetDragHandler(sender as DependencyObject);
            handler?.OnPreviewDragLeave(sender, e);
        }

        /// <summary> Determine if the event should be passed onto the <see cref="IDroppedFile"/> recipient </summary>
        private static void OnDragLeave(object sender, DragEventArgs e)
        {
            var handler = GetDragHandler(sender as DependencyObject);
            handler?.OnDragLeave(sender, e);
        }







        #endregion

    }
}
