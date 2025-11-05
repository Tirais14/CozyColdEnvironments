using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class TypeReflectionHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static object?[] GetFieldValues(object target,
            BindingFlags bindingFlags = BindingFlagsDefault.InstanceAll)
        {
            if (target.IsNull())
                throw new ArgumentNullException(nameof(target));

            return target.GetType()
                .ReflectQuery()
                .NonPublic()
                .IncludeBaseTypes()
                .Fields()
                .Select(x => x.GetValue(target))
                .ToArray();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static object?[] GetFieldValuesIncludeNestedFields(
            object target,
            BindingFlags bindingFlags = BindingFlagsDefault.InstanceAll)
        {
            object?[] targetFieldValues = GetFieldValues(target, bindingFlags);

            if (targetFieldValues.IsEmpty())
                return Array.Empty<object>();

            var results = new List<object?>();
            results.AddRange(targetFieldValues);

            Queue<object?> collected;

            int targetFieldCount = targetFieldValues.Length;
            for (int i = 0; i < targetFieldCount; i++)
            {
                collected = Do.Collect(targetFieldValues[i], (current) =>
                {
                    if (current is null)
                        return Array.Empty<object?>();

                    object?[] fieldValues = GetFieldValues(current,
                                                                                 bindingFlags);

                    return fieldValues;
                });

                results.AddRange(collected);
            }

            return results.ToArray();
        }

        public static FieldInfo[] GetFieldValuesIncludeNestedFieldTypes(Type type,
            BindingFlags bindingFlags = BindingFlagsDefault.InstanceAll)
        {
            FieldInfo[] objFields = type.ReflectQuery()
                .NonPublic()
                .IncludeBaseTypes()
                .Fields()
                .ToArray();

            if (objFields.IsEmpty())
                return Array.Empty<FieldInfo>();

            int objFieldCount = objFields.Length;
            var results = new List<FieldInfo>();
            Queue<FieldInfo> collected;

            for (int i = 0; i < objFieldCount; i++)
            {
                collected = Do.Collect(objFields[i], (current) =>
                {
                    FieldInfo[] tempFields = current.FieldType.ReflectQuery()
                        .NonPublic()
                        .IncludeBaseTypes()
                        .Fields()
                        .ToArray();

                    return tempFields;
                });

                results.AddRange(collected);
            }

            return results.ToArray();
        }
    }
}
