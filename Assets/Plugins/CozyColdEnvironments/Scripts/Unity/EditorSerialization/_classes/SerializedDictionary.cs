using CCEnvs;
using CCEnvs.Collections;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.Serialization
{
    [Serializable]
    public sealed class SerializedDictionary<TKey, TValue>
        : Serialized<Dictionary<TKey, TValue>>
    {
        [SerializeField]
        private SerializedTuple<TKey, TValue>[] items;

        public SerializedDictionary()
        {
        }

        public SerializedDictionary(Dictionary<TKey, TValue> defaultValue)
            :
            base(defaultValue)
        {
        }

        protected override Dictionary<TKey, TValue> ValueFactory()
        {
            var collection = new Dictionary<TKey, TValue>(this.items.Length);

            var items = this.items.Select(x => x.Value.ToKeyValuePair()).DistinctBy(pair => pair.Key);
            collection.AddRange(items);

            return collection;
        }
    }
}
