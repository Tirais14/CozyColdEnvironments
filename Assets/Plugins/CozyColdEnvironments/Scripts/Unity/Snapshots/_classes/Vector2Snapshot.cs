using CCEnvs.Snapshots;
using System;
using UnityEngine;

namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public sealed class Vector2Snapshot : Snapshot<Vector2>
    {
        [field: SerializeField]
        public float X { get; set; }

        [field: SerializeField]
        public float Y { get; set; }

        public Vector2Snapshot()
        {
        }

        public Vector2Snapshot(Vector2 target) : base(target)
        {
            X = target.x;
            Y = target.y;
        }

        public override bool TryRestore(Vector2 target, out Vector2 restored)
        {
            restored = new Vector2(X, Y);
            return true;
        }
    }
}
