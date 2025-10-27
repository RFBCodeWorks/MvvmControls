using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

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
        private int[] Test() => [];
    }
}
