namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class SelectorPropertyName
    {


        [ListBox(PropertyName = "MyListBox")]
        [ComboBox(PropertyName = "MyComboBox")]
        [Selector(PropertyName = "MySelector")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0300:Simplify collection initialization")]
        private int[] GetItems() => new int[] { 1, 2, 3, 4, 5 };

    }
}
