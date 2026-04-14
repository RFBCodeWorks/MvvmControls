using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests
{
    internal static class SelectorGeneratorTestExtensions
    {
        public static ISelector TestItemSourceChangedEvent(this ISelector selector)
        {
            bool raised = false;
            void CheckRaised(object? sender, EventArgs e) => raised = true;
            selector.ItemSourceChanged += CheckRaised;
            _ = selector.Items;
            selector.ItemSourceChanged -= CheckRaised;
            Assert.IsTrue(raised);
            return selector;
        }

        public static ISelector TestIndexChanged(this ISelector selector)
        {
            bool raised = false;
            void CheckRaised(object? sender, EventArgs e) => raised = true;
            selector.SelectedItemChanged += CheckRaised;
            selector.SelectedIndex = 0;
            selector.SelectedIndex = 1;
            selector.ItemSourceChanged -= CheckRaised;
            Assert.IsTrue(raised);
            return selector;
        }

        public static ISelector TestSelectedItemChangedEvent(this ISelector selector)
        {
            bool raised = false;
            void CheckRaised(object? sender, EventArgs e) => raised = true;
            selector.SelectedItemChanged += CheckRaised;
            selector.SelectedItem = selector.Items[0];
            selector.SelectedItem = selector.Items[1];
            selector.ItemSourceChanged -= CheckRaised;
            Assert.IsTrue(raised);
            return selector;
        }
    }
}
