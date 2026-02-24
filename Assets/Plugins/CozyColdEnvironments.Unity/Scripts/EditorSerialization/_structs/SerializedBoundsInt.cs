using System;
using UnityEngine;

namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public struct SerializedBoundsInt : IEditorSerialized<BoundsInt>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private Vector3Int position;

        [SerializeField]
        private Vector3Int size;

        public BoundsInt Deserialized { readonly get; private set; }

        public SerializedBoundsInt(BoundsInt bounds)
            :
            this()
        {
            Deserialized = bounds;
        }

        public static implicit operator BoundsInt(SerializedBoundsInt source)
        {
            return source.Deserialized;
        }

        readonly void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Deserialized = new BoundsInt(position, size);
        }
    }
}
