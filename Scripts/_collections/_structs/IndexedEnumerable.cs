#nullable enable
using CCEnvs.Collections;
using System.Collections;
using System.Collections.Generic;

namespace CCEnvs
{
    public readonly struct IndexedEnumerable<T> : IIndexedEnumerable<T>
    {
        private readonly IEnumerable<IndexValuePair<T>> items;

        public IEnumerator<IndexValuePair<T>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
