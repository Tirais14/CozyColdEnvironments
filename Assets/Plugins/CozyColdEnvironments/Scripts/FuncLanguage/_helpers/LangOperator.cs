#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#pragma warning disable S3236
#pragma warning disable IDE1006
namespace CCEnvs.FuncLanguage
{
    public static class LangOperator
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSome<T>(T obj) => obj.IsNotNull();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSome<T>(T obj,
            T @default,
            IEqualityComparer<T>? comparer = null)
        {
            if (obj.IsNull())
                return false;

            comparer ??= EqualityComparer<T>.Default;

            return !comparer.Equals(obj, @default);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNone<T>(T obj) => !IsSome(obj);

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNone<T>(T obj,
            T @default,
            IEqualityComparer<T>? comparer = null)

            where T : struct
        {
            return !IsSome(obj, @default, comparer);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<T, object> Left<T>(T value)
        {
            return new Either<T, object>(value, null);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<object, T> Right<T>(T value)
        {
            return new Either<object, T>(null, value);
        }
    }
}
