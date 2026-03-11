using CCEnvs.Linq;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Saves
{
    [Serializable]
    public readonly struct SaveCatalogSerialized : IEquatable<SaveCatalogSerialized>
    {
        private readonly Dictionary<string, SaveGroupSerialized> groups;

        public IReadOnlyDictionary<string, SaveGroupSerialized> Groups => groups;

        public string Path { get; }

        public object GroupsGate { get; }

        [JsonConstructor]
        public SaveCatalogSerialized(
            string path,
            IEnumerable<SaveGroupSerialized>? groups = null
            )
        {
            Guard.IsNotNullOrWhiteSpace(path, nameof(path));
            CC.Guard.IsNotNull(groups, nameof(groups));

            Path = path;
            GroupsGate = new object();

            groups ??= Array.Empty<SaveGroupSerialized>();

            var groupDic = new Dictionary<string, SaveGroupSerialized>();

            if (groups.TryGetNonEnumeratedCount(out var count))
                groupDic.EnsureCapacity(count);

            foreach (var group in groups)
                groupDic.Add(group.Name, group);

            this.groups = groupDic;
        }

        public static bool operator ==(SaveCatalogSerialized left, SaveCatalogSerialized right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveCatalogSerialized left, SaveCatalogSerialized right)
        {
            return !(left == right);
        }

        public void Add(SaveGroupSerialized group)
        {
            if (this == default)
                throw new InvalidOperationException($"{this} is not initialized");

            lock (GroupsGate)
                groups[group.Name] = group;
        }

        public bool Remove(string groupName)
        {
            if (this == default)
                throw new InvalidOperationException($"{this} is not initialized");

            Guard.IsNotNull(groupName, nameof(groupName));

            lock (GroupsGate)
                return groups.Remove(groupName);
        }

        public void Clear()
        {
            lock (GroupsGate)
                groups.Clear();
        }

        public override bool Equals(object? obj)
        {
            return obj is SaveCatalogSerialized serialized && Equals(serialized);
        }

        public bool Equals(SaveCatalogSerialized other)
        {
            return groups == other.groups
                   &&
                   Path == other.Path;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(groups, Path);
        }
    }
}
