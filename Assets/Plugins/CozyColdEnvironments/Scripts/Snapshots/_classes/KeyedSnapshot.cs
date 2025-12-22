
#nullable enable
using CCEnvs.FuncLanguage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CCEnvs.Snapshots
{
    [Serializable]
    public struct KeyedSnapshot<T> : IEquatable<KeyedSnapshot<T>>, ISnapshot
        where T : ISnapshot
    {
        public T Snapshot { readonly get; private set; }
        public object? Key { readonly get; private set; }

        [JsonIgnore]
        public readonly Maybe<object> Target {
            get => Snapshot.Target;
            set => Snapshot.Target = value; 
        }

        [JsonIgnore]
        public readonly Type TargetType => Snapshot.TargetType;

        public readonly bool CanRestoreWithoutTarget => Snapshot.CanRestoreWithoutTarget;
        public readonly bool IgnoreTarget => Snapshot.IgnoreTarget;

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

        public readonly Maybe<object> Restore() => Snapshot.Restore();
        public readonly Maybe<object> Restore(object? target) => Snapshot.Restore(target);

        public readonly bool CanRestore() => Snapshot.CanRestore();

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
