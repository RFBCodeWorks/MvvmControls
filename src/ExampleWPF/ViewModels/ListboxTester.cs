using CommunityToolkit.Mvvm.ComponentModel;
using RFBCodeWorks.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ExampleWPF.ViewModels
{
    partial class ListboxTester : ObservableObject
    {
        public ListBoxDefinition<string> ListBoxDefinition { get; } = new ListBoxDefinition<string>()
        {
            Items = new string[] { "Index0", "Index1", "Index2", "Index3" },
            SelectionMode = SelectionMode.Multiple
        };

        public ListBoxDefinition<string> ListBoxDefinition2 { get; } = new ListBoxDefinition<string>()
        {
            Items = new string[] { "Howdy", "OhNo", "This is a string", "Hello" },
            SelectionMode = SelectionMode.Multiple
        };

        public CheckBoxDefinition ListBoxEnabler { get; } = new CheckBoxDefinition();

        public int SelectedNumber { get; private set; }


        [OnSelectionChanged(Action = nameof(NumberWasSelected))]
        [OnCollectionChanged(SelectorAction = RFBCodeWorks.Mvvm.SelectorUtilities.SelectFirstItem)]
        [ListBox(
            PropertyName = "GeneratedListBox",
            RefreshOnInitialize = true,
            ToolTip = "This listbox was Generated using the SourceGenerator and [ListBox] attribute"
            )]
        private int[] GetRandomNumbers()
        {
            Random x = new Random();
            int[] values = new int[10];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = x.Next(0, 100);
            }
            return values;
        }
        private void NumberWasSelected()
        {
            SelectedNumber = this.GeneratedListBox.SelectedItem;
            OnPropertyChanged(nameof(SelectedNumber));
        }
            

    }
}
