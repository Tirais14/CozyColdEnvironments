using CCEnvs.Snapshots;
using Newtonsoft.Json;
using System;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    public readonly struct SaveUnit : IEquatable<SaveUnit>
    {
        public ISnapshot Snapshot { get; }

        public string Key { get; }

        [JsonConstructor]
        public SaveUnit(ISnapshot snapshot, string key)
        {
            Snapshot = snapshot;
            Key = key;
        }

        public static bool operator ==(SaveUnit left, SaveUnit right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveUnit left, SaveUnit right)
        {
            return !(left == right);
        }

        public readonly bool Equals(SaveUnit other)
        {
            return Snapshot.Equals(other.Snapshot)
                   &&
                   Key == other.Key;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SaveUnit unit && Equals(unit);
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
