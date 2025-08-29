using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace CozyColdEnvironments.Collections
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
}
