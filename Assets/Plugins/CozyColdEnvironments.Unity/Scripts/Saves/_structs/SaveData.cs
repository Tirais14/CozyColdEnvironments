using CCEnvs.Attributes.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [TypeSerializationDescriptor("SaveData", "{868DC038-8CB2-4C61-97DE-931D4D21212C}")]
    public struct SaveData : IEquatable<SaveData>
    {
        public IReadOnlyList<SaveUnit> SaveUnits { readonly get; private set; }

        public string? Version { readonly get; set; }

        public SaveData(IReadOnlyList<SaveUnit> saveUnits)
            :
            this()
        {
            SaveUnits = saveUnits;
        }

        public static bool operator ==(SaveData left, SaveData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveData left, SaveData right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SaveData data && Equals(data);
        }

        public readonly bool Equals(SaveData other)
        {
            return EqualityComparer<IReadOnlyList<SaveUnit>>.Default.Equals(SaveUnits, other.SaveUnits)
                   &&
                   Version == other.Version;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(SaveUnits, Version);
        }
    }
}
