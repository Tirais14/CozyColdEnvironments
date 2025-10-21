#nullable enable
namespace CCEnvs.Language
{
    public static class LiquidConverter
    {
        public static Ghost<T> AsGhost<T>(this T source) => source;
    }
}
