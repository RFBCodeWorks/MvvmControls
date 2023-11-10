using RFBCodeWorks.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExampleWPF.ViewModels
{
    class XmlLinqViewModel : ViewModelBase
    {
        public XmlLinqViewModel() : base()
        {
            SelectXmlFileCommand = new RelayCommand(LookupXmlFile);
        }

        public Models.XmlLinqModel XmlLinqVM { get; } = new();

        public RFBCodeWorks.Mvvm.Specialized.XDocumentTreeViewModel XmlTreeView { get; } = new RFBCodeWorks.Mvvm.Specialized.XDocumentTreeViewModel();

        public RelayCommand SelectXmlFileCommand { get; }

        private void LookupXmlFile()
        {
            var browser = new System.Windows.Forms.OpenFileDialog();
            browser.Filter = "XML|*.xml";
            if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlTreeView.LoadXDocument(browser.FileName);
            }
        }
    }
}
