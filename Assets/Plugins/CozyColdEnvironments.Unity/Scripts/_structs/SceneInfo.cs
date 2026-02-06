using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity
{
    [Serializable]
    public struct SceneInfo : IEquatable<SceneInfo>
    {
        [JsonProperty]
        [field: SerializeField]
        public int BuildIndex { get; private set; }

        [JsonProperty]
        [field: SerializeField]
        public string Path { get; private set; }

        [JsonProperty]
        [field: SerializeField]
        public string Name { get; private set; }

        [JsonConstructor]
        public SceneInfo(int buildIndex, string path, string name)
        {
            BuildIndex = buildIndex;
            Path = path;
            Name = name;
        }

        public SceneInfo(Scene scene)
            :
            this(scene.buildIndex, scene.path, scene.name)
        {
            if (!scene.IsValid())
                throw new InvalidOperationException("Scene is not valid");
        }

        public static bool operator ==(SceneInfo left, SceneInfo right)
        {
            return left.Equals(right); 
        }

        public static bool operator !=(SceneInfo left, SceneInfo right)
        {
            return !(left == right);
        }

        public readonly bool IsMatch(SceneInfo other)
        {
            if (BuildIndex > -1 && BuildIndex == other.BuildIndex)
                return true;

            return Name == other.Name && Path == other.Path;
        }

        public readonly bool Equals(SceneInfo other)
        {
            return BuildIndex == other.BuildIndex
                   &&
                   Path == other.Path
                   &&
                   Name == other.Name;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is SceneInfo typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(BuildIndex, Path, Name);
        }

        public readonly override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return $"Build index '{BuildIndex}'; name \"{Name}\"";
        }
    }

    public static class SceneInfoExtensions
    {
        public static SceneInfo GetSceneInfo(this Scene source)
        {
            return new SceneInfo(source);
        }
    }
}
