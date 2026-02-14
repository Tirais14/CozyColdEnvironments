using System;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class TypeofCache<T>
    {
        public static Type Type { get; } = typeof(T);
    }
}
