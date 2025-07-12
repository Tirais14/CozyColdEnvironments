using System;
using System.Reflection;
using System.Text;
using UTIRLib.Utils;

#nullable enable

namespace UTIRLib
{
    public static class TypeExtensions
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Is(this Type a, Type? b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                return false;

            if (a == b)
                return true;

            return a.IsAssignableFrom(b);
        }
        public static bool Is<T>(this Type a)
        {
            return a.Is(typeof(T));
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

        public static string GetProccessedName(this Type? type,
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

        public static T[] GetAllMembers<T>(this Type type,
                                           BindingFlags bindingFlags = BindingFlags.Default)
            where T : MemberInfo
        {
            return TypeHelper.GetAllMembers<T>(type, bindingFlags);
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
                return $"{type.GetProccessedName(TypeNameAttributes.Default | ~TypeNameAttributes.ShortName)}[]";

            throw new Exception($"Invalid type {type.Name}.");
        }
    }
}