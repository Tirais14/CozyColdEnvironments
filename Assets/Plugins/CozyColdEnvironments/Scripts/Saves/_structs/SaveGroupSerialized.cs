using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Saves
{
    [Serializable]
    public readonly struct SaveGroupSerialized : IEquatable<SaveGroupSerialized>
    {
        public string Name { get; }

        public string SaveDataSerialized { get; }

        public SaveGroupSerialized(
            string name,
            string saveDataSerialized
            )
        {
            Guard.IsNotNull(name, nameof(name));
            Guard.IsNotNull(saveDataSerialized, nameof(saveDataSerialized));

            Name = name;
            SaveDataSerialized = saveDataSerialized;
        }

        public static bool operator ==(SaveGroupSerialized left, SaveGroupSerialized right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveGroupSerialized left, SaveGroupSerialized right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is SaveGroupSerialized serialized && Equals(serialized);
        }

        public bool Equals(SaveGroupSerialized other)
        {
            return Name == other.Name
                   &&
                   SaveDataSerialized == other.SaveDataSerialized;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, SaveDataSerialized);
        }
    }
}
