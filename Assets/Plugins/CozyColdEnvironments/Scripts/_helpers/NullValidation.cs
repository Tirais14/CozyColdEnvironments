using CCEnvs.Attributes;
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

        [OnInstallResetable]
        private static bool hasOverride;

        public static Func<object, bool>? Overrided {
            get => overrided;
        }

        public static bool HasOverride => hasOverride;

        public static void SetOverride(Func<object, bool>? overrideFund)
        {
            overrided = overrideFund;
            hasOverride = overrideFund is not null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull<T>([NotNullWhen(false)] this T? source)
        {
            if (source is null)
                return true;

            if (!hasOverride)
            {
                if (CC.IsMainThread(Thread.CurrentThread))
                    return source.Equals(null);

                return false;
            }

            return overrided!.Invoke(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull<T>([NotNullWhen(true)] this T? source)
        {
            return !source.IsNull();
        }
    }
}
