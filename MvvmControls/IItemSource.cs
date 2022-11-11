using System;
using System.Collections;
using System.Collections.Generic;

namespace RFBCodeWorks.MvvmControls
{

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
    public interface IItemSource<T,E> : IItemSource<T>
        where E : IList<T>
    {
        /// <inheritdoc cref="ItemSourceDefinition{T, E}.ItemSource"/>
        new E ItemSource { get; set; }
    }

    
}
