#if WINDOWS || NETFRAMEWORK

using RFBCodeWorks.Mvvm.Specialized;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RFBCodeWorks.Mvvm.Helpers
{


    /// <summary>
    /// Helper class to Convert Icons to ImageSources
    /// </summary>
    public static class IconUtilities
    {
        /// <summary>
        /// The default IIconLoader object - Retrieves frozen ImageSource icons using <see cref="IconUtilities"/>
        /// </summary>
        public static IIconProvider IconLoader { get => IconLoaderField ??= new IconLoaderImplementation(); }
        private static IIconProvider IconLoaderField;
        private class IconLoaderImplementation : IIconProvider
        {
            public ImageSource GetDirectoryIcon() => LargeFolderIcon;
            public ImageSource GetFileIcon(string filePath) => IconUtilities.GetFileIcon(filePath).ToImageSource();
            public ImageSource GetDirectoryIcon(string directoryPath) => IconUtilities.GetDirectoryIcon(directoryPath).ToImageSource();
        }

        private static Icon largeFolderIcon;
        private static Icon smallFolderIcon;

        /// <summary>Gets an ImageSource representing the stock Large Folder Icon</summary>
        public static ImageSource LargeFolderIcon => (largeFolderIcon ??= GetStockIcon(StockIcons.Explorer_Closed_Folder, IconSize.LargeIcon)).ToImageSource();

        /// <summary>Gets an ImageSource representing the stock Small Folder Icon</summary>
        public static ImageSource SmallFolderIcon => (smallFolderIcon ??= GetStockIcon(StockIcons.Explorer_Closed_Folder, IconSize.SmallIcon)).ToImageSource();

        /// <param name="icon">The icon to convert</param>
        /// <param name="freezeImage">If set true (default), the ImageSource will be frozen via <see cref="Freezable.Freeze()"/></param>
        /// <param name="disposeIcon">Set to false is using the icon elsewhere, otherwise the icon will be disposed of after conversion.</param>
        /// <inheritdoc cref="Imaging.CreateBitmapSourceFromHIcon(IntPtr, Int32Rect, BitmapSizeOptions)"/>
        public static ImageSource ToImageSource(this Icon icon, bool freezeImage = true, bool disposeIcon = true)
        {
            if (icon is null) throw new ArgumentNullException(nameof(icon));
            BitmapSource image;
            if (disposeIcon)
            {
                using (icon)
                {
                    image = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    icon.Dispose();
                }
            }
            else
            {
                image = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            if (freezeImage) { image.Freeze(); }
            return image;
        }

        
        /// <summary>Retrieve an ImageSource of an Icon from the specified <paramref name="filePath"/></summary>
        /// <param name="filePath">The path to a file that contains an image - Must not be a UNC Path</param>
        /// <inheritdoc cref="System.Drawing.Icon.ExtractAssociatedIcon(string)"/>
        public static Icon GetFileIcon(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) { throw new ArgumentException(nameof(filePath)); }
            if (!File.Exists(filePath)) { throw new FileNotFoundException(nameof(filePath)); }
            try
            {
                return Icon.ExtractAssociatedIcon(filePath);
            }
            catch (Exception e)
            {
                throw new IconRetrievalException("Unable to retrieve icon from file path.", filePath, e);
            }
            
        }

        /// <summary>
        /// Get the stock icon using the specified parameters
        /// </summary>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        /// <seealso href="https://stackoverflow.com/questions/42910628/is-there-a-way-to-get-the-windows-default-folder-icon-using-c"/>
        public static Icon GetStockIcon(StockIcons type, IconSize size)
        {
            try
            {
                var info = new SHSTOCKICONINFO();
                info.cbSize = (uint)Marshal.SizeOf(info);
                _ = SHGetStockIconInfo((uint)type, SHGSI_ICON | (uint)size, ref info);
                try
                {
                    return (Icon)Icon.FromHandle(info.hIcon).Clone(); // Get a copy that doesn't use the original handle
                }
                finally
                {
                    _ = DestroyIcon(info.hIcon); // Clean up native icon to prevent resource leak
                }
            }
            catch(Exception e)
            {
                throw new IconRetrievalException("Unable to get stock icon.", type.ToString(), e);
            }
        }

        /// <summary>Get the Directory Icon from the specified directory path</summary>
        /// <param name="directoryPath">The path to the directory</param>
        /// <returns>The icon associated with the directory</returns>
        public static Icon GetDirectoryIcon(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath)) { throw new ArgumentException(nameof(directoryPath)); }
            if (!Directory.Exists(directoryPath)) { throw new DirectoryNotFoundException(directoryPath); }
            if (Path.GetExtension(directoryPath).ToLowerInvariant() == ".bin") return GetStockIcon(StockIcons.compressed_file_folder_overlay_icon, IconSize.LargeIcon);
            try
            {
                SHFILEINFO shinfo = new SHFILEINFO();
                _ = SHGetFileInfo(directoryPath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), 0x100 | 0x0);
                try
                {
                    return (Icon)Icon.FromHandle(shinfo.hIcon).Clone(); // Get a copy that doesn't use the original handle
                }
                finally
                {
                    _ = DestroyIcon(shinfo.hIcon); // Clean up native icon to prevent resource leak
                }
            }
            catch (Exception e)
            {
                throw new IconRetrievalException("Unable to retrieve icon from directory path.", directoryPath, e);
            }
        }

        private const uint SHSIID_FOLDER = 0x3;
        private const uint SHGSI_ICON = 0x100;


        public enum IconSize : uint
        {
            LargeIcon = 0x0,
            SmallIcon = 0x1,
        }

        public enum StockIcons : uint
        {
            Explorer = 0,
            Explorer_Default_Document = 1,
            Explorer_Default_Application = 2,
            Explorer_Closed_Folder = 3,
            Explorer_Open_Folder = 4,
            Drive_525_inch_floppy = 5,
            Drive_35_inch_floppy = 6,
            Drive_Removable_Drive = 7,
            Drive_HardDrive = 8,
            Drive_NetworkDrive = 9,
            Drive_NetworkDrive_disconnected = 10,
            Drive_CDROM = 11,
            Drive_RAM_Drive = 12,
            Global_Entire_network = 13,
            Explorer_Networked_Computer = 15,
            Explorer_Printers = 16,
            Desktop_Network_Neighborhood = 17,
            Explorer_Workgroup = 18,
            Startmenu_Programs = 19,
            Startmenu_Recent_documents = 20,
            Startmenu_Settings = 21,
            Startmenu_Find = 22,
            Startmenu_Help = 23,
            Startmenu_Run = 24,
            Startmenu_Suspend = 25,
            Startmenu_Docking = 26,
            Startmenu_Shutdown = 27,
            Overlay_Sharing = 28,
            Overlay_Shortcut = 29,
            Desktop_Recycle_bin_empty = 31,
            Desktop_Recycle_bin_full = 32,
            Explorer_DialUp_Networking = 33,
            Explorer_Desktop = 34,
            Startmenu_Settings_Control_Panel = 35,
            Startmenu_Programs_Program_folder = 36,
            Startmenu_Settings_Printers = 37,
            Startmenu_Settings_Taskbar = 39,
            Explorer_Audio_CD = 40,
            Explorer_Saved_search = 42,
            Explorer_und_Startmenu_Favorites = 43,
            Startmenu_Log_Off = 44,
            UAC_administrator_overlay_icon = 77,
            os_drive_folder_icon = 107,
            compressed_file_folder_overlay_icon = 179
        }

        [DllImport("shell32.dll")]
        private static extern int SHGetStockIconInfo(uint siid, uint uFlags, ref SHSTOCKICONINFO psii);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr handle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public IntPtr hIcon;
            public int iSysIconIndex;
            public int iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;
        }

        // https://stackoverflow.com/questions/23666076/how-to-get-the-icon-associated-with-a-specific-folder

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        //Struct used by SHGetFileInfo function
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#endif
