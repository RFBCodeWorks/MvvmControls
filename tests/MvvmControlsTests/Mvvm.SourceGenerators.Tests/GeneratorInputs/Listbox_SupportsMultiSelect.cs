using CommunityToolkit.Mvvm.Input;
using System;
using System.Security;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class Listbox_SupportsMultiSelect
    {
        public bool PartialMethodCalled { get; private set; }
        

        /// <summary>
        /// Text to add to the compilation to test if the partial method is called
        /// </summary>
        public static string GetPartialText(string generatorName)
        {
            string methodName = true switch
            {
                //true when generatorName.Contains("Selector") => "OnItemsSelectorSelectionChanged",
                //true when generatorName.Contains("ComboBox") => "OnItemsComboBoxSelectionChanged",
                true when generatorName.Contains("ListBox") => "OnItemsListBoxSelectedItemsChanged",
                _ => throw new InvalidOperationException()
            };

            return $@"
namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{{
    partial class {nameof(Listbox_SupportsMultiSelect)}
    {{
        partial void {methodName}() => {nameof(PartialMethodCalled)} = true;
    }}
}}";
        }

        public void Reset()
        {
            PartialMethodCalled = false;
        }

        [ListBox(SupportsMultiSelect = true)]
        [OnCollectionChanged]
        [OnSelectionChanged]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0300:Simplify collection initialization")]
        private int[] GetItems() => new int[] { 1, 2, 3, 4, 5 };
    }
}
