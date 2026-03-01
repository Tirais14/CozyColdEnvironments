using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using System;
using UnityEngine;

namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    [SerializationDescriptor("Vector3Snapshot", "80f07fb0-8669-4054-b9aa-2a85fc566b64")]
    public sealed record Vector3Snapshot : Snapshot<Vector3>
    {
        [field: SerializeField]
        public float? X { get; set; }

        [field: SerializeField]
        public float? Y { get; set; }

        [field: SerializeField]
        public float? Z { get; set; }

        public Vector3Snapshot()
        {
        }

        public Vector3Snapshot(Vector3 target) : base(target)
        {
        }

        protected override void OnRestore(ref Vector3 target)
        {
            if (X.HasValue)
                target.x = X.Value;

            if (Y.HasValue)
                target.y = Y.Value;

            if (Z.HasValue)
                target.z = Z.Value;
        }

        protected override void OnCapture(Vector3 target)
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
