using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using System;
using UnityEngine;

namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    [SerializationDescriptor("Vector2Snapshot", "e7762602-0d73-4577-8fbe-4aa37390a929")]
    public sealed record Vector2Snapshot : Snapshot<Vector2>
    {
        [field: SerializeField]
        public float? X { get; set; }

        [field: SerializeField]
        public float? Y { get; set; }

        public Vector2Snapshot()
        {
        }

        public Vector2Snapshot(Vector2 target) : base(target)
        {
        }

        protected override void OnRestore(ref Vector2 target)
        {
            if (X.HasValue)
                target.x = X.Value;

            if (Y.HasValue)
                target.y = Y.Value;
        }

        protected override void OnCapture(Vector2 target)
        {
            base.OnCapture(target);

            X = target.x;
            Y = target.y;
        }

        protected override void OnReset()
        {
            base.OnReset();

            X = default;
            Y = default;
        }
    }
}
