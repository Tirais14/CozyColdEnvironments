using System;

#nullable enable
namespace CCEnvs.Cacheables
{
    public static class TypeExtensions
    {
        public static bool IsCacheableType(this Type value)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            return Cacheable.IsCacheable(value);
        }
    }
}
