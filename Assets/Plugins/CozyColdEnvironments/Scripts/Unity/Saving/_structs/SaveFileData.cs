using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    [Serializable]
    public readonly struct SaveFileData : IEquatable<SaveFileData>
    {
        public string Version { get; }
        public IReadOnlyList<SaveSceneData> SceneDatas { get;}

        [JsonConstructor]
        public SaveFileData(string version, IReadOnlyList<SaveSceneData> sceneDatas)
        {
            Version = version;
            SceneDatas = sceneDatas.ToImmutableArray();
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
            if (SceneDatas.IsNullOrEmpty())
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
