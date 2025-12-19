using CCEnvs.Snapshots;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    [Serializable]
    public readonly struct SaveFileSceneData : IEquatable<SaveFileSceneData>
    {
        public SceneInfo SceneInfo { get; }

        public IReadOnlyList<ISnapshot> Snapshots { get; }

        [JsonConstructor]
        public SaveFileSceneData(SceneInfo sceneInfo, IReadOnlyList<ISnapshot> snapshots)
        {
            CC.Guard.IsNotNull(snapshots, nameof(snapshots));

            SceneInfo = sceneInfo;
            Snapshots = snapshots.ToImmutableArray();
        }

        public static bool operator ==(SaveFileSceneData left, SaveFileSceneData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveFileSceneData left, SaveFileSceneData right)
        {
            return !(left == right);
        }

        public readonly void Apply()
        {
            int length = Snapshots.Count;
            for (int i = 0; i < length; i++)
                Snapshots[i].Restore();
        }

        public readonly bool Equals(SaveFileSceneData other)
        {
            return Snapshots == other.Snapshots
                   &&
                   SceneInfo == other.SceneInfo;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SaveFileSceneData typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Snapshots, SceneInfo);
        }
    }
}
