using System.Threading.Tasks;
using System.Windows;

namespace RFBCodeWorks.Mvvm.DragAndDrop
{
    /// <summary>
    /// 
    /// </summary>
    public class DroppedZipFile : DroppedFile
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="e"></param>
        public DroppedZipFile(string filepath, DragEventArgs e) : base(filepath, e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public Task Extract(string destination)
        {
            return Task.Run( () => System.IO.Compression.ZipFile.ExtractToDirectory(Source, destination) );
        }
    }
}
