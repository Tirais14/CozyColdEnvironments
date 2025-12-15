using CCEnvs.Snapshots;
using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    public sealed class Vector3IntSnapshot : Snapshot<Vector3Int>
    {
        [SerializeField]
        [JsonProperty("x")]
        private int x;

        [SerializeField]
        [JsonProperty("y")]
        private int y;

        [SerializeField]
        [JsonProperty("z")]
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

        public override Vector3Int Restore(Vector3Int target)
        {
            return new Vector3Int(x, y, z);
        }
    }
}
