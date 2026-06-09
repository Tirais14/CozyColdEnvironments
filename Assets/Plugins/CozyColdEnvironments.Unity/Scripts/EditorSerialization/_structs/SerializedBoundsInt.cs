using System;
using UnityEngine;

namespace CCEnvs.UnityX.EditorSerialization
{
    [Serializable]
    public struct SerializedBoundsInt : IEditorSerialized<BoundsInt>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private Vector3Int position;

        [SerializeField]
        private Vector3Int size;

        public BoundsInt Data { readonly get; private set; }

        public SerializedBoundsInt(BoundsInt bounds)
            :
            this()
        {
            Data = bounds;
        }

        public static implicit operator BoundsInt(SerializedBoundsInt source)
        {
            return source.Data;
        }

        readonly void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Data = new BoundsInt(position, size);
        }
    }
}
