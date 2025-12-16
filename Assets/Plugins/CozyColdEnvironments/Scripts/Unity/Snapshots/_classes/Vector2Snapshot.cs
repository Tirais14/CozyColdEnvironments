using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace CCEnvs.Unity
{
    [Serializable]
    public sealed class Vector2Snapshot : Snapshot<Vector2>
    {
        [SerializeField]
		[JsonPropertyName("x")]
        public float X { get; private set; }

        [SerializeField]
		[JsonPropertyName("y")]
        public float Y { get; private set; }

        public Vector2Snapshot()
        {
        }

        public Vector2Snapshot(Vector2 target) : base(target)
        {
            X = target.x;
            Y = target.y;
        }

        public override Vector2 Restore(Vector2 target)
        {
            return new Vector2(X, Y);
        }
    }
}
