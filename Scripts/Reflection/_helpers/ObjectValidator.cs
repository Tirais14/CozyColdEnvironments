using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CozyColdEnvironments.Diagnostics;

#nullable enable
namespace CozyColdEnvironments.Reflection
{
    public static class ObjectValidator
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static bool EqaulsDefaultByFields(object obj,
            IReadOnlyDictionary<Type, object[]>? customDefaultValuesCollection = null,
            EqualsDefaultOption isDefaultOption = EqualsDefaultOption.None)
        {
            if (obj.IsNull())
                throw new ArgumentNullException(nameof(obj));

            FieldInfo[] fields = obj.GetType().GetFields(BindingFlagsDefault.InstanceAll);
            if (fields.IsEmpty())
                throw new Exception($"{obj.GetType().GetName()} doesn't have fields.");

            foreach (FieldInfo field in fields)
            {
                if (customDefaultValuesCollection is not null
                    &&
                    customDefaultValuesCollection.TryGetValue(field.FieldType,
                        out object[] customDefaultValues)
                    &&
                    field.GetValue(obj).IsNotDefault(customDefaultValues,
                                                     isDefaultOption)
                    )
                    return false;
                else if (field.GetValue(obj).IsNotDefault(isDefaultOption))
                    return false;
            }

            return true;
        }

        public static bool EqaulsDefaultByFieldsAndItTypes(object? obj,
            IReadOnlyDictionary<Type, object[]>? customDefaultValuesCollection = null,
            EqualsDefaultOption isDefaultOption = EqualsDefaultOption.None)
        {
            if (obj.IsDefault(isDefaultOption))
                return true;

            object?[] allFieldValues = TypeReflectionHelper.GetFieldValuesIncludeNestedFields(obj,
                BindingFlagsDefault.InstanceAll);

            if (allFieldValues.IsEmpty())
                return false;

            foreach (var fieldValue in allFieldValues)
            {
                if (fieldValue.IsDefault(isDefaultOption))
                    continue;

                if (!fieldValue.GetType().IsPrimitiveType())
                    continue;

                if (IsCustomDefaultValue(fieldValue, customDefaultValuesCollection))
                    continue;

                return false;
            }

            return true;
        }

        private static bool IsCustomDefaultValue(object fieldValue,
            IReadOnlyDictionary<Type, object[]>? customDefaultValuesCollection)
        {
            if (customDefaultValuesCollection is null)
                return false;

            return customDefaultValuesCollection is not null
                   &&
                   customDefaultValuesCollection.TryGetValue(fieldValue.GetType(),
                   out object?[] customDefaultValues)
                   &&
                   customDefaultValues.Any(x => Equals(fieldValue, x));
        }
    }
}
