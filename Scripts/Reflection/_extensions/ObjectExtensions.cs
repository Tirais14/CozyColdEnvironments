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

        public static Reflected AsReflected(this object value)
        {
            CC.Validate.ArgumentNull(value, nameof(value));

            return new Reflected(value);
        }
    }
}
