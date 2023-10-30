using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RFBCodeWorks.Mvvm.XmlLinq.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class XDocumentTreeView : TreeViewBase<XElementTreeViewItem, XNodeTreeViewItem>
    {
        /// <inheritdoc/>
        public XDocumentTreeView() : base()  { }

        private XDocument Document;
        private RelayCommand overwriteCmd;
        private RelayCommand<string> loadDocCmd;
        private RelayCommand<string> SaveDocCmd;
        private RelayCommand<XDocument> LoadXDocCmd;

        /// <summary>
        /// Command to load a specified XDocument
        /// </summary>
        public RelayCommand<string> LoadPathCommand => loadDocCmd ??= new RelayCommand<string>(LoadXDocument, CanLoadDocument);

        /// <summary>
        /// Command to load a specified XDocument
        /// </summary>
        public RelayCommand<XDocument> LoadDocumentCommand => LoadXDocCmd ??= new RelayCommand<XDocument>(LoadXDocument, CanLoadDocument);

        /// <summary>
        /// Command to save the current XDocument
        /// </summary>
        public RelayCommand<string> SaveToPathCommand => SaveDocCmd ??= new RelayCommand<string>(SaveDocument, CanSaveDocument);

        /// <summary>
        /// Save the document to its current path
        /// </summary>
        public RelayCommand OverWriteCommand => overwriteCmd ??= new RelayCommand(SaveDocument, CanOverWrite);

        /// <summary>
        /// Specify the XDocument to display
        /// </summary>
        public void LoadXDocument(XDocument document)
        {
            Document = document;
            if (document?.Root is XElement ex)
            this.TreeRoot = new XElementTreeViewItem(ex);
        }

        /// <inheritdoc cref="LoadXDocument(XDocument)"/>
        /// <inheritdoc cref="XDocument.Load(string)"/>
        public virtual void LoadXDocument(string uri) => LoadXDocument(XDocument.Load(uri));

        /// <summary>
        /// Determine if the specified XML can be loaded
        /// </summary>
        /// <returns>True if the path exists, otherwise false.</returns>
        protected virtual bool CanLoadDocument(string uri) => File.Exists(uri);

        private bool CanLoadDocument(XDocument document) => document?.Root is XElement;

        /// <summary>
        /// Save the current document to a specified path
        /// </summary>
        /// <param name="path"></param>
        public virtual void SaveDocument(string path) => Document?.Save(path);

        /// <summary>
        /// Determine if the document can be saved
        /// </summary>
        protected virtual bool CanSaveDocument(string path)
        {
            return Document is XDocument && Directory.Exists(Path.GetDirectoryName(path));
        }

        /// <summary>
        /// Overwrite the document
        /// </summary>
        public void SaveDocument()
        {
            if (CanOverWrite()) SaveDocument(Document?.BaseUri);
        }

        private bool CanOverWrite() => Document is XDocument;
    }
}
