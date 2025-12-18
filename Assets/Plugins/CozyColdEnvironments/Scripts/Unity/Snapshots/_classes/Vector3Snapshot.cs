using CCEnvs.Snapshots;
using System;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public sealed class Vector3Snapshot : Snapshot<Vector3>
    {
        [field: SerializeField]
        public float X { get; set; }

        [field: SerializeField]
        public float Y { get; set; }

        [field: SerializeField]
        public float Z { get; set; }

        public Vector3Snapshot()
        {
        }

        public Vector3Snapshot(Vector3 target) : base(target)
        {
            X = target.x;
            Y = target.y;
            Z = target.z;
        }

        public override Vector3 Restore(Vector3 target)
        {
            return new Vector3(X, Y, Z);
        }
    }
}
