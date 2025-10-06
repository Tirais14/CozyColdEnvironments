#nullable enable
using CCEnvs.Utils;
using System;
using System.Reflection;

namespace CCEnvs.Reflection
{
    public static class EnumExtensions
    {
        public static FieldInfo GetFieldInfo(this Enum value)
        {
            return EnumHelper.GetFieldInfo(value);
        }
        public static FieldInfo GetFieldInfo<T>(this T enumValue)
            where T : Enum
        {
            return EnumHelper.GetFieldInfo(enumValue);
        }
    }
}
