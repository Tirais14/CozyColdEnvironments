#nullable enable
using CCEnvs.Reflection;
using System;
using System.Collections.Generic;

namespace CCEnvs.Collections.Unsafe
{
    public static class ListUnsafeHelper
    {
        public const string INTERNAL_ARRAY_FIELD_NAME = "_items";

        public static T[] GetInternalArrayUnsafe<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            var t = source.Reflect()
                .NonPublic()
                .Name(INTERNAL_ARRAY_FIELD_NAME)
                .GetFieldValue<T[]>()
                .GetValueUnsafe();

            return t;
        }

        /// <summary>
        /// For less memory allocation.
        /// Use this only if you are sure that the source list will not be used anywhere else.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ArraySegment<T> GetInternalArraySegment<T>(this List<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            return new ArraySegment<T>(source.GetInternalArrayUnsafe(), 0, source.Count);
        }
    }
}
