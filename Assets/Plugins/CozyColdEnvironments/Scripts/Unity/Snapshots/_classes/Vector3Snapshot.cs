using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    public sealed class Vector3Snapshot : Snapshot<Vector3>
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

        public Vector3Snapshot()
        {
        }

        public Vector3Snapshot(Vector3 target) : base(target)
        {
            x = target.x;
            y = target.y;
            z = target.z;
        }

        [JsonConstructor]
        public Vector3Snapshot(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override Vector3 Restore(Vector3 target)
        {
            return new Vector3(x, y, z);
        }
    }
}
