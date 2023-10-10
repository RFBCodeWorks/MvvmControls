using System;
using System.Collections.Generic;
using System.Text;
using RFBCodeWorks.Mvvm;

namespace ExampleWPF.ViewModels
{
    class ComboBoxTester
    {
        public RadioButtonDefinition EnableComboBox { get; } = new RadioButtonDefinition() { DisplayText = "Enable ComboBox", GroupName = "EC" };
        public RadioButtonDefinition DisableComboBox { get; } = new RadioButtonDefinition() { DisplayText = "Disable ComboBox", GroupName = "EC" };

        public CheckBoxDefinition EnableDisableListBox { get; } = new CheckBoxDefinition()
        {
            IsThreeState = false,
            DisplayText = "Enable/Disable Listbox",
            IsChecked = true
        };

        public ComboBoxDefinition<string> ComboBoxDefinition { get; } = new ComboBoxDefinition<string>()
        {
            Items = new string[] { "Index0", "Index1", "Index2", "Index3" }
        };

        public RefreshableComboBoxDefinition<string> RefreshableCmb { get; } = new()
        {
            RefreshFunc = GetStrings,

        };

        private static string[] GetStrings()
        {
            return new string[]{ "One", "Two", "Three", "Four", "Five" };
        }



    }
}
