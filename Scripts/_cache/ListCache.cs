using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
#pragma warning disable S2743

namespace CCEnvs
{
    public static class ListCache<T>
    {
        private static FieldInfo? arrayField;

        public static T[] GetInternalArray(IList<T> target)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            arrayField ??= target.GetType()
                                 .GetFields(BindingFlagsDefault.InstanceNonPublic)
                                 .Single(x => x.FieldType.IsType<T[]>());

            return (T[])arrayField.GetValue(target);
        }
    }
}