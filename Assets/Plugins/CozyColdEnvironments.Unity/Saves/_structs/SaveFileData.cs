using CCEnvs.Collections;
using CCEnvs.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    public readonly struct SaveFileData : IEquatable<SaveFileData>
    {
        public string Version { get; }
        public IList<SaveFileSceneData> SceneDatas { get;}

        [JsonConstructor]
        public SaveFileData(string version, IList<SaveFileSceneData> sceneDatas)
        {
            Version = version;
            SceneDatas = sceneDatas.ToImmutableList();
        }

        public static bool operator ==(SaveFileData left, SaveFileData right)
        {
            return left.Equals(right); 
        }

        public static bool operator !=(SaveFileData left, SaveFileData right)
        {
            return !(left == right);
        }

        public readonly bool Equals(SaveFileData other)
        {
            return Version == other.Version
                   &&
                   SceneDatas.EqualsByElements(other.SceneDatas);
        }

        public readonly override bool Equals(object obj)
        {
            return obj is SaveFileData typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Version, SceneDatas.HashCodeByElements());
        }

        public override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return $"{nameof(Version)}: {Version}";
        }
    }
}
