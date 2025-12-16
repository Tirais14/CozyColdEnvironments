using CCEnvs.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    [Serializable]
    public struct SaveFileData : IEquatable<SaveFileData>, IEnumerable<SaveSceneData>
    {
        [JsonInclude]
        [JsonPropertyName("contexts")]
        private SaveSceneData[] contexts;

        [JsonInclude]
        [JsonPropertyName("version")]
        public string Version { get; private set; }

        public SaveFileData(IEnumerable<SaveSceneData> contexts, string version)
        {
            this.contexts = contexts.ToArray();
            Version = version;
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
            return contexts == other.contexts
                   &&
                   Version == other.Version;
        }

        public readonly void ApplyToLoadedScenes()
        {
            HashSet<SceneInfo> loadedSceneInfos = SceneManagerHelper.GetLoadedScenes()
                                                                    .Select(x => x.GetSceneInfo())
                                                                    .ToHashSet();

            foreach (var ctx in contexts)
            {
                if (!loadedSceneInfos.Contains(ctx.SceneInfo))
                    continue;

                ctx.Apply();
            }
        }

        public readonly override bool Equals(object obj)
        {
            return obj is SaveFileData typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(contexts, Version);
        }

        public readonly IEnumerator<SaveSceneData> GetEnumerator()
        {
            return contexts.GetEnumeratorT();
        }

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
