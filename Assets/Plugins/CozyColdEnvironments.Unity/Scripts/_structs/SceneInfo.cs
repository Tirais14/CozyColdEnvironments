using System;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Unity.EditorSerialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity
{
    [Serializable]
    [SerializationDescriptor("SceneInfo", "2c534bb6-ba5c-4eae-a359-abc635b00b8b")]
    public struct SceneInfo : IEquatable<SceneInfo>
    {
        private int? hashCode;

        [JsonProperty("buildIndex")]
        [field: SerializeField]
        public SerializedNullable<int> BuildIndex { get; private set; }

        [JsonProperty("path")]
        [field: SerializeField]
        public string Path { get; private set; }

        [JsonProperty("name")]
        [field: SerializeField]
        public string Name { get; private set; }

        public SceneInfo(int? buildIndex, string path, string name)
        {
            hashCode = null;

            BuildIndex = new SerializedNullable<int>(buildIndex);
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
            if (Equals(other))
                return true;

            if (BuildIndex.Deserialized.HasValue || other.BuildIndex.Deserialized.HasValue)
                return BuildIndex == other.BuildIndex;

            if (Path.IsNotNullOrWhiteSpace() || other.Path.IsNotNullOrWhiteSpace())
                return Path == other.Path;

            if (Name.IsNotNullOrWhiteSpace() || other.Name.IsNotNullOrWhiteSpace())
                return Name == other.Name;

            return false;
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

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(BuildIndex, Path, Name);

            return hashCode.Value;
        }

        public readonly override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(BuildIndex)}: {BuildIndex}; {nameof(Name)}: {Name}; {nameof(Path)}: {Path})";
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
