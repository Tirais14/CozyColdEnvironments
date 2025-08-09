using System;

#nullable enable

namespace UTIRLib
{
    public static class StructExtensions
    {
        public static T ThrowIf<T>(this T obj, T value, Exception? exception = null)
            where T : struct, IEquatable<T>
        {
            if (obj.Equals(value))
                throw exception ?? new Exception($"Value cannot be {value}");

            return obj;
        }
    }
}