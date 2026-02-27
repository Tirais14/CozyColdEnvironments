using System.Collections.Generic;
using System.Runtime.CompilerServices;


#nullable enable

namespace CCEnvs.Collections
{
    public static class EnumeratorHelper
    {
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> value)
        {
            if (value.IsNull())
                throw new System.ArgumentNullException(nameof(value));

            return new EnumeratorEnumerable<T>(value);
        }

#if ZLINQ_PLUGIN
        public static IEnumerable<T> AsEnumerable<TEnumerator, T>(
            this ZLinq.ValueEnumerable<TEnumerator, T> source)
            where TEnumerator : struct, ZLinq.IValueEnumerator<T>
        {
            var enumerator = source.Enumerator;
            while (enumerator.TryGetNext(out T item))
                yield return item;

            enumerator.Dispose();
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryMoveNext<T>(this IEnumerator<T> source, out T current)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.MoveNext())
            {
                current = source.Current;
                return true;
            }

            current = default!;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryMoveNextStruct<TEnumerator, T>(this ref TEnumerator source, out T current)
            where TEnumerator : struct, IEnumerator<T>
        {
            CC.Guard.IsNotNullSource(source);

            if (source.MoveNext())
            {
                current = source.Current;
                return true;
            }

            current = default!;
            return false;
        }
    }
}