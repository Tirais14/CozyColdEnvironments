using CCEnvs.Disposables;
using CCEnvs.Reflection;
using System;

#nullable enable
namespace CCEnvs.Saves
{
    public readonly struct SaveEntryUpgraderRegistration 
        :
        IEquatable<SaveEntryUpgraderRegistration>,
        IDisposable
    {
        private readonly LightDisposable<SaveEntryUpgradeVersionInfo> core;

        public SaveEntryUpgraderRegistration(SaveEntryUpgradeVersionInfo info)
        {
            core = new LightDisposable<SaveEntryUpgradeVersionInfo>(
                info,
                static info =>
                {
                    SaveEntryVersionHandler.UnregisterUpgrader(info);
                });
        }

        public static implicit operator LightDisposable<SaveEntryUpgradeVersionInfo>(SaveEntryUpgraderRegistration instance)
        {
            return instance.core;
        }

        public static bool operator ==(SaveEntryUpgraderRegistration left, SaveEntryUpgraderRegistration right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveEntryUpgraderRegistration left, SaveEntryUpgraderRegistration right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SaveEntryUpgraderRegistration registration && Equals(registration);
        }

        public readonly bool Equals(SaveEntryUpgraderRegistration other)
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
                return TypeofCache<SaveEntryUpgraderRegistration>.Type.Name;

            return $"({nameof(core)}: {core})";
        }

        public readonly void Dispose() => core.Dispose();
    }
}
