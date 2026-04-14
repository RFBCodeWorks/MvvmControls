#nullable enable

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Utility class for interacting with <see cref="ISelector"/> and <see cref="IItemSource"/>
    /// </summary>
    public static class SelectorUtilities
    {
        /// <summary>
        /// const string to be used with <see cref="OnCollectionChangedAttribute"/> to select the first item when the collection is populated.
        /// </summary>
        public const string SelectFirstItem = "RFBCodeWorks.Mvvm.SelectorUtilities." + nameof(TrySelectFirstItem);

        /// <summary>
        /// const string to be used with <see cref="OnCollectionChangedAttribute"/> to select the last item when the collection is populated.
        /// </summary>
        public const string SelectLastItem = "RFBCodeWorks.Mvvm.SelectorUtilities." + nameof(TrySelectLastItem);

        /// <summary>
        /// Sets the <see cref="ISelector.SelectedIndex"/> to the first item within the collection, or -1 if the collection is empty.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns>True if the index was set to the first item, otherwise false.</returns>
        public static bool TrySelectFirstItem(this ISelector selector)
        {
            selector.ThrowIfNull(nameof(selector));
            if (selector.Items.Count == 0)
            {
                selector.SelectedIndex = -1;
                return false;
            }
            selector.SelectedIndex = 0;
            return true;
        }

        /// <summary>
        /// Sets the <see cref="ISelector.SelectedIndex"/> to the last item within the collection, or -1 if the collection is empty.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns>True if the index was set to the last item, otherwise false.</returns>
        public static bool TrySelectLastItem(this ISelector selector)
        {
            selector.ThrowIfNull(nameof(selector));
            if (selector.Items.Count == 0)
            {
                selector.SelectedIndex = -1;
                return false;
            }
            selector.SelectedIndex = selector.Items.Count - 1;
            return true;
        }
    }
}
