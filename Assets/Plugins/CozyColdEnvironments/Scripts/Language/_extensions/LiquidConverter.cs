#nullable enable
namespace CCEnvs.Language
{
    public static class LiquidConverter
    {
        public static Liquid<T> ToLiquid<T>(this T source) => source;
    }
}
