using System;
using CCEnvs.Snapshots;
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

        protected override void OnRestore(ref Vector2 target)
        {
            target.x = X;
            target.y = Y;
        }
    }
}
