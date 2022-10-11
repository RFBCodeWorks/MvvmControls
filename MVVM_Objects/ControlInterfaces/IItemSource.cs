using RFBCodeWorks.MVVMObjects.BaseControlDefinitions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects.ControlInterfaces
{
    
    /// <summary>
    /// Represents an ItemSource collection
    /// </summary>
    public interface IItemSource<T,E> : IItemSource
        where E : IList<T>
    {
        /// <inheritdoc cref="BaseControlDefinitions.ItemSourceDefinition{T, E}.ItemSource"/>
        new E ItemSource { get; set; }
    }

    /// <summary>
    /// Represents an ItemSource collection
    /// </summary>
    public interface IItemSource : IControlDefinition
    {
        /// <inheritdoc cref="BaseControlDefinitions.ItemSourceDefinition{T, E}.ItemSourceChanged"/>
        event EventHandler ItemSourceChanged;

        /// <inheritdoc cref="BaseControlDefinitions.ItemSourceDefinition{T, E}.ItemSource"/>
        IList ItemSource { get; }

        /// <inheritdoc cref="BaseControlDefinitions.ItemSourceDefinition{T, E}.DisplayMemberPath"/>
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
    }

    /// <summary>
    /// Interface for Selectors of a specific type
    /// </summary>
    public interface ISelector<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when the SelectedItem changes
        /// </summary>
        event PropertyOfTypeChangedEventHandler<T> SelectedItemChanged;
        
        /// <summary>
        /// Gets the SelectedItem from the Selector
        /// </summary>
        T SelectedItem { get; }
    }

    /// <summary>
    /// Interface for Selectors and collection of a specific type
    /// </summary>
    public interface ISelector<T, E> : ISelector<T>, IItemSource<T,E> where E: IList<T>
    {

    }
}
