#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace CCEnvs.Collections
{
    public readonly struct CCArray<T>
    {
        private readonly T[] values;

        public CCArray(params T[] values)
        {
            this.values = values;
        }

        public CCArray(IEnumerable<T> values)
            :
            this(values.ToArray())
        {
        }

        public static implicit operator T[](CCArray<T> ccArray)
        {
            return ccArray.values;
        }
    }
}