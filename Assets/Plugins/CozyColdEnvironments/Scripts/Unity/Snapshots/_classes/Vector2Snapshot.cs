using CCEnvs.Snapshots;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using UnityEngine;

namespace CCEnvs.Unity
{
    [Serializable]
    public class Vector2Snapshot : Snapshot<Vector2>
    {
        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("x")]
        private float x;

        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("y")]
        private float y;

        public Vector2Snapshot()
        {
        }

        public Vector2Snapshot(Vector2 target) : base(target)
        {
        }

        public override Vector2 Restore(Vector2 target)
        {
            throw new System.NotImplementedException();
        }
    }
}
