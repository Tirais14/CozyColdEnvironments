using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    public sealed class Vector2IntSnapshot : Snapshot<Vector2Int>
    {
        [SerializeField]
        [JsonPropertyName("x")]
        public int X { get; private set; }

        [SerializeField]
        [JsonPropertyName("y")]
        public int Y { get; private set; }

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
