using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace CCEnvs.Unity
{
    [Serializable]
    public sealed class QuaternionSnapshot : Snapshot<Quaternion>
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

        [SerializeField]
        [JsonPropertyName("w")]
        public float W { get; private set; }

        public QuaternionSnapshot()
        {
        }

        public QuaternionSnapshot(Quaternion target) : base(target)
        {
            X = target.x;
            Y = target.y;
            Z = target.z;
            W = target.w;
        }

        public override Quaternion Restore(Quaternion target)
        {
            return new Quaternion(X, Y, Z, W);
        }
    }
}
