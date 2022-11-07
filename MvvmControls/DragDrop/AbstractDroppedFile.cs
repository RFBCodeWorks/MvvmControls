using System.IO;
using System.Windows;

namespace RFBCodeWorks.MvvmControls.DragDrop
{
    /// <summary>
    /// Base class all <see cref="IDroppedFile"/> objects should derive from
    /// </summary>
    public abstract class AbstractDroppedFile : IDroppedFile
    {
        /// <summary>
        /// 
        /// </summary>
        public AbstractDroppedFile(string source, DragEventArgs args)
        {
            DragEventArgs = args;
            Source = source;
            Name = Path.GetFileName(source);
        }

        /// <inheritdoc />
        public DragEventArgs DragEventArgs { get; }
        
        /// <inheritdoc />
        public virtual string Source { get; }

        /// <inheritdoc />
        public virtual string Name { get; }

        /// <inheritdoc cref="DragEventArgs.AllowedEffects" path="*" />
        public DragDropEffects AllowedEffects => DragEventArgs.AllowedEffects;

        /// <inheritdoc cref="RoutedEventArgs.Handled" path="*" />
        public bool Handled { get => DragEventArgs.Handled; set => DragEventArgs.Handled = value; }

        /// <inheritdoc />
        public abstract string FileExtension { get; }

        /// <inheritdoc />
        public void CopyTo(string destination) => CopyTo(destination, false);

        /// <inheritdoc />
        public abstract void CopyTo(string destination, bool overWrite);
    }
}
