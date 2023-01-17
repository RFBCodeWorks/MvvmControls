using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <inheritdoc cref="Primitives.SelectorDefinition{T, E, V}.SelectedValue"/>
        object SelectedValue { get; set; }

        /// <inheritdoc cref="Primitives.SelectorDefinition{T, E, V}.SelectedValuePath"/>
        string SelectedValuePath { get; }

        /// <inheritdoc cref="Primitives.SelectorDefinition{T, E, V}.SelectedIndex"/>
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

    /// <summary>
    /// Interface for Selectors and collection of a specific type
    /// </summary>
    public interface ISelector<T, E> : ISelector<T>, IItemSource<T, E> where E : IList<T>
    {

    }
}
