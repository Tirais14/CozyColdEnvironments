#nullable enable
using System.Collections.Generic;

namespace CCEnvs.Collections.Immutable
{
    public static class IEnumerableExtensions
    {
        public static ImmutableArray<T> ToImmutableArray<T>(this IEnumerable<T> source)
        {
            CC.Guard.NullArgument(source, nameof(source));

            return new ImmutableArray<T>(source);
        }

        public static ImmutableDictionary<TKey, TValue> ToImmutableDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return new ImmutableDictionary<TKey, TValue>(source);
        }
    }
}
