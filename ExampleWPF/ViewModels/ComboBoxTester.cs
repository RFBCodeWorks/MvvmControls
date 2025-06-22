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
    partial class ComboBoxTester : RFBCodeWorks.Mvvm.ObservableObject
    {
        public RadioButtonDefinition EnableComboBox { get; } = new RadioButtonDefinition() { DisplayText = "Enable ComboBox", GroupName = "EC" };
        public RadioButtonDefinition DisableComboBox { get; } = new RadioButtonDefinition() { DisplayText = "Disable ComboBox", GroupName = "EC" };


        private readonly Random random = new Random();

        [ObservableProperty]
        private string _concatenatedText;

        [OnSelectionChanged(MethodName = nameof(Run))]
        [OnCollectionChanged(MethodName = nameof(SelectFirstItem_1))]
        [TriggersRefresh(nameof(SecondDigitComboBox))]
        [ComboBox]
        private string[] FirstDigit()
        {
            return new string[]
            {
                GenerateRandomString(), GenerateRandomString(), GenerateRandomString(), GenerateRandomString(),
            };
        }

        [OnSelectionChanged(MethodName = nameof(Run))]
        [OnCollectionChanged(MethodName = nameof(SelectFirstItem_2))]
        [TriggersRefresh(nameof(ThirdDigitListBox))]
        [ComboBox(RefreshOnInitialize = false, SelectedValueType = typeof(int))]
        private async Task<string[]> SecondDigit()
        {
            await Task.Delay(1250);
            return new string[]
            {
                GenerateRandomString(), GenerateRandomString(), GenerateRandomString(), GenerateRandomString(),
            };
        }

        [OnSelectionChanged(MethodName = nameof(Run))]
        [OnCollectionChanged(MethodName = nameof(SelectFirstItem_3))]
        [TriggersRefresh(nameof(FourthDigitListBox))]
        [ListBox(RefreshOnInitialize =false)]
        private string[] ThirdDigit()
        {
            return new string[]
            {
                GenerateRandomString(), GenerateRandomString(), GenerateRandomString(), GenerateRandomString(),
            };
        }

        [OnSelectionChanged(MethodName = nameof(Run))]
        [OnCollectionChanged(MethodName = nameof(SelectFirstItem_4))]
        [ListBox(RefreshOnInitialize =false, SelectedValueType = typeof(int))]
        private async Task<string[]> FourthDigit()
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
        private void SelectFirstItem_3() => ThirdDigitListBox.SelectedIndex = 0;
        private void SelectFirstItem_4() => FourthDigitListBox.SelectedIndex = 0;

        private void Run() 
        {
            ConcatenatedText = string.Format("{0}.{1}.{2}.{3}", FirstDigitComboBox.SelectedValue, SecondDigitComboBox.SelectedItem, ThirdDigitListBox.SelectedItem, FourthDigitListBox.SelectedItem);
        }

    }
}
