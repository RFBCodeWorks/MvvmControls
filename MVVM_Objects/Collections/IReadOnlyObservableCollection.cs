using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace RFBCodeWorks.MVVMObjects
{
    /// <summary>
    /// Interface for a ReadOnly collection that implemented <see cref="INotifyCollectionChanged"/>
    /// </summary>
    /// <remarks>Taken from <see href="https://github.com/meziantou/Meziantou.Framework"/> under MIT license</remarks>
    public interface IReadOnlyObservableCollection<T> : IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }
}