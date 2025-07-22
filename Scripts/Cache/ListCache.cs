using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
#pragma warning disable S2743
namespace UTIRLib
{
    public static class ListCache<T>
    {
        private static FieldInfo? arrayField;

        public static T[] GetInternalArray(List<T> target)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            arrayField ??= target.GetType()
                                 .GetFields(BindingFlagsDefault.InstanceNonPublic)
                                 .Single(x => x.FieldType.Is<T[]>());

            return (T[])arrayField.GetValue(target);
        }
    }
}
