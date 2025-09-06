using System;

#nullable enable
namespace CCEnvs.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false, 
        Inherited = false)]
    public class CacheableAttribute : Attribute, ICCAttribute
    {
    }
}
