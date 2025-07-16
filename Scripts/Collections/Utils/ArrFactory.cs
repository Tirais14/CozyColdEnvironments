using System.Collections.Generic;

#nullable enable
namespace UTIRLib.Collections
{
    public static class ArrFactory
    {
        public static ArrayS<T> Create<T>(IEnumerable<T>? values)
        {
            return new ArrayS<T>(values);
        }

        public static ArrayS<T> Create<T>(params T[]? values)
        {
            return new ArrayS<T>(values);
        }

        public static ArrayS<T> Create<T>(T[] a, T[] b)
        {
            return new ArrayS<T>(a, b);
        }

        public static ArrayS<T> Create<T>(T[] a, T item)
        {
            return new ArrayS<T>(a, item);
        }

        public static ArrayS<T> Create<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            return new ArrayS<T>(a, b);
        }

        public static ArrayS<T> Create<T>(int length, int copyStartIndex, params T[] values)
        {
            return new ArrayS<T>(length, copyStartIndex, values);
        }

        public static ArrayS<T> Create<T>(int length, params T[] values)
        {
            return new ArrayS<T>(length, values);
        }
    }
}
