using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace CCEnvs.Unity
{
    [Serializable]
    public sealed class QuaternionSnapshot : Snapshot<Quaternion>
    {
        [JsonInclude]
        [SerializeField]
        [JsonPropertyName("x")]
        private float x;

        [JsonInclude]
        [SerializeField]
        [JsonPropertyName("y")]
        private float y;

        [JsonInclude]
        [SerializeField]
        [JsonPropertyName("z")]
        private float z;

        [JsonInclude]
        [SerializeField]
        [JsonPropertyName("w")]
        private float w;

        public QuaternionSnapshot()
        {
        }

        public QuaternionSnapshot(Quaternion target) : base(target)
        {
            x = target.x;
            y = target.y;
            z = target.z;
            w = target.w;
        }

        public override Quaternion Restore(Quaternion target)
        {
            return new Quaternion(x, y, z, w);
        }
    }
}
