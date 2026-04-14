namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class Selector_OnCollectionChanged_SelectLastItem
    {
        [ListBox]
        [ComboBox]
        [Selector]
        [OnCollectionChanged(SelectorAction = RFBCodeWorks.Mvvm.SelectorUtilities.SelectLastItem)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0300:Simplify collection initialization")]
        private Mvvm.Tests.SelectorTestItem[] GetItems()
        {
            return new Mvvm.Tests.SelectorTestItem[]
            {
                new Mvvm.Tests.SelectorTestItem(0, "0"),
                new Mvvm.Tests.SelectorTestItem(1, "1"),
                new Mvvm.Tests.SelectorTestItem(2, "2"),
                new Mvvm.Tests.SelectorTestItem(3, "3"),
            };
        }
    }
}
