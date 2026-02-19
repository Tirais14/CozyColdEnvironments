using CCEnvs.Attributes.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [TypeSerializationDescriptor("Saves.SaveData", "{868DC038-8CB2-4C61-97DE-931D4D21212C}")]
    public readonly struct SaveData : IEquatable<SaveData>
    {
        public IReadOnlyList<SaveUnit> SaveUnits { get; }

        [JsonConstructor]
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
            return EqualityComparer<IReadOnlyList<SaveUnit>>.Default.Equals(SaveUnits, other.SaveUnits);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(SaveUnits);
        }
    }
}
