using CCEnvs.Disposables;
using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Saves
{
    public struct SaveGroupRegistration
        :
        IEquatable<SaveGroupRegistration>,
        IDisposable
    {
        private readonly DisposableLight<(SaveGroup Group, string Key)> core;

        public SaveGroupRegistration(SaveGroup group, string key)
            :
            this()
        {
            Guard.IsNotNull(group, nameof(group));
            Guard.IsNotNull(key, nameof(key));

            core = CCDisposable.Light(
                (group, key),
                args =>
                {
                    args.group.UnregisterObject(args.key);
                });
        }

        public static implicit operator DisposableLight<(SaveGroup Group, string Key)>(SaveGroupRegistration instance)
        {
            return instance.core;
        }

        public static bool operator ==(SaveGroupRegistration left, SaveGroupRegistration right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveGroupRegistration left, SaveGroupRegistration right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SaveGroupRegistration registration && Equals(registration);
        }

        public readonly bool Equals(SaveGroupRegistration other)
        {
            return core.Equals(other.core);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(core);
        }
        public readonly override string ToString()
        {
            if (this == default)
                return GetType().Name;

            return $"({nameof(core)}: {core})";
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            core.Dispose();

            disposed = true;
        }
    }
}
