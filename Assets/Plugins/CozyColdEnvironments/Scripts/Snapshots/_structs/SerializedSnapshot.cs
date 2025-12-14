#nullable enable
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;

namespace CCEnvs.Snapshots
{
    [Serializable]
    public readonly struct SerializedSnapshot
    {
        [JsonProperty("type")]
        private readonly Type snapshotType;

        [JsonProperty("content")]
        private readonly string snapshotContent;

        public SerializedSnapshot(ISnapshot snapshot)
        {
            CC.Guard.IsNotNull(snapshot, nameof(snapshot));

            snapshotType = snapshot.GetType();
            snapshotContent = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
        }

        [JsonConstructor]
        public SerializedSnapshot(Type snapshotType, string snapshotContent)
        {
            Guard.IsNotNull(snapshotType);
            Guard.IsNotNull(snapshotContent);

            this.snapshotType = snapshotType;
            this.snapshotContent = snapshotContent;
        }

        public readonly ISnapshot Deserialize()
        {
            return JsonConvert.DeserializeObject(snapshotContent, snapshotType).To<ISnapshot>();
        }
    }
}
