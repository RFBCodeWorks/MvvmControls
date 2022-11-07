using RFBCodeWorks.MvvmControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MvvmControls.ControlInterfaces
{
    
    /// <summary>
    /// Represents an ItemSource collection
    /// </summary>
    public interface IItemSource<T,E> : IItemSource<T>
        where E : IList<T>
    {
        /// <inheritdoc cref="ItemSourceDefinition{T, E}.ItemSource"/>
        new E ItemSource { get; set; }
    }

    /// <summary>
    /// Represents an ItemSource collection
    /// </summary>
    public interface IItemSource<T> : IItemSource
    {
        /// <inheritdoc cref="ItemSourceDefinition{T, E}.ItemSource"/>
        new IList<T> ItemSource { get; }
    }

    /// <summary>
    /// Represents an ItemSource collection
    /// </summary>
    public interface IItemSource : IControlDefinition
    {
        /// <inheritdoc cref="ItemSourceDefinition{T, E}.ItemSourceChanged"/>
        event EventHandler ItemSourceChanged;

        /// <inheritdoc cref="ItemSourceDefinition{T, E}.ItemSource"/>
        IList ItemSource { get; }

        /// <inheritdoc cref="ItemSourceDefinition{T, E}.DisplayMemberPath"/>
        string DisplayMemberPath { get; }
    }

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

        /// <inheritdoc cref="SelectorDefinition{T, E, V}.SelectedValue"/>
        object SelectedValue { get; set; }

        /// <inheritdoc cref="SelectorDefinition{T, E, V}.SelectedValuePath"/>
        string SelectedValuePath { get; }

        /// <inheritdoc cref="SelectorDefinition{T, E, V}.SelectedIndex"/>
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
    public interface ISelector<T, E> : ISelector<T>, IItemSource<T,E> where E: IList<T>
    {

    }
}
