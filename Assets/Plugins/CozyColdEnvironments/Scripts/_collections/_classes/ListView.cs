using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Collections
{
    public class ListView<TOut> : IReadOnlyList<TOut>, IList<TOut>
    {
        private readonly IList list;

        public TOut this[int index] => list[index].CastTo<TOut>();

        public int Count => list.Count;

        bool ICollection<TOut>.IsReadOnly => true;

        TOut IList<TOut>.this[int index] { get => this[index]; set => throw new System.NotSupportedException(); }

        public ListView(IList list)
        {
            CC.Guard.IsNotNull(list, nameof(list));
            this.list = list;
        }

        public bool Contains(TOut item) => list.Contains(item);

        public void CopyTo(TOut[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<TOut> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public int IndexOf(TOut item)
        {
            throw new System.NotImplementedException();
        }

        void ICollection<TOut>.Add(TOut item)
        {
            throw new System.NotSupportedException();
        }

        void ICollection<TOut>.Clear()
        {
            throw new System.NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void IList<TOut>.Insert(int index, TOut item)
        {
            throw new System.NotSupportedException();
        }

        bool ICollection<TOut>.Remove(TOut item)
        {
            throw new System.NotSupportedException();
        }

        void IList<TOut>.RemoveAt(int index)
        {
            throw new System.NotSupportedException();
        }
    }
}
