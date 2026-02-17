using CCEnvs.Attributes;
using System;

#nullable enable
namespace CCEnvs.Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TypeDescriptorAttribute : Attribute, ICCAttribute
    {
        public string Name { get; }
        public string? ID { get; }

        public TypeDescriptorAttribute(string name, string? id = null)
        {
            Name = name;
            ID = id;
        }
    }
}
