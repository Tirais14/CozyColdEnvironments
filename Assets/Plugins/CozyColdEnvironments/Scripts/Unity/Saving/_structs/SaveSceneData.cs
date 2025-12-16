using CCEnvs.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    [Serializable]
    public struct SaveSceneData : IEquatable<SaveSceneData>
    {
        [JsonInclude]
        [JsonPropertyName("snapshots")]
        private ISnapshot[] snapshots;

        [JsonInclude]
		[JsonPropertyName("scene")]
        public SceneInfo SceneInfo { get; }

        [JsonIgnore]
        public readonly IEnumerable<ISnapshot> Snapshots => snapshots;

        [JsonConstructor]
        public SaveSceneData(SceneInfo sceneInfo, IEnumerable<ISnapshot> snapshots)
        {
            SceneInfo = sceneInfo;
            this.snapshots = snapshots.ToArray();
        }

        public static bool operator ==(SaveSceneData left, SaveSceneData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveSceneData left, SaveSceneData right)
        {
            return !(left == right);
        }

        public readonly void Apply()
        {
            int length = snapshots.Length;
            for (int i = 0; i < length; i++)
                snapshots[i].Restore();
        }

        public readonly bool Equals(SaveSceneData other)
        {
            return snapshots == other.snapshots
                   &&
                   SceneInfo == other.SceneInfo;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SaveSceneData typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(snapshots, SceneInfo);
        }
    }
}
