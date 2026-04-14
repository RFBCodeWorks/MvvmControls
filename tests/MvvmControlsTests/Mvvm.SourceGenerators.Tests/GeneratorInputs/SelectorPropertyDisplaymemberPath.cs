namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class SelectorPropertyDisplaymemberPath
    {
        public const string ExpectedDisplayMemberPath = "Value";

        [ListBox(DisplayMemberPath = ExpectedDisplayMemberPath)]
        [ComboBox(DisplayMemberPath = ExpectedDisplayMemberPath)]
        [Selector(DisplayMemberPath = ExpectedDisplayMemberPath)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0300:Simplify collection initialization")]
        private int[] GetItems() => new int[] { 1, 2, 3, 4, 5 };

    }
}
