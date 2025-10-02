using System;

#nullable enable
namespace CCEnvs.Cacheables
{
    public static class TypeExtensions
    {
        public static bool IsCacheableType(this Type value)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return Cacheable.IsCacheable(value);
        }
    }
}
