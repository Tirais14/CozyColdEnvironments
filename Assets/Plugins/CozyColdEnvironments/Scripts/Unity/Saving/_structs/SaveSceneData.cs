using CCEnvs.Snapshots;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    [Serializable]
    public struct SaveSceneData : IEquatable<SaveSceneData>
    {
		[JsonPropertyName("scene")]
        public SceneInfo SceneInfo { get; private set; }

        [JsonPropertyName("snapshots")]
        public ImmutableArray<ISnapshot> Snapshots { get; private set; }

        [JsonConstructor]
        public SaveSceneData(SceneInfo sceneInfo, IEnumerable<ISnapshot> snapshots)
        {
            CC.Guard.IsNotNull(snapshots, nameof(snapshots));

            SceneInfo = sceneInfo;
            Snapshots = snapshots.ToImmutableArray();
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
            int length = Snapshots.Length;
            for (int i = 0; i < length; i++)
                Snapshots[i].Restore();
        }

        public readonly bool Equals(SaveSceneData other)
        {
            return Snapshots == other.Snapshots
                   &&
                   SceneInfo == other.SceneInfo;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SaveSceneData typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Snapshots, SceneInfo);
        }
    }
}
