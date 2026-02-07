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

        protected override void OnRestore(ref Vector2Int target)
        {
            target.x = X;
            target.y = Y;
        }
    }
}
