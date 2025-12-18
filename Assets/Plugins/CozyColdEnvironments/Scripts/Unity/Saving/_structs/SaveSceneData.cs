using CCEnvs.Snapshots;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    [Serializable]
    public readonly struct SaveSceneData : IEquatable<SaveSceneData>
    {
        public SceneInfo SceneInfo { get; }

        public IReadOnlyList<ISnapshot> Snapshots { get; }

        [JsonConstructor]
        public SaveSceneData(SceneInfo sceneInfo, IReadOnlyList<ISnapshot> snapshots)
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
            int length = Snapshots.Count;
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
