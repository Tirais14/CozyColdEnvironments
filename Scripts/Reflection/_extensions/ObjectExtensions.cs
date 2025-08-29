using System;
using CozyColdEnvironments.Diagnostics;

#nullable enable
namespace CozyColdEnvironments.Reflection
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
    }
}
