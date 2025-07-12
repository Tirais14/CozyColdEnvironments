using System;
using System.Reflection;
using UTIRLib.Attributes;
using UTIRLib.Collections;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.Utils
{
    public static class MemberValidator
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static void ValidateInstance(object instance)
        {
            if (instance.IsNull())
            {
                throw new ArgumentNullException(nameof(instance));
            }

            Type instanceType = instance.GetType();

            FieldInfo[] fields = instanceType.GetFields(BindingFlagsDefault.InstanceAll.ToBindingFlags());
            Validate(fields, instance);

            PropertyInfo[] properties = instanceType.GetProperties(BindingFlagsDefault.InstanceAll.ToBindingFlags());
            Validate(properties, instance);
        }

        private static bool HasRequiredAttribute(MemberInfo member)
        {
            return member.IsDefined(typeof(RequiredMemberAttribute));
        }

        private static void Validate(FieldInfo[] fields, object instance)
        {
            if (fields.IsEmpty()) return;

            int fieldsLength = fields.Length;
            object? fieldValue;
            FieldInfo field;
            for (int i = 0; i < fieldsLength; i++)
            {
                field = fields[i];

                if (HasRequiredAttribute(field))
                {
                    fieldValue = field.GetValue(instance);

                    TirLibDebug.Assert(fieldValue.IsNull(),
                        $"Field: {field.Name} in type: {instance.GetTypeName()} not setted but required.",
                        instance);
                }
            }
        }

        private static void Validate(PropertyInfo[] properties, object instance)
        {
            if (properties.IsEmpty()) return;

            int propertiesLength = properties.Length;
            object? propertyValue;
            PropertyInfo property;
            for (int i = 0; i < propertiesLength; i++)
            {
                property = properties[i];

                if (HasRequiredAttribute(property))
                {
                    propertyValue = property.GetValue(instance);

                    TirLibDebug.Assert(propertyValue.IsNull(),
                        $"Property: {property.Name} in type: {instance.GetTypeName()} not setted but required.",
                        instance);
                }
            }
        }
    }
}