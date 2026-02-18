using CCEnvs.Attributes;
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
    }
}
