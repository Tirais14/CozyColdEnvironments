
#nullable enable
using CCEnvs.FuncLanguage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CCEnvs.Snapshots
{
    [Serializable]
    public readonly struct KeyedSnapshot<T> : IEquatable<KeyedSnapshot<T>>, ISnapshot
        where T : ISnapshot
    {
        public T Snapshot { get; }
        public object? Key { get; }

        [JsonIgnore]
        public Maybe<object> Target => Snapshot.Target;

        [JsonConstructor]
        public KeyedSnapshot(T snapshot, object? key)
        {
            CC.Guard.IsNotNull(snapshot, nameof(snapshot));

            Snapshot = snapshot;
            Key = key;
        }

        public static bool operator ==(KeyedSnapshot<T> left, KeyedSnapshot<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(KeyedSnapshot<T> left, KeyedSnapshot<T> right)
        {
            return !(left == right);
        }

        public object Restore() => Snapshot.Restore();
        public object Restore(object target) => Snapshot.Restore(target);

        public override bool Equals(object? obj)
        {
            return obj is KeyedSnapshot<T> snapshot && Equals(snapshot);
        }

        public bool Equals(KeyedSnapshot<T> other)
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
