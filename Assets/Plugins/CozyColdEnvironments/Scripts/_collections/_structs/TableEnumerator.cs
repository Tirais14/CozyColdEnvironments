#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;

namespace CCEnvs.Collections
{
    public struct TableEnumerator<T> : IEnumerator<T>
    {
        private readonly TablePointer pointer;
        private readonly ImmutableTable<T> table;

        public T Current { get; private set; }

        readonly object IEnumerator.Current => Current!;

        public TableEnumerator(TablePointer pointer, ImmutableTable<T> table)
        {
            this.pointer = pointer;
            this.table = table;

            Current = default!;
        }

        public bool MoveNext()
        {
            if (pointer.MoveNext())
            {
                Current = table[pointer.Current];
                return true;
            }

            return false;
        }

        public readonly void Reset() => pointer.Reset();

        readonly void IDisposable.Dispose()
        {
        }
    }
}
