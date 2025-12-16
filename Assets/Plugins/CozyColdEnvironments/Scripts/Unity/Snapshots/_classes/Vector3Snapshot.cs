using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    public sealed class Vector3Snapshot : Snapshot<Vector3>
    {
        [SerializeField]
		[JsonPropertyName("x")]
        public float X { get; private set; }

        [SerializeField]
		[JsonPropertyName("y")]
        public float Y { get; private set; }

        [SerializeField]
		[JsonPropertyName("z")]
        public float Z { get; private set; }

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
