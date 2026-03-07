using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using CCEnvs.Collections;
using CCEnvs.Linq;
using CCEnvs.Snapshots;
using Newtonsoft.Json;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    public readonly struct SaveFileSceneData : IEquatable<SaveFileSceneData>
    {
        public SceneInfo SceneInfo { get; }
        public IList<KeyedSnapshot<ISnapshot>> Snapshots { get; }

        [JsonConstructor]
        public SaveFileSceneData(SceneInfo sceneInfo, IList<KeyedSnapshot<ISnapshot>> snapshots)
        {
            CC.Guard.IsNotNull(snapshots, nameof(snapshots));

            SceneInfo = sceneInfo;
            Snapshots = snapshots.ToImmutableList();
        }

        public static bool operator ==(SaveFileSceneData left, SaveFileSceneData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveFileSceneData left, SaveFileSceneData right)
        {
            return !(left == right);
        }

        public readonly bool Equals(SaveFileSceneData other)
        {
            return SceneInfo == other.SceneInfo
                   &&
                   Snapshots.EqualsByElements(other.Snapshots);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SaveFileSceneData typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(SceneInfo, Snapshots.HashCodeByElements());
        }

        public readonly override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return $"{nameof(SceneInfo)}: {SceneInfo}";
        }
    }
}
