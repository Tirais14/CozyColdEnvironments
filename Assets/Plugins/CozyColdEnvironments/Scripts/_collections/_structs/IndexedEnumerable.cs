#nullable enable
using CCEnvs.Collections;
using System.Collections;
using System.Collections.Generic;

namespace CCEnvs
{
    public readonly struct IndexedEnumerable<T> : IIndexedEnumerable<T>
    {
        public readonly IEnumerable<IndexValuePair<T>> items;

        public IndexedEnumerable(IEnumerable<IndexValuePair<T>> items)
        {
            this.items = items;
        }

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
