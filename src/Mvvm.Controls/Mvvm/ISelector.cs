using System;
using System.Collections.Generic;

namespace RFBCodeWorks.Mvvm
{
    /// <summary>
    /// Interface for Selectors
    /// </summary>
    public interface ISelector : IItemSource
    {
        /// <summary>
        /// Occurs when the SelectedItem changes
        /// </summary>
        event EventHandler SelectedItemChanged;

        /// <summary>
        /// Gets the SelectedItem from the Selector
        /// </summary>
        object SelectedItem { get; set; }

        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}"/>
        object SelectedValue { get; set; }

        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectedValuePath"/>
        string SelectedValuePath { get; }

        /// <inheritdoc cref="Primitives.SelectorDefinition{T, TList, TItemValue}.SelectedIndex"/>
        int SelectedIndex { get; set; }
    }

    /// <summary>
    /// Interface for Selectors of a specific type
    /// </summary>
    public interface ISelector<T> : ISelector, IItemSource<T>
    {
        /// <summary>
        /// Gets the SelectedItem from the Selector
        /// </summary>
        new T SelectedItem { get; set; }
    }
}
