using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#nullable enable
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
                                 .GetFields(BindingFlagsDefault.InstanceNonPublic.ToBindingFlags())
                                 .Where(x => x.FieldType.Is<T[]>())
                                 .Single();

            return (T[])arrayField.GetValue(target);
        }
    }
}
