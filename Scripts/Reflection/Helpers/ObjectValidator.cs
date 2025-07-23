using System;
using System.Collections.Generic;
using System.Reflection;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class ObjectValidator
    {
        public static bool IsDefaultByFields<T>(T obj,
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

        //public static bool IsFieldValueDefault(string fieldName,
        //                                     object obj,
        //                                     object? value,
        //                                     BindingFlags bindingFlags)
        //{
        //    if (fieldName.IsNullOrWhiteSpace())
        //        throw new StringArgumentException(nameof(fieldName), fieldName);
        //    if (obj.IsNull())
        //        throw new ArgumentNullException(nameof(obj));

        //    FieldInfo? field = obj.GetType().GetField(fieldName, bindingFlags);

        //    if (field is null)
        //        return false;

        //    object fieldValue = field.GetValue(obj);

        //    return Equals(fieldValue, value);
        //}
    }
}
