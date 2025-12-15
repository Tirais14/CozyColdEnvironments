using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity
{
    [Serializable]
    public readonly struct SceneInfo : IEquatable<SceneInfo>
    {
        [JsonInclude]
		[JsonPropertyName("buildIndex")]
        public int BuildIndex { get; }

        [JsonInclude]
		[JsonPropertyName("path")]
        public string Path { get; }

        [JsonInclude]
		[JsonPropertyName("name")]
        public string Name { get; }

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

        public bool Equals(SceneInfo other)
        {
            return BuildIndex == other.BuildIndex
                   &&
                   Path == other.Path
                   &&
                   Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is SceneInfo typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BuildIndex, Path, Name);
        }
    }
}
