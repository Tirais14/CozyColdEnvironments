#nullable enable
using CCEnvs.Reflection;
using System.Collections.Generic;

namespace CCEnvs.Collections.Unsafe
{
    public static class ListExtensions
    {
        public static string? InternalArrayFieldName { get; private set; } = "items";

        public static T[] GetInternalArray<T>(this List<T> source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return source.ReflectQuery()
                .NonPublic()
                .Name(InternalArrayFieldName)
                .Field()
                .Lax()
                .IfNone(() => source.ReflectQuery()
                .NonPublic()
                .ExtraType<T[]>()
                .Field()
                .Strict()
                .GetValue(source)
                )
                .RightTarget.As<T[]>();
        }
    }
}
