using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace CCEnvs.Unity
{
    [Serializable]
    public sealed class Vector2Snapshot : Snapshot<Vector2>
    {
        [JsonInclude]
        [SerializeField]
        private float x;

        [JsonInclude]
        [SerializeField]
        private float y;

        public Vector2Snapshot()
        {
        }

        public Vector2Snapshot(Vector2 target) : base(target)
        {
            x = target.x;
            y = target.y;
        }

        [JsonConstructor]
        public Vector2Snapshot(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override Vector2 Restore(Vector2 target)
        {
            return new Vector2(x, y);
        }
    }
}
