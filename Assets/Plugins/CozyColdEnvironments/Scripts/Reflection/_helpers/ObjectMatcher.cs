using System;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class ObjectMatcher
    {
        public static bool IsInstanceOfType<T>(this T? value,
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
            return !value.IsInstanceOfType(other, typeMatchingSettings);
        }
    }
}
