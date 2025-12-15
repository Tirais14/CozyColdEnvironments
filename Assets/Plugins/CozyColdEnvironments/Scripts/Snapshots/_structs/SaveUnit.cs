#nullable enable
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;

namespace CCEnvs
{
    [Serializable]
    public readonly struct SaveUnit
    {
        [JsonProperty("type")]
        private readonly Type snapshotType;

        [JsonProperty("content")]
        private readonly string snapshotContent;

        public SaveUnit(ISnapshot snapshot)
        {
            CC.Guard.IsNotNull(snapshot, nameof(snapshot));

            snapshotType = snapshot.GetType();
            snapshotContent = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
        }

        [JsonConstructor]
        public SaveUnit(Type snapshotType, string snapshotContent)
        {
            Guard.IsNotNull(snapshotType);
            Guard.IsNotNull(snapshotContent);

            this.snapshotType = snapshotType;
            this.snapshotContent = snapshotContent;
        }

        public readonly ISnapshot? Deserialize()
        {
            return (ISnapshot?)JsonConvert.DeserializeObject(snapshotContent, snapshotType);
        }
    }
}
