using System;
using System.Linq;
using System.Reflection;
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Reflection;

#nullable enable

namespace CCEnvs.Utils
{
    public static class EnumHelper
    {
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="MemberNotFoundException"></exception>
        public static FieldInfo GetFieldInfo(Type enumType, string name)
        {
            if (!enumType.IsEnum && !enumType.IsType<Enum>())
                throw new ArgumentException($"{enumType.Name} is not enum.");

            FieldInfo field = enumType.GetField(name, BindingFlagsDefault.All) ??
                (FieldInfo)CC.Throw.MemberNotFound(name: name, memberType: MemberTypes.Field, reflectedType: enumType);

            return field;
        }
        public static FieldInfo GetFieldInfo<T>(string name)
            where T : Enum
        {
            if (typeof(T) == typeof(Enum))
                throw new ArgumentException("Type cannot be base Enum class.");

            return GetFieldInfo(typeof(T), name);
        }
        public static FieldInfo GetFieldInfo<T>(T value)
            where T : Enum
        {
            return GetFieldInfo(value.GetType(), value.ToString());
        }

        public static T[] GetValues<T>()
            where T : Enum
        {
            Type type = typeof(T);

            return (T[])Enum.GetValues(type);
        }

        public static bool IsFlags(Type type)
        {
            return type.IsEnum && type.IsDefined(typeof(FlagsAttribute));
        }

        public static string[] GetTypeNamesExcept(Type enumType, string fieldName = "None")
        {
            return Enum.GetNames(enumType).Except(fieldName).ToArray();
        }
    }
}