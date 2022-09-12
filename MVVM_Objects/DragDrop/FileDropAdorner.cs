//using System;
//using System.Collections.Generic;
//using System.Drawing;
////using System.Drawing.Common;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Media;
//using GongSolutions.Wpf.DragDrop.Utilities;

//namespace MVVMObjects.DragDrop
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <remarks>
//    /// Modified ripped from https://github.com/punker76/gong-wpf-dragdrop - DropTargetAdorner , which allows commercial use and modification    
//    /// </remarks>
//    public abstract class DropTargetAdorner : Adorner
//    {
//        public DropTargetAdorner(UIElement adornedElement, DragEventArgs args)
//            : base(adornedElement)
//        {
//            //VisualTarget = sender;
//            this.IsHitTestVisible = false;
//            this.AllowDrop = false;
//            this.SnapsToDevicePixels = true;
//            this.m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
//            this.m_AdornerLayer.Add(this);
//        }

//        DragEventArgs DragArgs { get; protected set; }

//        UIElement VisualTarget { get; protected set; }

//        /// <summary>
//        /// Gets or Sets the pen which can be used for the render process.
//        /// </summary>
//        public Pen Pen { get; set; } = new Pen(Brushes.Gray, 2);

//        public void Detatch()
//        {
//            this.m_AdornerLayer.Remove(this);
//        }

//        internal static DropTargetAdorner Create(Type type, UIElement adornedElement, IDropInfo dropInfo)
//        {
//            if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
//            {
//                throw new InvalidOperationException("The requested adorner class does not derive from DropTargetAdorner.");
//            }
//            return type.GetConstructor(new[] { typeof(UIElement), typeof(DropInfo) })?.Invoke(new object[] { adornedElement, dropInfo }) as DropTargetAdorner;
//        }

//        private readonly AdornerLayer m_AdornerLayer;
//    }

//    /// <summary>
//    /// Creates a rectangle the displays the files being dragged
//    /// </summary>
//    public class FileDragAdorner : DropTargetAdorner
//    {

//        const int IconSizePx = 15;
//        const int RowSpacing = 3;
//        const int TextWidth = 100;

//        /// <summary></summary>
//        public FileDragAdorner(UIElement adornedElement, DragEventArgs e)
//            : base(adornedElement, e)
//        {
//            Files = e.GetDroppedFiles();
//            FileNames = new string[5];
//            Icons = new System.Drawing.Image[5];
//            for (int i = 0; i <=4; i++)
//            {
//                FileNames[i] = Files[i].Name;
//                try
//                {
//                    System.Windows.
//                    Icons[i] = Icon.ExtractAssociatedIcon(Files[i].Source)?.ToBitmap()?.GetThumbnailImage(IconSizePx, IconSizePx, IconDrawAbortCallback, IntPtr.Zero);
//                }
//                catch
//                {

//                }
//                if (i >= Files.Length) break;
//            }

//        }

//        IDroppedFile[] Files { get; }
//        string[] FileNames { get; }
//        System.Drawing.Image[] Icons { get; }
//        Brush TextBrush { get; } = Brushes.Black;

//        private bool IconDrawAbortCallback() { return false; }

//        /// <summary>
//        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system.
//        /// The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for
//        /// later asynchronous use by layout and drawing.
//        /// </summary>
//        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
//        protected override void OnRender(DrawingContext drawingContext)
//        {
//            var dropInfo = this.DropInfo;
//            var visualTargetItem = dropInfo.VisualTargetItem;
//            if (visualTargetItem != null)
//            {
//                var rect = Rect.Empty;
                
//                var StartCorner = DragArgs.
                
//                    drawingContext.DrawRectangle(null, this.Pen, rect);


//                var tvItem = visualTargetItem as TreeViewItem;
//                if (tvItem != null && VisualTreeHelper.GetChildrenCount(tvItem) > 0)
//                {
//                    var descendant = VisualTreeExtensions.GetVisibleDescendantBounds(tvItem);
//                    var translatePoint = tvItem.TranslatePoint(new Point(), this.AdornedElement);
//                    var itemRect = new Rect(translatePoint, tvItem.RenderSize);
//                    descendant.Union(itemRect);
//                    translatePoint.Offset(1, 0);
//                    rect = new Rect(translatePoint, new Size(descendant.Width - translatePoint.X - 1, tvItem.ActualHeight));
//                }

//                if (rect.IsEmpty)
//                {
//                    rect = new Rect(visualTargetItem.TranslatePoint(new Point(), this.AdornedElement), VisualTreeExtensions.GetVisibleDescendantBounds(visualTargetItem).Size);
//                }

//                //Create the Rectangle Internals
                


//                drawingContext.DrawRoundedRectangle(null, this.Pen, rect, 2, 2);
//            }
//        }

//        private void DrawFileName(DrawingContext context, int index, System.Drawing.Point corner)
//        {
//            var img = Icons[index];
//            var imgRec = new Rect(corner.X, corner.Y, IconSizePx, IconSizePx);

//            var str = FileNames[index];
//            FormattedText txt = new FormattedText(str, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, default, 14, TextBrush);
            
//            System.Windows.Point txtPoint = new (corner.X + IconSizePx + 5, corner.Y);
//            context.DrawImage(img, imgRec);
//            context.DrawText(txt, txtPoint);
//        }
//    }
//}
