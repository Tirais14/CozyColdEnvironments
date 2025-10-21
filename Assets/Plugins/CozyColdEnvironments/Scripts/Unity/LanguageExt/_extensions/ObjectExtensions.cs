using LanguageExt;

#nullable enable
namespace CCEnvs.Unity.LanguageExt
{
    public static class ObjectExtensions
    {
        public static Some<T?> ToUnitySome<T>(this T? source)
        {
            return source.IsNull() ? new Some<T?>() : source;
        }

        public static Option<T> ToUnityOption<T>(this T source)
        {
            return source.IsNull() ? Option<T>.None : source;
        }
    }
}
