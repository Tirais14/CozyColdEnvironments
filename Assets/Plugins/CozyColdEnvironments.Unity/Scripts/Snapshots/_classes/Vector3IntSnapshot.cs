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

        protected override void OnRestore(ref Vector3Int target)
        {
            target.x = X;
            target.y = Y;
            target.z = Z;
        }
    }
}
