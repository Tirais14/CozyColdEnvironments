using System;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Saves
{
    public struct SaveEntryUpgradeVersionInfo : IEquatable<SaveEntryUpgradeVersionInfo>
    {
        private int? hash;

        public Type SnapshotType { get; }

        public long Version { get; }

        public long NextVersion { get; }

        public SaveEntryUpgradeVersionInfo(Type snapshotType, long version, long nextVersion)
            :
            this()
        {
            Guard.IsNotNull(snapshotType, nameof(snapshotType));

            SnapshotType = snapshotType;
            Version = version;
            NextVersion = nextVersion;
        }

        public static bool operator ==(SaveEntryUpgradeVersionInfo left, SaveEntryUpgradeVersionInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveEntryUpgradeVersionInfo left, SaveEntryUpgradeVersionInfo right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SaveEntryUpgradeVersionInfo info && Equals(info);
        }

        public readonly bool Equals(SaveEntryUpgradeVersionInfo other)
        {
            return hash == other.hash
                   &&
                   SnapshotType == other.SnapshotType
                   &&
                   Version == other.Version
                   &&
                   NextVersion == other.NextVersion;
        }

        public override int GetHashCode()
        {
            hash ??= HashCode.Combine(hash, SnapshotType, Version, NextVersion);

            return hash.Value;  
        }

        public readonly override string ToString()
        {
            if (this == default)
                return GetType().Name;

            return $"({nameof(SnapshotType)}: {SnapshotType}; {nameof(Version)}: {Version}; {nameof(NextVersion)}: {NextVersion})";
        }
    }
}
