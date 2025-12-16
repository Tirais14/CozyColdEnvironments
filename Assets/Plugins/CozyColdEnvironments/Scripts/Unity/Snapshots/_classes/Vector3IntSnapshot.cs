using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    public sealed class Vector3IntSnapshot : Snapshot<Vector3Int>
    {
        [SerializeField]
		[JsonPropertyName("x")]
        public int X { get; private set; }

        [SerializeField]
		[JsonPropertyName("y")]
        public int Y { get; private set; }

        [SerializeField]
		[JsonPropertyName("z")]
        public int Z { get; private set; }

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
