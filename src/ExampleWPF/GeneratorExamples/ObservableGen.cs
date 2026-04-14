using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests.Gen
{
    /// <summary>
    /// Class to test fields that also get tagged with ObservableProperty
    /// </summary>
    /// <remarks>Expected : Generate a partial method that updates the source. Sets WasRefreshed = true;</remarks>
    public partial class ObservableGen : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        [TriggersRefresh(nameof(RefreshReciever))]
        [ObservableProperty]
        private int value;

        public bool WasRefreshed { get; private set; }

        private readonly Mvvm.IRefreshableItemSource RefreshReciever; 

        public ObservableGen()
        {
            RefreshReciever = new Mvvm.RefreshableComboBoxDefinition<int, int[], object>(
            refresh: () => { this.WasRefreshed = true; return Array.Empty<int>(); }
            );
        }

        public void Reset() => WasRefreshed = false;

        [ComboBox(ToolTip = "Test Tooltip")]
        [Selector(ToolTip = "Selector Tooltip")]
        [ListBox(ToolTip = "Listbox Tooltip")]
        private int[] Test() => Array.Empty<int>();
    }
}
