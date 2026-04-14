namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class SelectorCanRefresh
    {
        public const string ExpectedDisplayMemberPath = "Value";

        public bool CanRefresh { get; set; }

        [ListBox(CanRefresh = nameof(RefreshFunc))]
        [ComboBox(CanRefresh = nameof(RefreshFunc))]
        [Selector(CanRefresh = nameof(RefreshFunc))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0300:Simplify collection initialization")]
        private int[] GetItems() => new int[] { 1, 2, 3, 4, 5 };


        private bool RefreshFunc() => CanRefresh;
    }
}
