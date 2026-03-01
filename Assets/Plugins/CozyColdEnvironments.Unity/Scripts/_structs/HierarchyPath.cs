using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;

#nullable enable
namespace CCEnvs.Unity
{
    [Serializable]
    [SerializationDescriptor("HierarchyPath", "77427b05-d52c-4cb1-8c64-4bcd8bead81f")]
    public struct HierarchyPath : IEquatable<HierarchyPath>
    {
        private int? hashCode;

        [JsonProperty("rawPath")]
        public readonly string RawPath { get; }

        [JsonProperty("position")]
        public readonly int Position { get; }

        [JsonIgnore]
        public readonly string Path { get; }

        [JsonConstructor]
        public HierarchyPath(string rawPath, int position)
        {
            hashCode = null;

            RawPath = rawPath;
            Position = position;

            Path = $"{rawPath} [{position}]";
        }

        public static bool operator ==(HierarchyPath left, HierarchyPath right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HierarchyPath left, HierarchyPath right)
        {
            return !(left == right);
        }

        public static HierarchyPath operator +(HierarchyPath left, string pathPart)
        {
            return left.Combine(pathPart);
        }

        public static HierarchyPath operator +(HierarchyPath left, string[] pathParts)
        {
            return left.Combine(pathParts);
        }

        public static HierarchyPath Parse(string path)
        {
            var regexCaptures = Regex.Match(path, @"(.*)\[(d+)\]$").Captures;

            string rawPath = regexCaptures.ElementAtOrDefault(1).Value
                ??
                throw new System.ArgumentException($"Invalid path: {path}");

            string rawPos = regexCaptures.ElementAtOrDefault(2).Value;

            int pos = 0;
            if (rawPos.IsNotNullOrWhiteSpace())
                pos = int.Parse(rawPos);

            return new HierarchyPath(rawPath, pos);
        }

        public readonly string[] Split()
        {
            return Regex.Split(RawPath, @"[/\\]");
        }

        public readonly HierarchyPath Combine(params string[] pathParts)
        {
            pathParts = pathParts.PrependToArray(RawPath.TrimEnd('/', '\\'));

            for (int i = 0; i < pathParts.Length; i++)
                pathParts[i] = pathParts[i].Trim('/', '\\');

            string combinedPath = System.IO.Path.Combine(pathParts);

            return new HierarchyPath(combinedPath, Position);
        }

        public readonly bool Equals(HierarchyPath other)
        {
            return RawPath == other.RawPath
                   &&
                   Position == other.Position
                   &&
                   Path == other.Path;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is HierarchyPath path && Equals(path);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(RawPath, Position, Path);

            return hashCode.Value;
        }

        public readonly override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return Path;
        }
    }
}
