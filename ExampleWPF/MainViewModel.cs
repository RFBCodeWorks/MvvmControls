using RFBCodeWorks.MvvmControls;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleWPF
{
    class MainViewModel : ViewModelBase, IWindowClosing
    {
        public MainViewModel()
        {

        }

        public ListBoxDefinition<string> ListBoxDefinition { get; } = new ListBoxDefinition<string>()
        {
            ItemSource = new string[] { "Index0", "Index1", "Index2", "Index3" }
        };

        public ComboBoxDefinition<string> ComboBoxDefinition { get; } = new ComboBoxDefinition<string>()
        {
            ItemSource = new string[] { "Index0", "Index1", "Index2", "Index3" }
        };

        public CheckBoxDefinition EnableDisableListBox { get; } = new CheckBoxDefinition()
        {
            IsThreeState = true,
            DisplayText = "Enable/Disable Listbox"
        };

        public RadioButtonDefinition EnableComboBox { get; } = new RadioButtonDefinition() { DisplayText = "Enable ComboBox", GroupName = "EC" };
        public RadioButtonDefinition DisableComboBox { get; } = new RadioButtonDefinition() { DisplayText = "Disable ComboBox", GroupName = "EC" };

        public void OnClosed()
        {
            System.Windows.MessageBox.Show("Window Closed", "Window Closed Event");
        }

        public bool OnClosing()
        {
            return System.Windows.MessageBox.Show("Allow window to close?", "Window Closing Event", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes;
        }
    }
}
