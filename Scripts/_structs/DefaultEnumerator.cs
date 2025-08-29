#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;

namespace CozyColdEnvironments.Collections
{
    [Obsolete("Use Cast<T> on empty array instead")]
    public readonly struct DefaultEnumerator<T> : IEnumerator<T?>
    {
        private readonly IEnumerator defaultEnumerator;

        public readonly T? Current => (T)defaultEnumerator.Current;
        readonly object? IEnumerator.Current => Current;

        public DefaultEnumerator(IEnumerable enumerable) => defaultEnumerator = enumerable.GetEnumerator();

        public readonly bool MoveNext() => defaultEnumerator.MoveNext();

        public readonly void Reset() => defaultEnumerator.Reset();

        public readonly void Dispose()
        {
        }
    }
}