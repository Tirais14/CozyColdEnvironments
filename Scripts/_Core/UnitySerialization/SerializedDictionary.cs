using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;
using UTIRLib.Unity.Serialization;

#nullable enable
#pragma warning disable S1155
namespace UTIRLib
{
    [Serializable]
    public sealed class SerializedDictionary<TKey, TValue> :
        IUnitySerialized<Dictionary<TKey, TValue>>,
        ISerializationCallbackReceiver
    {
        private IDictionary<TKey, TValue> serializedCollection = new Dictionary<TKey, TValue>();

        [SerializeField]
        private SerializedKeyValuePair<TKey, TValue>[] serializedItems = null!;

        public Dictionary<TKey, TValue> Value => (Dictionary<TKey, TValue>)serializedCollection;

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

            serializedCollection = tempCollection;

            serializedItems = validItems.ToArray();
        }

        private static bool IsDefault<T>(T key)
        {
            if (key.IsDefault(IsDefaultOption.IncludeWhitespaceOrEmptyString))
                return true;

            return ObjectValidator.IsDefaultByTypeFieldsAndFieldsValues(key,
                isDefaultOption: IsDefaultOption.IncludeWhitespaceOrEmptyString);
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
