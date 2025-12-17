using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public sealed class Vector3IntSnapshot : Snapshot<Vector3Int>
    {
        [JsonInclude]
        [SerializeField]
        private int x;

        [JsonInclude]
        [SerializeField]
        private int y;

        [JsonInclude]
        [SerializeField]
        private int z;

        public Vector3IntSnapshot()
        {
        }

        public Vector3IntSnapshot(Vector3Int target) : base(target)
        {
            x = target.x;
            y = target.y;
            z = target.z;
        }

        [JsonConstructor]
        public Vector3IntSnapshot(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override Vector3Int Restore(Vector3Int target)
        {
            return new Vector3Int(x, y, z);
        }
    }
}
