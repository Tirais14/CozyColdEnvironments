using System;
using System.Reflection;

#nullable enable
namespace UTIRLib
{
    public static class MemberInfoExtensions
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
    }
}
