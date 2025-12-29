using CCEnvs.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ZLinq;

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

        /// <returns>not restored</returns>
        public readonly IList<SaveFileSceneData> RestoreLoadedScenes()
        {
            if (SceneDatas.IsNullOrEmpty())
                return Array.Empty<SaveFileSceneData>();

            SceneInfo[] sceneInfos = SceneManagerHelper.GetLoadedScenes()
                .Select(x => x.GetSceneInfo())
                .ToArray();

            var notRestoredSceneDatas = LazyLight.Create<List<SaveFileSceneData>>();
            foreach (var sceneData in SceneDatas)
            {
                if (sceneData.SceneInfo != null && !sceneInfos.Contains(sceneData.SceneInfo))
                {
                    notRestoredSceneDatas.Value.Add(sceneData);
                    continue;
                }

                try
                {
                    var restSnapshots = sceneData.RestoreScene();

                    if (restSnapshots.IsNotEmpty())
                    {
                        var notRestoredSceneData = new SaveFileSceneData(sceneData.SceneInfo, restSnapshots);

                        notRestoredSceneDatas.Value.Add(notRestoredSceneData);
                    }
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }

            return notRestoredSceneDatas.HasValue ? notRestoredSceneDatas.Value : Array.Empty<SaveFileSceneData>();
        }

        public readonly bool Equals(SaveFileData other)
        {
            return Version == other.Version
                   &&
                   SceneDatas == other.SceneDatas;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is SaveFileData typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Version, SceneDatas);
        }

        public override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return $"Version \"{Version}\"";
        }
    }
}
