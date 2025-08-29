using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;
using UTIRLib.Unity.Serialization;

#nullable enable
namespace UTIRLib
{
    [Serializable]
    public sealed class SerializedDictionary<TKey, TValue> :
        IUnitySerialized<Dictionary<TKey, TValue>>,
        ISerializationCallbackReceiver
    {
        private Dictionary<TKey, TValue> serializedCollection = new();

        [SerializeField]
        private SerializedKeyValuePair<TKey, TValue>[] serializedItems = null!;

        public Dictionary<TKey, TValue> Value => serializedCollection;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            IDictionary<TKey, TValue> tempCollection = new Dictionary<TKey, TValue>(serializedItems.Length);
            var validItems = new List<SerializedKeyValuePair<TKey, TValue>>(serializedItems.Length);

            KeyValuePair<TKey, TValue> pair;
            foreach (var serialized in serializedItems)
            {
                pair = serialized.Value;

                if (tempCollection.ContainsKey(pair.Key))
                    continue;

                tempCollection.Add(pair);
                validItems.Add(serialized);
            }

            serializedCollection = (Dictionary<TKey, TValue>)tempCollection;

            serializedItems = validItems.ToArray();
        }

        private static bool IsDefault<T>(T key)
        {
            return ObjectValidator.EqaulsDefaultByFieldsAndItTypes(key,
                isDefaultOption: EqualsDefaultOption.IncludeWhitespaceOrEmptyString);
        }

        private void TryAddEmptySerializedItem()
        {
            if (!serializedCollection.Keys.Any(x => IsDefault(x))
                && 
                !serializedItems.Any(x => IsDefault(x.Value.Key)))
            {
                serializedItems = new SerializedKeyValuePair<TKey, TValue>[serializedCollection.Count + 1];
                serializedCollection.ToSeralizedPairs().CopyTo(serializedItems, 0);
                serializedItems[^1] = default;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            serializedItems = serializedCollection.ToSeralizedPairs();

            TryAddEmptySerializedItem();
        }

        public static implicit operator Dictionary<TKey, TValue>(SerializedDictionary<TKey, TValue> srdDictionary)
        {
            return srdDictionary.Value;
        }
    }
}
