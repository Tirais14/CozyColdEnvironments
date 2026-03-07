using System;
using CCEnvs.Attributes;

#nullable enable
namespace CCEnvs.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class PolymorphSerializableAttribute : Attribute, ICCAttribute
    {
    }
}
