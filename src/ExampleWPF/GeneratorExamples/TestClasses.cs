namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen
{
    public partial class Generic<T>
    {
        [Button]
        private static void GenericMethod() { }

        [ComboBox]
        [ListBox]
        [Selector]
        [OnCollectionChanged]
        [OnSelectionChanged]
        private static string[] GetData() => new string[] { "A", "B", "C" };
    }

    public partial class GenericClass<T>
        where T : class
    {
        [Button]
        private static void GenericMethod() { }

        [ComboBox]
        [ListBox]
        [Selector]
        [OnCollectionChanged]
        [OnSelectionChanged]
        private static string[] GetData() => new string[] { "A", "B", "C" };
    }

    public partial class GenericStruct<T>
        where T : struct
    {
        [Button]
        private static void GenericMethod() { }

        [ComboBox]
        [ListBox]
        [Selector]
        [OnCollectionChanged]
        [OnSelectionChanged]
        private static string[] GetData() => new string[] { "A", "B", "C" };
    }

    public partial class GenericMultiple<T, T2>
        where T : class, System.IServiceProvider
        where T2 : class, System.IDisposable
    {
        [Button]
        private static void GenericMethod() { }

        [ComboBox]
        [ListBox]
        [Selector]
        [OnCollectionChanged]
        [OnSelectionChanged]
        private static string[] GetData() => new string[] { "A", "B", "C" };
    }
}
