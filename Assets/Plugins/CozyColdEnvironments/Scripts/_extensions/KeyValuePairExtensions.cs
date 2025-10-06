#nullable enable
using System.Collections.Generic;

namespace CCEnvs
{
    public static class KeyValuePairExtensions
    {
        public static (TKey, TValue) ToTuple<TKey, TValue>(
            this KeyValuePair<TKey, TValue> source)
        {
            return (source.Key, source.Value);
        }
    }
}
