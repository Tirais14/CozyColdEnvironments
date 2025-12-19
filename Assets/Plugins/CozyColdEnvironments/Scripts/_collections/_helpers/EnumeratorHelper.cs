using CCEnvs.Diagnostics;
using System.Collections.Generic;
using ZLinq;

#nullable enable

namespace CCEnvs.Collections
{
    public static class EnumeratorHelper
    {
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> value)
        {
            if (value.IsNull())
                throw new System.ArgumentNullException(nameof(value));

            return new EnumeratorEnumerable<T>(value);
        }

#if Z_LINQ
        public static IEnumerable<T> AsEnumerable<TEnumerator, T>(
            this ValueEnumerable<TEnumerator, T> source)
            where TEnumerator : struct, IValueEnumerator<T>
        {
            var enumerator = source.Enumerator;
            while (enumerator.TryGetNext(out T item))
                yield return item;

            enumerator.Dispose();
        }
#endif
    }
}