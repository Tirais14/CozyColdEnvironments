using CCEnvs.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                .Reflect()
                .WithBindingFlags(bindingFlags)
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
                collected = Loops.BreadthFirstSearch(targetFieldValues[i], (current) =>
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
            FieldInfo[] objFields = type.Reflect()
                .WithBindingFlags(bindingFlags)
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
                collected = Loops.BreadthFirstSearch(objFields[i], (current) =>
                {
                    FieldInfo[] tempFields = current.FieldType.Reflect()
                        .WithBindingFlags(bindingFlags)
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
