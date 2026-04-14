namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class SelectorRefreshOnInitializeFalse
    {
        
        [ListBox(RefreshOnInitialize = false)]
        [ComboBox(RefreshOnInitialize = false)]
        [Selector(RefreshOnInitialize = false)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0300:Simplify collection initialization")]
        private int[] GetItems() => new int[] { 1, 2, 3, 4, 5 };

    }
}
