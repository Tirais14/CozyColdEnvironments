using System;
using System.Collections.Generic;
using UnityEngine;
using CozyColdEnvironments.Unity.Serialization;

#nullable enable
#pragma warning disable S2955
namespace CozyColdEnvironments
{
    [Serializable]
    public struct SerializedKeyValuePair<TKey, TValue> :
        IUnitySerialized<KeyValuePair<TKey, TValue>>
    {
        [SerializeField]
        private TKey key;

        [SerializeField]
        private TValue value;

        public readonly KeyValuePair<TKey, TValue> Value => new(key, value);

        public SerializedKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public static implicit operator KeyValuePair<TKey, TValue>(SerializedKeyValuePair<TKey, TValue> serialized)
        {
            return serialized.Value;
        }
    }
}
