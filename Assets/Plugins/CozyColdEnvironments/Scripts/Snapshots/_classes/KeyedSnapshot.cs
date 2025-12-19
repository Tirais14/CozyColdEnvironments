
#nullable enable
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CCEnvs.Snapshots
{
    [Serializable]
    public readonly struct KeyedSnapshot : IEquatable<KeyedSnapshot>
    {
        public ISnapshot Snapshot { get; }
        public object? Key { get; }

        [JsonConstructor]
        /// <param name="key">Must be serializable by Json</param>
        public KeyedSnapshot(ISnapshot snapshot, object? key)
        {
            CC.Guard.IsNotNull(snapshot, nameof(snapshot));

            Snapshot = snapshot;
            Key = key;
        }

        public static bool operator ==(KeyedSnapshot left, KeyedSnapshot right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(KeyedSnapshot left, KeyedSnapshot right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is KeyedSnapshot snapshot && Equals(snapshot);
        }

        public bool Equals(KeyedSnapshot other)
        {
            return EqualityComparer<ISnapshot>.Default.Equals(Snapshot, other.Snapshot)
                   &&
                   Key == other.Key;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Snapshot, Key);
        }
    }
}
