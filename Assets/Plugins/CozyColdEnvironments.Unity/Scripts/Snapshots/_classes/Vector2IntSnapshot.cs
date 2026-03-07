using System;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    [SerializationDescriptor("Vector2IntSnapshot", "36645266-52d5-41b1-a907-d556cdeb61f4")]
    public sealed record Vector2IntSnapshot : Snapshot<Vector2Int>
    {
        [field: SerializeField]
        public int? X { get; set; }

        [field: SerializeField]
        public int? Y { get; set; }

        public Vector2IntSnapshot()
        {
        }

        public Vector2IntSnapshot(Vector2Int target) : base(target)
        {
        }

        protected override void OnRestore(ref Vector2Int target)
        {
            if (X.HasValue)
                target.x = X.Value;

            if (Y.HasValue)
                target.y = Y.Value;
        }

        protected override void OnCapture(Vector2Int target)
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
