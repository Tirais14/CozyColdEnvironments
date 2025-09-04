#nullable enable
using CCEnvs.Diagnostics;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Collections
{
    public static class CCEnumerable
    {
#pragma warning disable S1751
        public static bool IsEmpty(IEnumerable enumerable)
        {
            foreach (var _ in enumerable)
                return false;

            return true;
        }
#pragma warning restore S1751

        public static bool IsNotEmpty(IEnumerable enumerable)
        {
            return !IsEmpty(enumerable);
        }

        public static bool IsNullOrEmpty([NotNullWhen(false)] IEnumerable? enumerable)
        {
            return enumerable.IsNull() || IsEmpty(enumerable);
        }

        public static bool IsNotNullOrEmpty([NotNullWhen(true)] IEnumerable? enumerable)
        {
            return !IsNullOrEmpty(enumerable);
        }
    }
}
