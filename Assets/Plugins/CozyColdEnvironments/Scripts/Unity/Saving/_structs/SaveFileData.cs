using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    [Serializable]
    public struct SaveFileData : IEquatable<SaveFileData>
    {
        [JsonPropertyName("version")]
        public string Version { get; private set; }

        [JsonPropertyName("contexts")]
        public ImmutableArray<SaveSceneData> SceneDatas { get; private set; }

        public SaveFileData(string version, IEnumerable<SaveSceneData> sceneDatas)
        {
            Version = version;
            this.SceneDatas = sceneDatas.ToImmutableArray();
        }

        public static bool operator ==(SaveFileData left, SaveFileData right)
        {
            return left.Equals(right); 
        }

        public static bool operator !=(SaveFileData left, SaveFileData right)
        {
            return !(left == right);
        }

        public readonly void ApplyToLoadedScenes()
        {
            if (SceneDatas.IsDefaultOrEmpty)
                return;

            HashSet<SceneInfo> loadedSceneInfos = SceneManagerHelper.GetLoadedScenes()
                .Select(x => x.GetSceneInfo())
                .ToHashSet();

            foreach (var ctx in SceneDatas)
            {
                if (!loadedSceneInfos.Contains(ctx.SceneInfo))
                    continue;

                ctx.Apply();
            }
        }

        public readonly bool Equals(SaveFileData other)
        {
            return SceneDatas == other.SceneDatas
                   &&
                   Version == other.Version;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is SaveFileData typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(SceneDatas, Version);
        }
    }
}
