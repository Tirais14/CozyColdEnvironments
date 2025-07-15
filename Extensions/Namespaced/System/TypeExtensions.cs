using System;

#nullable enable
namespace UTIRLib.Attributes
{
    public static class TypeExtensions
    {
        public static bool IsDefined<T>(this Type value, bool inherit)
            where T : Attribute
        {
            return value.IsDefined(typeof(T), inherit);
        }
    }
}
