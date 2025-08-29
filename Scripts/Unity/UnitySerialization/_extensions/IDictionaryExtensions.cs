using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CozyColdEnvironments.Unity.Serialization
{
    public static class IDictionaryExtensions
    {
        public static SerializedKeyValuePair<TKey, TValue>[] ToSeralizedPairs<TKey, TValue>(
            this IDictionary<TKey, TValue> collection)
        {
            return collection.Select(x => new SerializedKeyValuePair<TKey, TValue>(x.Key, x.Value))
                             .ToArray();
        }
    }
}
