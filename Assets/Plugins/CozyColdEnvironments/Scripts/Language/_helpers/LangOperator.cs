#nullable enable
using System.Runtime.CompilerServices;

#pragma warning disable S3236
#pragma warning disable IDE1006
namespace CCEnvs.FuncLanguage
{
    public static class LangOperator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<T, object> Left<T>(T value)
        {
            return new Either<T, object>(value, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<object, T> Right<T>(T value)
        {
            return new Either<object, T>(null, value);
        }

        public static Catched<T> Catch()
    }
}
