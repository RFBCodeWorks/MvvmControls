using RFBCodeWorks.Mvvm;
using RFBCodeWorks.Mvvm.DialogServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleWPF.ViewModels
{
    class TreeViewViewModel : ViewModelBase
    {
        public TreeViewViewModel()
        {
            SelectNewRootButton = new RelayCommand(SelectRoot);
        }

        public RFBCodeWorks.Mvvm.Specialized.FileTreeViewModel TreeViewVM { get; } = new RFBCodeWorks.Mvvm.Specialized.FileTreeViewModel();

        public RelayCommand SelectNewRootButton { get; }

        private void SelectRoot()
        {
            var browser = new System.Windows.Forms.FolderBrowserDialog();
            if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TreeViewVM.SetRootDirectory(browser.SelectedPath);
            }
        }
    }
}
