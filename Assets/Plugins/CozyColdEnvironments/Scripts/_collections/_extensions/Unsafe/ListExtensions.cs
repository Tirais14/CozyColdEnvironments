#nullable enable
using CCEnvs.Reflection;
using System.Collections.Generic;

namespace CCEnvs.Collections.Unsafe
{
    public static class ListExtensions
    {
        public const string INTERNAL_ARRAY_FIELD_NAME = "_items";

        public static T[] GetInternalArrayUnsafe<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            var t = source.Reflect()
                .NonPublic()
                .Name(INTERNAL_ARRAY_FIELD_NAME)
                .GetFieldValue<T[]>()
                .AccessUnsafe();

            return t;
        }
    }
}
