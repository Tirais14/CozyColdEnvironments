using CCEnvs.Snapshots;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public sealed class Vector3IntSnapshot : Snapshot<Vector3Int>
    {
        [field: SerializeField]
        public int X { get; set; }

        [field: SerializeField]
        public int Y { get; set; }

        [field: SerializeField]
        public int Z { get; set; }

        public Vector3IntSnapshot()
        {
        }

        public Vector3IntSnapshot(Vector3Int target) : base(target)
        {
            X = target.x;
            Y = target.y;
            Z = target.z;
        }

        public override Vector3Int Restore(Vector3Int target)
        {
            return new Vector3Int(X, Y, Z);
        }
    }
}
