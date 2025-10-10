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

        public static Reflected AsReflected(this object value, bool nonPublic)
        {
            CC.Guard.NullArgument(value, nameof(value));

            Reflected.Settings settings = Reflected.Settings.Default;

            if (nonPublic)
                settings |= Reflected.Settings.IncludeNonPublic;

            return new Reflected(value, settings);
        }

        public static Reflected AsReflected(this object value)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return new Reflected(value);
        }
    }
}
