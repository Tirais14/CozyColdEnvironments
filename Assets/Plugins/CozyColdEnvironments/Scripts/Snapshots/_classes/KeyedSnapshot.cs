
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CCEnvs.Snapshots
{
    [Serializable]
    public struct KeyedSnapshot<T> : IEquatable<KeyedSnapshot<T>>, ISnapshot
        where T : ISnapshot
    {
        public T Snapshot { readonly get; private set; }
        public object? Key { readonly get; private set; }

        [JsonIgnore]
        public readonly Type TargetType => Snapshot.TargetType;

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

        public readonly ISnapshot CaptureFrom(object obj)
        {
            Snapshot.CaptureFrom(obj);

            return this;
        }

        public readonly bool TryRestore(object? target, [NotNullWhen(true)] out object? restored)
        {
            return Snapshot.TryRestore(target, out restored);
        }

        public readonly bool TryRestore(object? target) => TryRestore(target, out _);

        public readonly ISnapshot TryRestoreQ(object? target)
        {
            TryRestore(target);
            return this;
        }

        public readonly bool CanRestore(object? target)
        {
            return Snapshot.CanRestore(target);
        }

        public readonly ISnapshot Reset()
        {
            Snapshot.Reset();
            return this;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is KeyedSnapshot<T> snapshot && Equals(snapshot);
        }

        public readonly bool Equals(KeyedSnapshot<T> other)
        {
            return EqualityComparer<ISnapshot>.Default.Equals(Snapshot, other.Snapshot)
                   &&
                   Key == other.Key;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Snapshot, Key);
        }
    }
}
