#nullable enable
using System.Collections;
using System.Collections.Generic;
using ZLinq;

namespace CCEnvs
{
    public readonly struct ZLinqEnumerable<TEnumerator, T> : IEnumerable<T>
        where TEnumerator : struct, IValueEnumerator<T>
    {
        private readonly TEnumerator enumerator;

        public ZLinqEnumerable(TEnumerator enumerator)
        {
            this.enumerator = enumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(enumerator);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            private readonly TEnumerator enumerator;

            public T Current { get; private set; }

            readonly object IEnumerator.Current => Current!;

            public Enumerator(TEnumerator enumerator)
            {
                this.enumerator = enumerator;
                Current = default!;
            }

            public readonly void Dispose()
            {
                enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (enumerator.TryGetNext(out T current))
                {
                    Current = current;
                    return true;
                }

                return false;
            }

            public readonly void Reset()
            {
            }
        }
    }
}
