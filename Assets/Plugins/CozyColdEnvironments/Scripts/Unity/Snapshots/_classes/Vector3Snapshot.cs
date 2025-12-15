using CCEnvs.Snapshots;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using UnityEngine;

namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    public sealed class Vector3Snapshot : Snapshot<Vector3>
    {
        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("x")]
        private float x;

        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("x")]
        private float y;

        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("x")]
        private float z;

        public Vector3Snapshot()
        {
        }

        public Vector3Snapshot(Vector3 target) : base(target)
        {
            x = target.x;
            y = target.y;
            z = target.z;
        }

        public override Vector3 Restore(Vector3 target)
        {
            return new Vector3(x, y, z);
        }
    }
}
