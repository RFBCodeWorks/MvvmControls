namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen
{
    /// <summary>
    /// Interface to run through the 'Selector' generator tests to ensure all functions operating as expected
    /// </summary>
    public interface ISelectorGen
    {
        bool IsRefreshable { get; set; }
        bool WasRefreshed { get; }
        bool WasSecondaryCollectionRefreshedOnSelectionChange { get; }
        bool WasCommandNotifiedOnSelectionChange { get; }
        bool WasCommandNotifiedOnCollectionChange { get; }
        bool WasSelectionChangeMethodRun { get; }
        void Reset();
        IRefreshableItemSource<int> Selector_Asynchronous { get; }
        IRefreshableItemSource<int> Selector_AsynchronousCancellable { get; }
        IRefreshableItemSource<int> Selector_Synchronous { get; }
    }
}
