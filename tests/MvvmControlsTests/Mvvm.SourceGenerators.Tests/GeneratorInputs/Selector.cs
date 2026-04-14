namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.GeneratorInputs
{
    public partial class Selector
    {
        /// <summary>
        /// Get the integers that will be used for this <see cref="RFBCodeWorks.Mvvm.ISelector"/>
        /// </summary>
        /// <returns>A collection of <see cref="int"/> values</returns>
        [ListBox]
        [ComboBox]
        [Selector]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0300:Simplify collection initialization")]
        private static int[] GetItems() => new int[] { 1, 2, 3, 4, 5 };
    }
}
