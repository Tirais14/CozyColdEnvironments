using System;
using System.Linq;
using System.Reflection;
using System.Text;

#nullable enable

namespace UTIRLib.Reflection
{
    public static class TypeExtensions
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsType(this Type value, Type? other)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (other == null)
                return false;

            if (value == other)
                return true;

            return other.IsAssignableFrom(value);
        }
        public static bool IsType<T>(this Type value)
        {
            return value.IsType(typeof(T));
        }

        public static bool IsNotType(this Type value, Type other)
        {
            return !value.IsType(other);
        }
        public static bool IsNotType<T>(this Type value)
        {
            return value.IsNotType(typeof(T));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsAnyType(this Type a, params Type?[] types)
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

        public static bool IsNotAnyType(this Type value, params Type?[] types)
        {
            return !value.IsAnyType(types);
        }

        public static string GetName(this Type? type,
            TypeNameAttributes attributes = TypeNameAttributes.Default)
        {
            if (type == null) return "null";

            if (attributes.HasFlag(TypeNameAttributes.ShortName)
                &&
                TypeHelper.IsPrimitiveType(type)
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

        private static string ProccessGenericArguments(Type type)
        {
            Type[] argumentTypes = type.GetGenericArguments();

            StringBuilder sb = new();
            sb.Append('<');

            sb.AppendJoin(", ", argumentTypes.Select(x => x.GetName()));

            sb.Append('>');

            return type.Name[..^2] + sb.ToString();
        }

        private static string ToShortName(Type type)
        {
            if (type.IsType<byte>())
                return "byte";
            else if (type.IsType<sbyte>())
                return "sbyte";
            else if (type.IsType<short>())
                return "short";
            else if (type.IsType<ushort>())
                return "ushort";
            else if (type.IsType<int>())
                return "int";
            else if (type.IsType<uint>())
                return "uint";
            else if (type.IsType<long>())
                return "long";
            else if (type.IsType<ulong>())
                return "ulong";
            else if (type.IsType<string>())
                return "string";
            else if (type.IsType<bool>())
                return "bool";
            else if (type.IsType<Array>())
                return $"{type.GetName(TypeNameAttributes.Default | ~TypeNameAttributes.ShortName)}[]";

            throw new Exception($"Invalid type {type.Name}.");
        }
    }
}