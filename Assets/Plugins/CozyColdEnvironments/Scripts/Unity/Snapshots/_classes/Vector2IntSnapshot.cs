using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public sealed class Vector2IntSnapshot : Snapshot<Vector2Int>
    {
        [JsonInclude]
        [SerializeField]
        private int x;

        [JsonInclude]
        [SerializeField]
        private int y;

        public Vector2IntSnapshot()
        {
        }

        public Vector2IntSnapshot(Vector2Int target) : base(target)
        {
            x = target.x;
            y = target.y;
        }

        [JsonConstructor]
        public Vector2IntSnapshot(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override Vector2Int Restore(Vector2Int target)
        {
            return new Vector2Int(x, y);
        }
    }
}
