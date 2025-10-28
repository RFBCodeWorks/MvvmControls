using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using RFBCodeWorks.Mvvm;

namespace ExampleWPF.ViewModels
{
    partial class ComboBoxTester : ObservableObject
    {
        public RadioButtonDefinition EnableComboBox { get; } = new RadioButtonDefinition() { DisplayText = "Enable ComboBox", GroupName = "EC" };
        public RadioButtonDefinition DisableComboBox { get; } = new RadioButtonDefinition() { DisplayText = "Disable ComboBox", GroupName = "EC" };


        private readonly Random random = new Random();

        [ObservableProperty]
        private string _concatenatedText;

        [OnSelectionChanged(Action = nameof(Run))]
        [OnCollectionChanged(Action = nameof(SelectFirstItem_1))]
        [TriggersRefresh(nameof(SecondDigitComboBox))]
        [ComboBox]
        private string[] FirstDigit()
        {
            return new string[]
            {
                GenerateRandomString(), GenerateRandomString(), GenerateRandomString(), GenerateRandomString(),
            };
        }

        [OnSelectionChanged(Action = nameof(Run))]
        [OnCollectionChanged(Action = nameof(SelectFirstItem_2))]
        [TriggersRefresh(nameof(ThirdDigitSelector))]
        [ComboBox(RefreshOnInitialize = false, SelectedValueType = typeof(int))]
        private async Task<string[]> SecondDigit()
        {
            await Task.Delay(1250);
            return new string[]
            {
                GenerateRandomString(), GenerateRandomString(), GenerateRandomString(), GenerateRandomString(),
            };
        }

        [OnSelectionChanged(Action = nameof(Run))]
        [OnCollectionChanged(SelectorAction = SelectorUtilities.SelectFirstItem)]
        [TriggersRefresh(nameof(FourthDigitSelector))]
        [ComboBox(RefreshOnInitialize =false, PropertyName ="ThirdDigitSelector")]
        private async Task<string[]> ThirdDigit()
        {
            await Task.Delay(1350);
            return new string[]
            {
                GenerateRandomString(), GenerateRandomString(), GenerateRandomString(), GenerateRandomString(),
            };
        }

        [OnSelectionChanged(Action = nameof(Run))]
        [OnCollectionChanged(SelectorAction = SelectorUtilities.SelectLastItem)]
        [ComboBox(RefreshOnInitialize =false, SelectedValueType = typeof(int), PropertyName = "FourthDigitSelector")]
        private async Task<string[]> GetFourthDigit()
        {
            await Task.Delay(1350);
            return new string[]
            {
                GenerateRandomString(), GenerateRandomString(), GenerateRandomString(), GenerateRandomString(),
            };
        }

        public string GenerateRandomString(int length = 4)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        private void SelectFirstItem_1() => FirstDigitComboBox.SelectedIndex = 0;
        private void SelectFirstItem_2() => SecondDigitComboBox.SelectedIndex = 0;

        private void Run() 
        {
            ConcatenatedText = string.Format("{0}.{1}.{2}.{3}", FirstDigitComboBox.SelectedValue, SecondDigitComboBox.SelectedItem, ThirdDigitSelector.SelectedItem, FourthDigitSelector.SelectedItem);
        }

    }
}
