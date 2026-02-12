using System;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class CachedTypeof<T>
    {
        public static Type Type { get; } = typeof(T);
    }
}
