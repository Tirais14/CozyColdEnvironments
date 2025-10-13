using System;
using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class ObjectExtensions
    {
        public static bool IsType<T>(this T? value, Type other)
        {
            if (value.IsNull())
                return false;

            return value.GetType().IsType(other);
        }

        public static bool IsNotType<T>(this T? value, Type other)
        {
            return !value.IsType(other);
        }

        public static Reflected AsReflected(this object source, bool nonPublic)
        {
            CC.Guard.NullArgument(source, nameof(source));

            Reflected.Settings settings = Reflected.Settings.Default;

            if (nonPublic)
                settings |= Reflected.Settings.IncludeNonPublic;

            return new Reflected(source, settings);
        }

        public static Reflected AsReflectedNonCacheable(this object source)
        {
            CC.Guard.NullArgument(source, nameof(source));

            return new Reflected(source, Reflected.Settings.Default & ~Reflected.Settings.DisallowCaching);
        }

        public static Reflected AsReflected(this object source)
        {
            CC.Guard.NullArgument(source, nameof(source));

            return new Reflected(source);
        }
    }
}
