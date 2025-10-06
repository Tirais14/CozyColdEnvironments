#nullable enable
using CCEnvs.Attributes;
using System;

namespace CCEnvs.Cacheables
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CacheAccessorAttribute : Attribute, ICCAttribute
    {
    }
}
