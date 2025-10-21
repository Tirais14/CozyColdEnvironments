#nullable enable
namespace CCEnvs.Language
{
    public static class LiquidConverter
    {
        public static Ghost<T> ToLiquid<T>(this T source) => source;
    }
}
