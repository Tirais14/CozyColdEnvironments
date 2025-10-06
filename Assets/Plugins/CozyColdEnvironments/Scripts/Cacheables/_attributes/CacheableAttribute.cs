using CCEnvs.Attributes;
using System;

#nullable enable
namespace CCEnvs.Cacheables
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false, 
        Inherited = false)]
    public class CacheableAttribute : Attribute, ICCAttribute
    {
    }
}
