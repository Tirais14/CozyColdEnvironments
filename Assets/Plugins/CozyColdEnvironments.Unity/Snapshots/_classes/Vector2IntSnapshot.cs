using CCEnvs.Snapshots;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public sealed class Vector2IntSnapshot : Snapshot<Vector2Int>
    {
        [field: SerializeField]
        public int X { get; set; }

        [field: SerializeField]
        public int Y { get; set; }

        public Vector2IntSnapshot()
        {
        }

        public Vector2IntSnapshot(Vector2Int target) : base(target)
        {
            X = target.x;
            Y = target.y;
        }

        public override bool TryRestore(Vector2Int target, out Vector2Int restored)
        {
            restored = new Vector2Int(X, Y);
            return true;
        }
    }
}
