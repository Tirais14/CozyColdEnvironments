using CCEnvs.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    public readonly struct SaveFileData : IEquatable<SaveFileData>
    {
        public string Version { get; }
        public IReadOnlyList<SaveFileSceneData> SceneDatas { get;}

        [JsonConstructor]
        public SaveFileData(string version, IReadOnlyList<SaveFileSceneData> sceneDatas)
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

        /// <returns>not restored</returns>
        public readonly IList<SaveFileSceneData> RestoreLoadedScenes()
        {
            if (SceneDatas.IsNullOrEmpty())
                return Array.Empty<SaveFileSceneData>();

            SceneInfo[] sceneInfos = SceneManagerHelper.GetLoadedScenes()
                .Select(x => x.GetSceneInfo())
                .ToArray();

            var notRestored = new LazyLight<List<SaveFileSceneData>>(static () => new List<SaveFileSceneData>());
            foreach (var sceneData in SceneDatas)
            {
                if (sceneData.SceneInfo == null || !sceneInfos.Contains(sceneData.SceneInfo!.Value))
                {
                    notRestored.Value.Add(sceneData);
                    continue;
                }

                try
                {
                    sceneData.RestoreScene();
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }

            return notRestored.HasValue ? notRestored.Value : Array.Empty<SaveFileSceneData>();
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

        public override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return $"Version \"{Version}\"";
        }
    }
}
