using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs
{
    public static class ArrayExtensions
    {
        public static IEnumerator<T> GetEnumeratorT<T>(this T[] values)
        {
            CC.Guard.IsNotNull(values, nameof(values));

            return ((IEnumerable<T>)values).GetEnumerator();
        }
    }
}
namespace CCEnvs.Diagnostics
{
    public static class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this T[] array) => array.Length == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty<T>(this T[] array) => !array.IsEmpty();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this T[]? array)
        {
            return array is null || array.IsEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] this T[]? array)
        {
            return array is not null && array.IsNotEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasNullElement<T>(this T[] array) => array.Any(x => x.IsNull());
    }
}
