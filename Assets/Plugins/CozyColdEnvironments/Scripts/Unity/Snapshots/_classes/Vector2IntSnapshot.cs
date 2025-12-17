using CCEnvs.Snapshots;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public sealed class Vector2IntSnapshot : Snapshot<Vector2Int>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2IntSnapshot()
        {
        }

        public Vector2IntSnapshot(Vector2Int target) : base(target)
        {
            X = target.x;
            Y = target.y;
        }

        public override Vector2Int Restore(Vector2Int target)
        {
            return new Vector2Int(X, Y);
        }
    }
}
