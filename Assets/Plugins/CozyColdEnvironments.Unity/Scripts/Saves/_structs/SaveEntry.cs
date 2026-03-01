using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [SerializationDescriptor("Saves.SaveUnit", "77478f05-ab2b-4cb0-b420-39baf8fd8452")]
    public readonly struct SaveEntry : IEquatable<SaveEntry>
    {
        public string Key { get; }

        public ISnapshot Snapshot { get; }

        [JsonConstructor]
        public SaveEntry(string key, ISnapshot snapshot)
        {
            Guard.IsNotNull(key, nameof(key));
            CC.Guard.IsNotNull(snapshot, nameof(snapshot));

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
