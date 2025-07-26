using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace UTIRLib
{
    public readonly struct EmptyEnumerator<T> : IEnumerator<T>
    {
        public T Current => default!;

        object IEnumerator.Current => Current!;

        readonly public bool MoveNext() => false;

        readonly void IEnumerator.Reset()
        {
        }

        readonly void IDisposable.Dispose()
        { 
        }
    }
}
