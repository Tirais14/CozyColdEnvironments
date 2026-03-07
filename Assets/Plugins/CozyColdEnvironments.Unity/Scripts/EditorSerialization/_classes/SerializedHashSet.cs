using System;
using System.Collections.Generic;
using System.Linq;
using CCEnvs.Collections;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public sealed class SerializedHashSet<T> : Serialized<HashSet<T>>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private T[] items = new arr<T>();

        [SerializeField]
        [HideInInspector]
        private T? defaultItem;

        public SerializedHashSet()
        {
        }

        public SerializedHashSet(HashSet<T> defaultValue) : base(defaultValue)
        {
        }

        protected override HashSet<T> ValueFactory()
        {
            var set = new HashSet<T>(items.Length);

            set.AddRange(items.Skip(1));

            items = null!;
            defaultItem = default;

            return set;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (items is null)
                return;

            items = items.Where(item =>
                {
                    return !EqualityComparer<T?>.Default.Equals(item, defaultItem);
                })
                .Prepend(defaultItem)
                .ToArray()!;
        }
    }
}
