using System;
using UnityEngine;

namespace CCEnvs.Unity.Serialization
{
    [Serializable]
    public struct SerializedBoundsInt : IEditorSerialized<BoundsInt>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private Vector3Int position;

        [SerializeField]
        private Vector3Int size;

        public BoundsInt Value { readonly get; private set; }

        public SerializedBoundsInt(BoundsInt bounds)
            :
            this()
        {
            Value = bounds;
        }

        public static implicit operator BoundsInt(SerializedBoundsInt source)
        {
            return source.Value;
        }

        readonly void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Value = new BoundsInt(position, size);
        }
    }
}
