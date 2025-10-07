using CCEnvs.Diagnostics;
using System.Collections.Generic;

#nullable enable

namespace CCEnvs.Collections
{
    public static class IEnumeratorExtensions
    {
        public static EnumeratorEnumerable<T> AsEnumerable<T>(this IEnumerator<T> value)
        {
            if (value.IsNull())
                throw new System.ArgumentNullException(nameof(value));

            return new EnumeratorEnumerable<T>(value);
        }
        public static EnumeratorEnumerable<T> AsEnumerable<TEnumerator, T>(this TEnumerator value)
            where TEnumerator : IEnumerator<T>
        {
            if (value.IsNull())
                throw new System.ArgumentNullException(nameof(value));

            return new EnumeratorEnumerable<T>(value);
        }
    }
}