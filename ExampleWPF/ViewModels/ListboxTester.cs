using RFBCodeWorks.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace ExampleWPF.ViewModels
{
    class ListboxTester
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
    }
}
