using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using RFBCodeWorks.Mvvm;

namespace ExampleWPF.ViewModels
{
    partial class ComboBoxTester : RFBCodeWorks.Mvvm.ObservableObject
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

        // Task Test(string, token) = invalid -> not generating AsyncRelayCommand<string>
        // Task Test(token)         = invalid -> generating AsyncButtonDefinition<Token>
        // Task test(string)        = Invalid -> Not generating AsyncRelayCommand<string>
        // void Test()              = OK
        // void test(string)        = OK
        // multiple args            = OK (no generation)
        [Button] // invalid -> not generating AsyncRelayCommand<string>
        private void Test(string token) => Task.CompletedTask;
        
        // Not OK - > compiler thinks this needs to be generic?
        [ComboBox]
        private IList<string> GetStrings()
        {
            return new string[]{ "One", "Two", "Three", "Four", "Five" };
        }

        [TriggersRefresh(nameof(EnableComboBox))]
        [ObservableProperty]
        private string data;

    }
}
