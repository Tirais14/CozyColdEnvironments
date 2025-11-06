using System;
using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class ObjectExtensions
    {
        public static bool IsIntanceOfType<T>(this T? value,
            Type other,
            TypeMatchingSettings typeMatchingSettings = TypeMatchingSettings.Default)
        {
            if (value.IsNull())
                return false;

            return value.GetType().IsType(other, typeMatchingSettings);
        }

        public static bool IsNotInstanceOfType<T>(this T? value,
            Type other,
            TypeMatchingSettings typeMatchingSettings = TypeMatchingSettings.Default)
        {
            return !value.IsIntanceOfType(other, typeMatchingSettings);
        }
    }
}
