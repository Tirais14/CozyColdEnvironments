using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace UTIRLib
{
    public readonly struct EmptyEnumerator<T> : IEnumerator<T>
    {
        public T Current => default!;
        object IEnumerator.Current => Current!;

        public bool MoveNext() => false;
        public void Reset() { }
        public void Dispose() { }
    }
}
