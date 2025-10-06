#nullable enable
using System.Collections.Generic;

namespace CCEnvs.Collections
{
    public interface IIndexedEnumerable<T> : IEnumerable<IndexValuePair<T>>
    {
    }
}
