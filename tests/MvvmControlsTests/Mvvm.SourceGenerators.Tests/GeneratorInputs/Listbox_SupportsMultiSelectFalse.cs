using CommunityToolkit.Mvvm.Input;
using System;
using System.Security;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class Listbox_SupportsMultiSelectFalse
    {
        public bool PartialMethodCalled { get; private set; }
        

        /// <summary>
        /// Text to add to the compilation to test if the partial method is called
        /// </summary>
        public static string GetPartialText(string generatorName)
        {
            return "";
        }

        public void Reset()
        {
            PartialMethodCalled = false;
        }

        [ListBox(SupportsMultiSelect = false)]
        [OnCollectionChanged]
        [OnSelectionChanged]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0300:Simplify collection initialization")]
        private int[] GetItems() => new int[] { 1, 2, 3, 4, 5 };
    }
}
