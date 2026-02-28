using CCEnvs.Collections;
using System;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class MemberHelper
    {
        public static bool IsDefined<T>(this MemberInfo value)
            where T : Attribute
        {
            return value.IsDefined(typeof(T));
        }
        public static bool IsDefined<T>(this MemberInfo value, bool inherit)
            where T : Attribute
        {
            return value.IsDefined(typeof(T), inherit);
        }

        public static bool IsDefinedAny(this MemberInfo value,
                                        params Type[] attributeTypes)
        {
            return attributeTypes.Any(x => value.IsDefined(x));
        }
        public static bool IsDefinedAny(this MemberInfo value,
                                        bool inherit,
                                        params Type[] attributeTypes)
        {
            return attributeTypes.Any(x => value.IsDefined(x, inherit));
        }

        public static ParameterModifier GetParameterModifiers(this ParameterInfo[] values)
        {
            if (values.IsEmpty())
                return default;

            var modifier = new ParameterModifier(values.Length);

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].ParameterType.IsByRef)
                    modifier[i] = true;
            }

            return modifier;
        }
    }
}
