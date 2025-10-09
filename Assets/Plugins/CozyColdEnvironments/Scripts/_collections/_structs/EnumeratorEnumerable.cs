using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace CCEnvs.Collections
{
    public readonly struct EnumeratorEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> enumerator;

        public EnumeratorEnumerable(IEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public readonly struct EnumeratorEnumerable<TEnumerator, T> : IEnumerable<T>
        where TEnumerator : struct, IEnumerator<T>
    {
        private readonly TEnumerator enumerator;

        public EnumeratorEnumerable(TEnumerator enumerator)
        {
            this.enumerator = enumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}