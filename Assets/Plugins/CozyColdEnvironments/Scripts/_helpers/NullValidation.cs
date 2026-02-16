using CCEnvs.Attributes;
using CCEnvs.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable
namespace CCEnvs
{
    public static class NullValidation
    {
        [OnInstallResetable]
        private static Func<object, bool>? overrided;

        public static Func<object, bool>? Overrided {
            get => overrided;
        }

        public static bool HasOverride => overrided is not null;

        public static void SetOverride(Func<object, bool>? overrideFunc)
        {
            overrided = overrideFunc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>([NotNullWhen(false)] this T? source)
        {
            if (source is null)
                return true;

            if (TypeofCache<T>.Type.IsValueType)
                return false;

            if (overrided is null)
            {
                if (CC.IsMainThread(Thread.CurrentThread))
                    return source.Equals(null);

                return false;
            }

            return overrided!.Invoke(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>(
            [NotNullWhen(false)] this T? source,
            [NotNullWhen(false)] out T? result
            )
        {
            result = source;

            return result.IsNull();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull<T>([NotNullWhen(true)] this T? source)
        {
            return !source.IsNull();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull<T>(
            [NotNullWhen(true)] this T? source,
            [NotNullWhen(true)] out T? result
            )
        {
            result = source;

            return result.IsNotNull();
        }
    }
}
