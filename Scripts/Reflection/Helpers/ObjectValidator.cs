using Codice.Client.BaseCommands.BranchExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class ObjectValidator
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static bool IsDefaultByFields(object obj,
            IReadOnlyDictionary<Type, object[]>? customDefaultValuesCollection = null,
            IsDefaultOption isDefaultOption = IsDefaultOption.None)
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

        public static bool IsDefaultByTypeFieldsAndFieldsValues(object obj,
            IReadOnlyDictionary<Type, object[]>? customDefaultValuesCollection = null,
            IsDefaultOption isDefaultOption = IsDefaultOption.None)
        {
            if (obj.IsNull())
                throw new ArgumentNullException(nameof(obj));

            object?[] allFieldValues = TypeHelper.GetFieldValuesByTypeAndFieldValues(obj,
                BindingFlagsDefault.InstanceAll);

            if (allFieldValues.IsEmpty())
                throw new Exception("Cannot find any field value.");

            foreach (var fieldValue in allFieldValues)
            {
                if (fieldValue.IsNotDefault(isDefaultOption))
                {
                    if (IsCustomDefaultValue(fieldValue, customDefaultValuesCollection))
                        continue;

                    return false;
                }
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
