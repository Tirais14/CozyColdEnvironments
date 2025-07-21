using System;
using System.Reflection;
using System.Text;
using UTIRLib.Reflection;

#nullable enable

namespace UTIRLib
{
    public static class TypeExtensions
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Is(this Type value, Type? other)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (other == null)
                return false;

            if (value == other)
                return true;

            return other.IsAssignableFrom(value);
        }
        public static bool Is<T>(this Type value)
        {
            return value.Is(typeof(T));
        }

        public static bool IsNot(this Type value, Type other)
        {
            return !value.Is(other);
        }
        public static bool IsNot<T>(this Type value)
        {
            return value.IsNot(typeof(T));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsAny(this Type a, params Type?[] types)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            for (int i = 0; i < types.Length; i++)
            {
                if (a.IsAssignableFrom(types[i]))
                    return true;
            }

            return false;
        }

        public static bool IsNotAny(this Type value, params Type?[] types)
        {
            return !value.IsAny(types);
        }

        public static string GetName(this Type? type,
            TypeNameAttributes attributes = TypeNameAttributes.Default)
        {
            if (type == null) return "null";

            if (attributes.HasFlag(TypeNameAttributes.ShortName)
                &&
                TypeHelper.HasSpecialName(type)
                )
                return ToShortName(type);

            if (type.IsGenericType)
            {
                if (attributes.HasFlag(TypeNameAttributes.IncludeGenericArguments))
                    return ProccessGenericArguments(type);
                else
                    return type.Name[..^2];
            }
            else return type.Name;
        }

        private static string ProccessGenericArguments(Type type)
        {
            Type[] argumentTypes = type.GetGenericArguments();

            StringBuilder sb = new();
            sb.Append('<');

            for (int i = 0; i < argumentTypes.Length; i++)
                sb.AppendJoin(", ", argumentTypes[i].Name);

            sb.Append('>');

            return type.Name[..^2] + sb.ToString();
        }

        private static string ToShortName(Type type)
        {
            if (type.Is<byte>())
                return "byte";
            else if (type.Is<sbyte>())
                return "sbyte";
            else if (type.Is<short>())
                return "short";
            else if (type.Is<ushort>())
                return "ushort";
            else if (type.Is<int>())
                return "int";
            else if (type.Is<uint>())
                return "uint";
            else if (type.Is<long>())
                return "long";
            else if (type.Is<ulong>())
                return "ulong";
            else if (type.Is<string>())
                return "string";
            else if (type.Is<bool>())
                return "bool";
            else if (type.Is<Array>())
                return $"{type.GetName(TypeNameAttributes.Default | ~TypeNameAttributes.ShortName)}[]";

            throw new Exception($"Invalid type {type.Name}.");
        }
    }
}
namespace UTIRLib.Reflection.Types
{
    public static class TypeExtensions
    {
        public static MemberInfo[] ForceGetMembers(this Type value,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            return TypeHelper.ForceGetMembers(value, bindingFlags);
        }

        public static FieldInfo[] ForceGetFields(this Type value,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            return TypeHelper.ForceGetMembers<FieldInfo>(value, bindingFlags);
        }

        public static PropertyInfo[] ForceGetProperties(this Type value,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            return TypeHelper.ForceGetMembers<PropertyInfo>(value, bindingFlags);
        }

        public static MethodInfo[] ForceGetMethods(this Type value,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            return TypeHelper.ForceGetMembers<MethodInfo>(value, bindingFlags);
        }

        public static ConstructorInfo[] ForceGetConstructors(this Type value,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            return TypeHelper.ForceGetMembers<ConstructorInfo>(value, bindingFlags);
        }
    }
}