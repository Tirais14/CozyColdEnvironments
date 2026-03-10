using CCEnvs.Linq;
using CommunityToolkit.Diagnostics;
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

        public SaveCatalogSerialized(
            string path,
            IEnumerable<SaveGroupSerialized> groups
            )
        {
            Guard.IsNotNullOrWhiteSpace(path, nameof(path));
            CC.Guard.IsNotNull(groups, nameof(groups));

            Path = path;

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
