using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Cacheables
{
    public static class ObjectExtensions
    {
        public static bool IsCacheable<T>(this T value)
        {
            return Cacheable.IsCacheable(value);
        }
    }
}
