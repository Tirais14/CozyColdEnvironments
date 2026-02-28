using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using System;
using UnityEngine;

namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    [TypeSerializationDescriptor("QuaternionSnapshot", "f6c48935-fce4-4ba6-bbf2-e815ccb1fa4f")]
    public sealed record QuaternionSnapshot : Snapshot<Quaternion>
    {
        [field: SerializeField]
        public float? X { get; private set; }

        [field: SerializeField]
        public float? Y { get; private set; }

        [field: SerializeField]
        public float? Z { get; private set; }

        [field: SerializeField]
        public float? W { get; private set; }

        public QuaternionSnapshot()
        {
        }

        public QuaternionSnapshot(Quaternion target) : base(target)
        {
        }

        public QuaternionSnapshot(Snapshot<Quaternion> original) : base(original)
        {
        }

        protected override void OnRestore(ref Quaternion target)
        {
            if (X.HasValue)
                target.x = X.Value;

            if (Y.HasValue)
                target.y = Y.Value;

            if (Z.HasValue)
                target.z = Z.Value;

            if (W.HasValue)
                target.w = W.Value;
        }

        protected override void OnCapture(Quaternion target)
        {
            base.OnCapture(target);

            X = target.x;
            Y = target.y;
            Z = target.z;
            W = target.w;
        }

        protected override void OnReset()
        {
            base.OnReset();

            X = default;
            Y = default;
            Z = default;
            W = default;
        }
    }
}
