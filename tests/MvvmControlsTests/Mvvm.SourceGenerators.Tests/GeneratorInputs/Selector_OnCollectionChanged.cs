using CommunityToolkit.Mvvm.Input;
using System;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class Selector_OnCollectionChanged
    {
        public bool ActionPerformed { get; private set; }
        public bool SelectorActionPerformed { get; private set; }
        public bool PartialMethodCalled { get; private set; }
        public bool CommandNotified { get; private set; }

        public Selector_OnCollectionChanged()
        {
            CommandToNotify.CanExecuteChanged += (o, e) => CommandNotified = true;
        }

        /// <summary>
        /// Text to add to the compilation to test if the partial method is called
        /// </summary>
        public static string GetPartialText(string generatorName)
        {
            string methodName = true switch
            {
                true when generatorName.Contains("Selector") => "OnItemsSelectorCollectionChanged",
                true when generatorName.Contains("ComboBox") => "OnItemsComboBoxCollectionChanged",
                true when generatorName.Contains("ListBox") => "OnItemsListBoxCollectionChanged",
                _ => throw new InvalidOperationException()
            };
         return   $@"
namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{{
    partial class Selector_OnCollectionChanged
    {{
        partial void {methodName}() => PartialMethodCalled = true;
    }}
}}";
        }

        public void Reset()
        {
            ActionPerformed = false;
            SelectorActionPerformed = false;
            PartialMethodCalled = false;
        }

        [ListBox]
        [ComboBox]
        [Selector]
        [OnCollectionChanged(nameof(CommandToNotify), Action = nameof(Action), SelectorAction = nameof(SelectorAction))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0300:Simplify collection initialization")]
        private int[] GetItems() => new int[] { 1, 2, 3, 4, 5 };

        private readonly IRelayCommand CommandToNotify = new RelayCommand(() => { });
        private void Action() => ActionPerformed = true;
        private void SelectorAction(ISelector selector) => SelectorActionPerformed = true;
    }
}
