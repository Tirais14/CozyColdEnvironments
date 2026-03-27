using System;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;

#nullable enable
namespace CCEnvs.Serialization
{
    [Serializable]
    public readonly struct TypeSerializationDescriptor : IEquatable<TypeSerializationDescriptor>
    {
        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("id")]
        public string? ID { get; init; }

        public TypeSerializationDescriptor(string name, string? id = null)
        {
            Guard.IsNotNull(name, nameof(name));

            Name = name;
            ID = id;
        }

        public static implicit operator TypeSerializationDescriptor(string name)
        {
            return new TypeSerializationDescriptor(name);
        }

        public static implicit operator TypeSerializationDescriptor((string Name, string? ID) item)
        {
            return new TypeSerializationDescriptor(item.Name, item.ID);
        }

        public static bool operator ==(TypeSerializationDescriptor left, TypeSerializationDescriptor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TypeSerializationDescriptor left, TypeSerializationDescriptor right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is TypeSerializationDescriptor descriptor && Equals(descriptor);
        }

        public bool Equals(TypeSerializationDescriptor other)
        {
            return Name == other.Name &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, ID);
        }

        public override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Name)}: {Name}; {nameof(ID)}: {ID})";
        }
    }
}
