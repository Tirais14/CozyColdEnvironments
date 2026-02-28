using CCEnvs.Serialization;
using System;

#nullable enable
namespace CCEnvs.Attributes.Serialization
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TypeSerializationDescriptorAttribute : Attribute, ICCAttribute
    {
        public string Name { get; }

        public string? ID { get; }

        public TypeSerializationDescriptorAttribute(string name, string? id = null)
        {
            Name = name;
            ID = id;
        }

        public TypeSerializationDescriptor ToDescriptor()
        {
            return new TypeSerializationDescriptor(Name, ID);
        }
    }
}
