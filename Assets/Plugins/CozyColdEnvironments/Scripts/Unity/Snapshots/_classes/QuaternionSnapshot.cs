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
        private float x;

        [JsonInclude]
        [SerializeField]
        private float y;

        [JsonInclude]
        [SerializeField]
        private float z;

        [JsonInclude]
        [SerializeField]
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

        [JsonConstructor]
        public QuaternionSnapshot(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public override Quaternion Restore(Quaternion target)
        {
            return new Quaternion(x, y, z, w);
        }
    }
}
