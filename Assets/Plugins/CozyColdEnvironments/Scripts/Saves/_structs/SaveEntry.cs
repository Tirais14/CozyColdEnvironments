using System;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;

#nullable enable
namespace CCEnvs.Saves
{
    [Serializable]
    [SerializationDescriptor("Saves.SaveUnit", "77478f05-ab2b-4cb0-b420-39baf8fd8452")]
    public readonly struct SaveEntry : IEquatable<SaveEntry>
    {
        [JsonProperty("version")]
        public long Version { get; init; }

        [JsonProperty("key")]
        public string Key { get; init; }

        [JsonProperty("snapshot")]
        public ISnapshot Snapshot { get; init; }

        public SaveEntry(long version, string key, ISnapshot snapshot)
        {
            Guard.IsNotNull(key, nameof(key));
            CC.Guard.IsNotNull(snapshot, nameof(snapshot));

            Version = version;
            Key = key;
            Snapshot = snapshot;
        }

        public void Deconstruct(out string key, out ISnapshot snapshot)
        {
            key = Key;
            snapshot = Snapshot;
        }

        public static bool operator ==(SaveEntry left, SaveEntry right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveEntry left, SaveEntry right)
        {
            return !(left == right);
        }

        public readonly bool Equals(SaveEntry other)
        {
            return Snapshot.Equals(other.Snapshot)
                   &&
                   Key == other.Key;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SaveEntry unit && Equals(unit);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Snapshot, Key);
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Snapshot)}: {Snapshot}; {nameof(Key)}: {Key})";
        }
    }
}
