using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects
{
    
    /// <summary>
    /// Represents an ItemSource collection
    /// </summary>
    public interface IItemSource<T,E> : INotifyPropertyChanged
        where E : IEnumerable<T>
    {
        /// <inheritdoc cref="BaseControlDefinitions.ItemSourceDefinition{T, E}.ItemSourceChanged"/>
        event EventHandler ItemSourceChanged;

        /// <inheritdoc cref="BaseControlDefinitions.ItemSourceDefinition{T, E}.ItemSource"/>
        E ItemSource { get; set; }

    }
}
