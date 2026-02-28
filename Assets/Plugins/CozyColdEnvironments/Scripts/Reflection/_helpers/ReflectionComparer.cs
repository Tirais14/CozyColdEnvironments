using CCEnvs.Collections;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class ReflectionComparer
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static bool IsDefault(object obj)
        {
            if (obj.IsNull())
                throw new ArgumentNullException(nameof(obj));

            FieldInfo[] fields = obj.GetType().GetFields(BindingFlagsDefault.InstanceAll);

            if (fields.IsEmpty())
                throw new InvalidOperationException($"{obj.GetType().GetName()} doesn't have fields.");

            foreach (FieldInfo field in fields)
            {
                if (field.GetValue(obj).IsNotDefault())
                    return false;
            }

            return true;
        }
    }
}
