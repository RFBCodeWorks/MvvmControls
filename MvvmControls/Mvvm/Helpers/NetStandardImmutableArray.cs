using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Helpers
{
    internal class NetStandardImmutableArray<T> : IList<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection">The collection to make immutaable</param>
        public NetStandardImmutableArray(IEnumerable<T> collection)
        {
            Collection = collection.ToArray();
        }

        private T[] Collection;
        
        T this[int index] { get => Collection[index]; }

        T IList<T>.this[int index] { get => this[index]; set => throw new NotImplementedException(); }

        public int Length => Collection.Length;
        int ICollection<T>.Count => Collection.Length;
        bool ICollection<T>.IsReadOnly => true;
        void ICollection<T>.Add(T item) => throw new NotImplementedException();
        void ICollection<T>.Clear() => throw new NotImplementedException();
        public bool Contains(T item) => Collection.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => Collection.CopyTo(array, arrayIndex);
        IEnumerator IEnumerable.GetEnumerator() => Collection.GetEnumerator();
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Collection).GetEnumerator();
        public int IndexOf(T item) => ((IList<T>)Collection).IndexOf(item);
        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
        void IList<T>.RemoveAt(int index) => throw new NotImplementedException();
    }
}
