using System;

#nullable enable
namespace CCEnvs
{
    public static class LazyExtensions
    {
        public static bool TryGetValue<T>(
            this Lazy<T> source, 
            out T result
            )
        {
            CC.Guard.IsNotNullSource(source);

            if (source.IsValueCreated)
            {
                result = default!;
                return false;
            }

            result = source.Value;
            return true;
        }
    }
}
