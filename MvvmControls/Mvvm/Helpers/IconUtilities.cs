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
        public static IIconLoader IconLoader { get => IconLoaderField ??= new IconLoaderImplementation(); }
        private static IIconLoader IconLoaderField;
        private class IconLoaderImplementation : IIconLoader
        {
            public ImageSource GetDirectoryIcon() => LargeFolderIcon;
            public ImageSource GetIcon(FileInfo file) => ExtractAssociatedIcon(file?.FullName ?? throw new ArgumentNullException(nameof(file)), true);
            public ImageSource GetIcon(string filePath) => ExtractAssociatedIcon(filePath, true);
        }

        private static Icon largeFolderIcon;
        private static Icon smallFolderIcon;

        /// <summary>Gets an ImageSource representing the stock Large Folder Icon</summary>
        public static ImageSource LargeFolderIcon => (largeFolderIcon ??= GetStockIcon(StockIcons.Explorer_Closed_Folder, IconSize.LargeIcon)).ToImageSource(true);

        /// <summary>Gets an ImageSource representing the stock Small Folder Icon</summary>
        public static ImageSource SmallFolderIcon => (smallFolderIcon ??= GetStockIcon(StockIcons.Explorer_Closed_Folder, IconSize.SmallIcon)).ToImageSource(true);

        /// <summary>Retrieve an ImageSource of an Icon from the specified <paramref name="filePath"/></summary>
        /// <param name="filePath">The path to a file that contains an image - Must not be a UNC Path</param>
        /// <inheritdoc cref="ToImageSource(System.Drawing.Icon, bool)"/>
        /// <inheritdoc cref="System.Drawing.Icon.ExtractAssociatedIcon(string)"/>
        /// <param name="freezeImage"/>
        public static ImageSource ExtractAssociatedIcon(string filePath, bool freezeImage = true)
        {
            if (string.IsNullOrWhiteSpace(filePath)) { throw new ArgumentException(nameof(filePath)); }
            if (!File.Exists(filePath)) { throw new FileNotFoundException(nameof(filePath)); }
            ImageSource imageSource = null;
            using (System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath))
            {
                imageSource = icon?.ToImageSource(freezeImage);
            }
            return imageSource;
        }

        /// <param name="icon">The icon to convert</param>
        /// <param name="freezeImage">If set true (default), the ImageSource will be frozen via <see cref="Freezable.Freeze()"/></param>
        /// <inheritdoc cref="Imaging.CreateBitmapSourceFromHIcon(IntPtr, Int32Rect, BitmapSizeOptions)"/>
        public static ImageSource ToImageSource(this System.Drawing.Icon icon, bool freezeImage = true)
        {
            var image = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            if (freezeImage) { image.Freeze(); }
            return image;
        }

        // https://stackoverflow.com/questions/42910628/is-there-a-way-to-get-the-windows-default-folder-icon-using-c

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public static Icon GetStockIcon(StockIcons type, IconSize size)
        {
            var info = new SHSTOCKICONINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);

            SHGetStockIconInfo((uint)type, SHGSI_ICON | (uint)size, ref info);

            var icon = (Icon)Icon.FromHandle(info.hIcon).Clone(); // Get a copy that doesn't use the original handle
            DestroyIcon(info.hIcon); // Clean up native icon to prevent resource leak

            return icon;
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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
    }
}

#endif
