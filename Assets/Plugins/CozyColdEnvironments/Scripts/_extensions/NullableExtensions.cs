#nullable enable
namespace CCEnvs
{
    public static class NullableExtensions
    {
        public static bool TryGetValue<TValue>(this TValue? source, out TValue result)
            where TValue : struct
        {
            if (!source.HasValue)
            {
                result = default;
                return false;
            }

            result = source.Value;
            return true;
        }

        public static TValue GetValueOrDefault<TValue>(this TValue? source, TValue @default)
            where TValue : struct
        {
            if (!source.HasValue)
                return @default;

            return source.Value;
        }
    }
}
