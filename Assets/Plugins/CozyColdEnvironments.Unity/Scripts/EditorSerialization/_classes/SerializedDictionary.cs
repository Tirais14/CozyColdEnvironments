using CCEnvs.Collections;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public sealed class SerializedDictionary<TKey, TValue>
        :
        Serialized<Dictionary<TKey, TValue>>,
        ISerializationCallbackReceiver
    {
        [SerializeField]
        [HideInInspector]
        private SerializedKeyValuePair<TKey, TValue> defaultItem;

        [SerializeField]
        private SerializedKeyValuePair<TKey, TValue>[] items;

        private EqualityComparer<SerializedKeyValuePair<TKey, TValue>> ItemEqualityComparer => EqualityComparer<SerializedKeyValuePair<TKey, TValue>>.Default;

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

            var items = this.items.Skip(1)
                .Select(pair => pair.Deserialized)
                .DistinctBy(pair => pair.Key);

            collection.AddRange(items);

            this.items = null!;
            defaultItem = default;

            return collection;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (items is null)
                return;

            items = items.Where(item =>
                {
                    return !EqualityComparer<SerializedKeyValuePair<TKey, TValue>>.Default.Equals(item, defaultItem);
                })
                .Prepend(defaultItem)
                .ToArray();
        }
    }
}
