using CCEnvs.Attributes.Serialization;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using ObservableCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [TypeSerializationDescriptor("Saves.SaveCatalog", "f6d4d3d5-bfab-4d7a-89a8-2107c8b2d497")]
    public class SaveCatalog 
        : 
        IEquatable<SaveCatalog>,
        IEnumerable<SaveGroup>
    {
        [JsonProperty("groups")]
        private ObservableDictionary<string, SaveGroup> groups = new();

        private int? hashCode;

        [JsonIgnore]
        public IReadOnlyObservableDictionary<string, SaveGroup> Groups => groups;

        [JsonProperty("path")]
        public string Path { get; init; }

        [JsonProperty("archive")]
        public SaveArchive Archive { get; init; }

        public SaveCatalog(
            SaveArchive archive,
            string? path = null
            )
        {
            Guard.IsNotNull(archive, nameof(archive));

            Path = path ?? string.Empty;
            Archive = archive;
        }

        public static bool operator ==(SaveCatalog? left, SaveCatalog? right)
        {
            if (ReferenceEquals(left, right)) 
                return true;

            if (left is null || right is null)
                return false;

            return left.Path == right.Path
                   &&
                   left.Archive ==right.Archive;
        }

        public static bool operator !=(SaveCatalog? left, SaveCatalog? right)
        {
            return !(left == right);
        }

        public bool RemoveGroup(string groupName, out SaveGroup? removed)
        {
            Guard.IsNotNull(groupName, nameof(groupName));

            return groups.Remove(groupName, out removed);
        }

        public SaveGroup GetOrCreateGroup(string groupName)
        {
            Guard.IsNotNull(groupName, nameof(groupName));

            if (!groups.TryGetValue(groupName, out var group))
            {
                group = new SaveGroup(this, groupName);

                groups.Add(groupName, group);
            }

            return group;
        }

        public SaveCatalog Clear()
        {
            groups.Clear();

            return this;
        }

        public string GetFullPath()
        {
            using var sb = StringBuilderPool.Shared.Get();

            using var pathParts = PooledArray<string>.FromRange(
                Archive.Path,
                Path
                );

            sb.Value.AppendJoin('/', pathParts.Value);

            return sb.Value.ToString();
        }

        public bool Equals(SaveCatalog other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is SaveCatalog typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(Path, Archive);

            return hashCode.Value;
        }

        public override string ToString()
        {
            return $"({nameof(Path)}: {Path}; {nameof(Archive)}: {Archive})";
        }

        public IEnumerator<SaveGroup> GetEnumerator()
        {
            return groups.To<IDictionary<string, SaveGroup>>().Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
