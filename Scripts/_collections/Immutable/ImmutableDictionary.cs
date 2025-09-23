using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable enable
namespace CCEnvs
{
    public class ImmutableDictionary<TKey, TValue> : ReadOnlyDictionary<TKey, TValue>
    {
        public ImmutableDictionary() : base(new Dictionary<TKey, TValue>())
        {
        }

        public ImmutableDictionary(int capacity)
            : 
            base(new Dictionary<TKey, TValue>(capacity))
        {
        }

        public ImmutableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items)
            :
            base(new Dictionary<TKey, TValue>(items.ToArray()))
        {
        }
    }
}
