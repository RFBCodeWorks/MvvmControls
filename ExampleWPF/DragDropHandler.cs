using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using RFBCodeWorks.Mvvm;
using RFBCodeWorks.Mvvm.DragAndDrop;

namespace ExampleWPF
{
    class DragDropHandler : ObservableObject, IFileDropTarget, IDragHandler
    {

        /// <summary>
        /// 
        /// </summary>
        public string FileName
        {
            get { return FileNameField; }
            set { SetProperty(ref FileNameField, value, nameof(FileName)); }
        }
        private string FileNameField;


        /// <summary>
        /// 
        /// </summary>
        public string DirectoryPath
        {
            get { return DirectoryPathField; }
            set { SetProperty(ref DirectoryPathField, value, nameof(DirectoryPath)); }
        }
        private string DirectoryPathField;


        /// <summary>
        /// 
        /// </summary>
        public long FileSize
        {
            get { return FileSizeField; }
            set { SetProperty(ref FileSizeField, value, nameof(FileSize)); }
        }
        private long FileSizeField;


        /// <summary>
        /// 
        /// </summary>
        public bool IsHitTestVisible
        {
            get { return IsHitTestVisibleField; }
            set { SetProperty(ref IsHitTestVisibleField, value, nameof(IsHitTestVisible)); }
        }

        public bool IsDragging => throw new NotImplementedException();

        private bool IsHitTestVisibleField;


        /// <summary>
        /// 
        /// </summary>
        public string DragRectangleToolTip
        {
            get { return DragRectangleToolTipField; }
            set { SetProperty(ref DragRectangleToolTipField, value, nameof(DragRectangleToolTip)); }
        }
        private string DragRectangleToolTipField;


        public void OnFileDrop(object sender, IDroppedFile e)
        {
            var f = new FileInfo(e.Source);
            FileName = e.Name;
            DirectoryPath = f.DirectoryName;
            FileSize = f.Length;
            IsHitTestVisible = false;
            DragRectangleToolTip = "";
        }

        public void OnPreviewFileDrop(object sender, IDroppedFile e)
        {
            //do nothing
        }

        public void OnDragEnter(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void OnDragLeave(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            IsHitTestVisible = true;
            var ef = RFBCodeWorks.Mvvm.DragAndDrop.IDroppedFileFactory.GetDroppedFile(e);
            if (ef != null)
            {
                DragRectangleToolTip = ef.Source;
            }
        }

        public void OnPreviewDragLeave(object sender, DragEventArgs e)
        {
            DragRectangleToolTip = "";
            IsHitTestVisible = false;
        }

        public void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            //
        }

        public void OnDragOver(object sender, DragEventArgs e)
        {
            //
        }
    }
}
