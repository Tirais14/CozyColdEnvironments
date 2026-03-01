using CCEnvs.Attributes;
using System;

#nullable enable
namespace CCEnvs.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class PolymorphSerializableAttribute : Attribute, ICCAttribute
    {
    }
}
