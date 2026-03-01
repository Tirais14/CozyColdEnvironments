using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    [SerializationDescriptor("Vector3IntSnapshot", "3f223844-7484-4b29-9f0c-3ed152b974e1")]
    public sealed record Vector3IntSnapshot : Snapshot<Vector3Int>
    {
        [field: SerializeField]
        public int? X { get; set; }

        [field: SerializeField]
        public int? Y { get; set; }

        [field: SerializeField]
        public int? Z { get; set; }

        public Vector3IntSnapshot()
        {
        }

        public Vector3IntSnapshot(Vector3Int target) : base(target)
        {
        }

        protected override void OnRestore(ref Vector3Int target)
        {
            if (X.HasValue)
                target.x = X.Value;

            if (Y.HasValue)
                target.y = Y.Value;

            if (Z.HasValue)
                target.z = Z.Value;
        }

        protected override void OnCapture(Vector3Int target)
        {
            base.OnCapture(target);

            X = target.x;
            Y = target.y;
            Z = target.z;
        }

        protected override void OnReset()
        {
            base.OnReset();

            X = default;
            Y = default;
            Z = default;
        }
    }
}
