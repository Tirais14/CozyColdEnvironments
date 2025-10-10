#nullable enable
using CCEnvs.Reflection;
using System.Collections.Generic;

namespace CCEnvs.Collections.Unsafe
{
    public static class ListExtensions
    {
        public static string? InternalArrayFieldName { get; private set; }

        public static T[] GetInternalArray<T>(this List<T> source)
        {
            CC.Guard.NullArgument(source, nameof(source));

            Reflected rSource = source.AsReflected();

            if (InternalArrayFieldName.IsNullOrEmpty())
            {
                var field = rSource.Field<T[]>();

                InternalArrayFieldName = rSource.Field<T[]>().value.Name;

                return field.GetValueT<T[]>();
            }

            return rSource.Field<T[]>(InternalArrayFieldName).GetValue();
        }
    }
}
