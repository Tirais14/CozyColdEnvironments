using CCEnvs.Attributes.Serialization;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using ObservableCollections;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [TypeSerializationDescriptor("Saves.SaveCatalog", "f6d4d3d5-bfab-4d7a-89a8-2107c8b2d497")]
    public class SaveCatalog : IEquatable<SaveCatalog>, IEnumerable<KeyValuePair<string, SaveGroup>>
    {
        private readonly ObservableDictionary<string, SaveGroup> groups = new();

        private int? hashCode;

        [JsonIgnore]
        public IReadOnlyObservableDictionary<string, SaveGroup> Groups => groups;

        [JsonProperty("path")]
        public string Path { get; }

        [JsonProperty("archive")]
        public SaveArchive Archive { get; }

        [JsonConstructor]
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

        public bool ContainsSaveGroup(string groupName)
        {
            Guard.IsNotNull(groupName, nameof(groupName));

            return groups.ContainsKey(groupName);
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

        public IEnumerator<KeyValuePair<string, SaveGroup>> GetEnumerator()
        {
            return groups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
