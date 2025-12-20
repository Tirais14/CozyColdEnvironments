using CCEnvs.Snapshots;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    public readonly struct SaveFileSceneData : IEquatable<SaveFileSceneData>
    {
        public SceneInfo? SceneInfo { get; }
        public IReadOnlyList<KeyedSnapshot<ISnapshot>> Snapshots { get; }

        [JsonConstructor]
        public SaveFileSceneData(SceneInfo? sceneInfo, IReadOnlyList<KeyedSnapshot<ISnapshot>> snapshots)
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

        public readonly void RestoreScene()
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

        public readonly override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return $"Scene info \"{SceneInfo}\"";
        }
    }
}
