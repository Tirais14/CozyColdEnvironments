using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace UTIRLib.Collections
{
    public class ReadOnlyLinkedList<T> : IReadOnlyList<T>
    {
        public T this[int index] => throw new System.NotImplementedException();

        public int Count => throw new System.NotImplementedException();

        public IEnumerator<T> GetEnumerator() => throw new System.NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}