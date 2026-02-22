using CCEnvs.Attributes.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [TypeSerializationDescriptor("Saves.SSaveGroupDataPairaveGroupDataPair")]
    public struct SaveGroupDataPair : IEquatable<SaveGroupDataPair>
    {
        [JsonProperty("group")]
        public SaveGroup Group { get; private set; }

        [JsonProperty("data")]
        public SaveData Data { get; private set; }

        public static bool operator ==(SaveGroupDataPair left, SaveGroupDataPair right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveGroupDataPair left, SaveGroupDataPair right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is SaveGroupDataPair pair && Equals(pair);
        }

        public readonly bool Equals(SaveGroupDataPair other)
        {
            return SaveGroup.NameCatalogEqualityComparer.Instance()
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Group, Data);
        }
    }
}
