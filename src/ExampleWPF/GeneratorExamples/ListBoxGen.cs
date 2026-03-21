using RFBCodeWorks.Mvvm;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

#nullable enable

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen
{
    /// <summary>
    /// Class to run the Source Generator, which will then be unit tested for integration test.
    /// </summary>
    public partial class ListBoxGen : ISelectorGen
    {

        public ListBoxGen()
        {
            _collectionCommand = new ButtonDefinition(static () => { });
            _selectionCommand = new ButtonDefinition(static () => { });

            _collectionCommand.CanExecuteChanged += this.CollectionNotifiedCommand;
            _selectionCommand.CanExecuteChanged += this.SelectionNotifiedCommand;

            _refreshableItemSource = new Mvvm.RefreshableSelector<object, object[], object>(
                refresh: () =>
            {
                WasSecondaryCollectionRefreshedOnSelectionChange = true;
                return Array.Empty<object>();
            });
        }

        private readonly IRelayCommand _collectionCommand;
        private readonly IRelayCommand _selectionCommand;
        private readonly IRefreshableItemSource _refreshableItemSource;

        IRefreshableItemSource<int> ISelectorGen.Selector_Asynchronous => this.AsyncCollectionListBox;
        IRefreshableItemSource<int> ISelectorGen.Selector_AsynchronousCancellable => this.CancellableAsyncCollectionListBox;
        IRefreshableItemSource<int> ISelectorGen.Selector_Synchronous => this.SyncCollectionListBox;

        public bool IsRefreshable { get; set; }
        public bool WasRefreshed { get; private set; }
        public bool WasSecondaryCollectionRefreshedOnSelectionChange { get; private set; }
        public bool WasCommandNotifiedOnSelectionChange { get; private set; }
        public bool WasCommandNotifiedOnCollectionChange { get; private set; }
        public bool WasSelectionChangeMethodRun {  get; private set; }

        public void Reset()
        {
            WasRefreshed = false;
            WasSecondaryCollectionRefreshedOnSelectionChange = false;
            WasCommandNotifiedOnCollectionChange = false;
            WasCommandNotifiedOnSelectionChange = false;
            WasSelectionChangeMethodRun = false;    
        }

        [TriggersRefresh(nameof(_refreshableItemSource))]
        [OnCollectionChanged(nameof(_collectionCommand))]
        [OnSelectionChanged(nameof(_selectionCommand))]
        [OnSelectionChanged(Action =nameof(SelectionChanged))]
        [ListBox(CanRefresh = nameof(CanRefresh))]
        private int[] SyncCollection()
        {
            WasRefreshed = true;
            return new int[] { 0, 1, 2, 3, 4, 5 };
        }

        [TriggersRefresh(nameof(_refreshableItemSource))]
        [OnCollectionChanged(nameof(_collectionCommand))]
        [OnSelectionChanged(nameof(_selectionCommand))]
        [OnSelectionChanged(Action = nameof(SelectionChanged))]
        [ListBox(CanRefresh = nameof(CanRefresh))]
        public async Task<int[]> AsyncCollection()
        {
            await Task.Delay(125, default);
            return SyncCollection();
        }

        [TriggersRefresh(nameof(_refreshableItemSource))]
        [OnCollectionChanged(nameof(_collectionCommand))]
        [OnSelectionChanged(nameof(_selectionCommand))]
        [OnSelectionChanged(Action = nameof(SelectionChanged))]
        [ListBox(CanRefresh = nameof(CanRefresh))]
        public async Task<int[]> CancellableAsyncCollection(CancellationToken token)
        {
            await Task.Delay(350, token);
            return SyncCollection();
        }

        private bool CanRefresh() => IsRefreshable;
        private void SelectionChanged() => WasSelectionChangeMethodRun = true;
        private void CollectionNotifiedCommand(object? sender, EventArgs e) => WasCommandNotifiedOnCollectionChange = true;
        private void SelectionNotifiedCommand(object? sender, EventArgs e) => WasCommandNotifiedOnSelectionChange = true;
    }
}
