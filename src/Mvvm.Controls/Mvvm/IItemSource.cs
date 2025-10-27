using System;
using System.Collections;
using System.Collections.Generic;

namespace RFBCodeWorks.Mvvm
{

    /// <summary>
    /// Represents an ItemSource collection
    /// </summary>
    public interface IItemSource : IControlDefinition
    {
        /// <inheritdoc cref="Primitives.ItemSource{T, E}.ItemSourceChanged"/>
        event EventHandler ItemSourceChanged;

        /// <inheritdoc cref="Primitives.ItemSource{T, E}.Items"/>
        IList Items { get; }

        /// <inheritdoc cref="Primitives.ItemSource{T, E}.DisplayMemberPath"/>
        string DisplayMemberPath { get; }
    }

    /// <summary>
    /// Represents an ItemSource collection
    /// </summary>
    public interface IItemSource<T> : IItemSource
    {
        /// <inheritdoc cref="Primitives.ItemSource{T, E}.Items"/>
        new IList<T> Items { get; }
    }    
}
