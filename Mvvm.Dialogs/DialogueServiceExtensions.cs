using MvvmDialogs.FrameworkDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using MvvmDialogs.FrameworkDialogs.MessageBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.DialogServices
{
    /// <summary></summary>
    public static class IDialogueServiceExtensions
    {

        #region < Message Dialog >

        /// <summary>
        /// Gets a <see cref="MessageBoxSettings" /> object
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static MessageBoxSettings GetMessageBoxSettings(this MvvmDialogs.IDialogService service) => new MessageBoxSettings();

        #endregion

        #region < Folder Browser >

        /// <summary>
        /// Get a new FolderBrowserDialogSettings object
        /// </summary>
        /// <returns>new FolderBrowserDialog settings</returns>
        public static FolderBrowserDialogSettings GetFolderBrowserDialogSettings(this MvvmDialogs.IDialogService service) => new FolderBrowserDialogSettings();

        /// <summary>
        /// Get a new FolderBrowserDialogSettings object
        /// </summary>
        /// <param name="service">Not Required - Only for shortcut purposes</param>
        /// <param name="root"><inheritdoc cref="MvvmDialogs.FrameworkDialogs.FolderBrowser.FolderBrowserDialogSettings.RootFolder"/></param>
        /// <param name="showNewFolderButton"><inheritdoc cref="MvvmDialogs.FrameworkDialogs.FolderBrowser.FolderBrowserDialogSettings.ShowNewFolderButton"/></param>
        /// <param name="descrip"><inheritdoc cref="MvvmDialogs.FrameworkDialogs.FolderBrowser.FolderBrowserDialogSettings.Description"/></param>
        /// <returns>new FolderBrowserDialog settings</returns>
        public static MvvmDialogs.FrameworkDialogs.FolderBrowser.FolderBrowserDialogSettings GetFolderBrowserDialogSettings(this MvvmDialogs.IDialogService service, Environment.SpecialFolder root, bool showNewFolderButton = false, string descrip = "Select a folder")
            => new FolderBrowserDialogSettings()
            {
                RootFolder = root,
                ShowNewFolderButton = showNewFolderButton,
                Description = descrip
            };


        /// <summary>
        /// Get a new FolderBrowserDialogSettings object
        /// </summary>
        /// <param name="service">Not Required - Only for shortcut purposes</param>
        /// <param name="root"><inheritdoc cref="FolderBrowserDialogSettings.SelectedPath" path="*"/></param>
        /// <param name="title"><inheritdoc cref="FolderBrowserDialogSettings.Description" path="*"/></param>
        /// <param name="showNewFolderButton"><inheritdoc cref="FolderBrowserDialogSettings.ShowNewFolderButton" path="*"/></param>
        /// <returns>new FolderBrowserDialog settings</returns>
        public static MvvmDialogs.FrameworkDialogs.FolderBrowser.FolderBrowserDialogSettings GetFolderBrowserDialogSettings(this MvvmDialogs.IDialogService service, string root = "", string title = "Select a folder", bool showNewFolderButton = false)
            => new FolderBrowserDialogSettings()
            {
                SelectedPath = root,
                ShowNewFolderButton = showNewFolderButton,
                Description = title
            };

        /// <summary>Use the <see cref="DialogServiceLocator.DefaultLocator"/> to show a FolderBrowser Dialog</summary>
        /// <inheritdoc cref="ShowDialog(OpenFileDialogSettings, IViewModel)"/>
        public static bool ShowDialog(this FolderBrowserDialogSettings settings, IViewModel owner) => DialogServiceLocator.DefaultLocator.GetDialogService(owner).ShowFolderBrowserDialog(owner, settings) ?? false;

        #endregion

        #region < Open File Browser >

        /// <summary>
        /// Get a new FolderBrowserDialogSettings object
        /// </summary>
        /// <returns>new FolderBrowserDialog settings</returns>
        public static MvvmDialogs.FrameworkDialogs.OpenFile.OpenFileDialogSettings GetOpenFileDialogSettings() => CreateFileDialogSettings<OpenFileDialogSettings>("Open File");

        /// <param name="service">Not Required - Only for shortcut purposes</param>
        /// <param name="allowMultiSelect"> <inheritdoc cref="OpenFileDialogSettings.Multiselect" path="*"/></param>
        /// <param name="filters">File Filter objects to define the desired filters</param>
        /// <returns></returns>
        /// <inheritdoc cref="CreateFileDialogSettings{T}"/>
        /// <param name="fileName"/>
        /// <param name="initialDirectory"/>
        /// <param name="title"/>
        /// <param name="defaultExt"/>
        public static MvvmDialogs.FrameworkDialogs.OpenFile.OpenFileDialogSettings GetOpenFileDialogSettings(this MvvmDialogs.IDialogService service, bool allowMultiSelect, string fileName = "", string initialDirectory = "", string title = "Open File", string defaultExt = "", params FileFilter[] filters)
        {
            var diag = CreateFileDialogSettings<OpenFileDialogSettings>("Open", fileName, filters?.GetMvvmDialogFilter(), defaultExt, initialDirectory, null);
            diag.Multiselect = allowMultiSelect;
            diag.CheckFileExists = true;
            return diag;
        }


        /// <summary>Use the <see cref="DialogServiceLocator.DefaultLocator"/> to show an OpenFile Dialog</summary>
        /// <param name="settings">The Settings Dialog</param>
        /// <param name="owner">The parent window</param>
        /// <returns>TRUE if the user clicked ACCEPT, otherwise false.</returns>
        public static bool ShowDialog(this OpenFileDialogSettings settings, IViewModel owner) => DialogServiceLocator.DefaultLocator.GetDialogService(owner).ShowOpenFileDialog(owner, settings) ?? false;

        #endregion

        const string DefaultExt = "Any File (*.*) | *.*";

        /// <summary>
        /// Create the Dialog Settings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="title"><inheritdoc cref="FileDialogSettings.Title" path="*"/></param>
        /// <param name="fileName"><inheritdoc cref="FileDialogSettings.FileName" path="*"/></param>
        /// <param name="filter"><inheritdoc cref="FileDialogSettings.Filter" path="*"/></param>
        /// <param name="defaultExt"><inheritdoc cref="FileDialogSettings.DefaultExt" path="*"/></param>
        /// <param name="initialDirectory"><inheritdoc cref="FileDialogSettings.InitialDirectory" path="*"/></param>
        /// <param name="customPlaces"><inheritdoc cref="FileDialogSettings.CustomPlaces" path="*"/></param>
        /// <returns>new Dialog Settings of type <typeparamref name="T"/></returns>
        private static T CreateFileDialogSettings<T>(string title, string fileName = "", string filter = DefaultExt, string defaultExt = "", string initialDirectory = null, IList<Microsoft.Win32.FileDialogCustomPlace> customPlaces = null) where T : FileDialogSettings, new()
        {
            var diag = new T()
            {
                AddExtension = false,
                DereferenceLinks = true, //Convert shortcuts to actual file paths
                CheckPathExists = true,
                DefaultExt = defaultExt,
                Filter = filter,
                FileName = fileName,
                InitialDirectory = initialDirectory ?? Path.GetDirectoryName(fileName),
                Title = title,
                ValidateNames = true,
            };
            if (customPlaces != null) diag.CustomPlaces = customPlaces;
            return diag;
        }

        #region < Save File Browser >

        /// <summary>
        /// Get a new FolderBrowserDialogSettings object
        /// </summary>
        /// <returns>new FolderBrowserDialog settings</returns>
        public static SaveFileDialogSettings GetSaveFileDialogSettings(this MvvmDialogs.IDialogService service) { return CreateFileDialogSettings<SaveFileDialogSettings>("Save File"); }

        /// <param name="service">Not Required - Only for shortcut purposes</param>
        /// <param name="overWritePrompt"> <inheritdoc cref="SaveFileDialogSettings.OverwritePrompt" path="*"/></param>
        /// <param name="filters">File Filter objects to define the desired filters</param>
        /// <returns></returns>
        /// <inheritdoc cref="CreateFileDialogSettings{T}"/>
        /// <param name="fileName"/>
        /// <param name="initialDirectory"/>
        /// <param name="title"/>
        /// <param name="defaultExt"/>
        public static SaveFileDialogSettings GetSaveFileDialogSettings(this MvvmDialogs.IDialogService service, string fileName, bool overWritePrompt = true, string initialDirectory = "", string title = "Open File", string defaultExt = "", params FileFilter[] filters)
        {
            var diag = CreateFileDialogSettings<SaveFileDialogSettings>("Open", fileName, filters?.GetMvvmDialogFilter(), defaultExt, initialDirectory, null);
            diag.CreatePrompt = false;
            diag.OverwritePrompt = overWritePrompt;
            diag.CheckFileExists = true;
            return diag;
        }

        /// <summary>Use the <see cref="DialogServiceLocator.DefaultLocator"/> to show a SaveFile Dialog</summary>
        /// <inheritdoc cref="ShowDialog(OpenFileDialogSettings, IViewModel)"/>
        public static bool ShowDialog(this SaveFileDialogSettings settings, IViewModel owner) => DialogServiceLocator.DefaultLocator.GetDialogService(owner).ShowSaveFileDialog(owner, settings) ?? false;

        #endregion

    }
}
