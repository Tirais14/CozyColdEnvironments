using CCEnvs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class TypeExtensions
    {
        private static Dictionary<Type, int> baseTypeCountCache = new(0);

        private readonly static HashSet<Type> basicTypes = new()
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(object),
            typeof(string),
            typeof(decimal),
            typeof(float),
            typeof(double),
            typeof(char),
            typeof(Array)
        };

        public static Type[] GetPrimitiveTypes()
        {
            return basicTypes.ToArray();
        }

        public static int GetParentsCount(this Type value, bool trimCache = false)
        {
            CC.Guard.NullArgument(value, nameof(value));

            if (baseTypeCountCache.TryGetValue(value, out int count))
                return count;

            Type[] baseTypes = TypeHelper.CollectBaseTypes(value).ToArray();

            for ( int i = 0; i < baseTypes.Length; i++)
                baseTypeCountCache.TryAdd(baseTypes[i], baseTypes.Length - 1 - i);

            if (trimCache)
                baseTypeCountCache.TrimExcess();

            return count;
        }

        public static Reflected AsReflected(this Type value)
        {
            CC.Guard.NullArgument(value, nameof(value));

            return new Reflected(value);
        }

        public static string GetTypeName<T>(this T? obj,
            TypeNameConvertingAttributes attributes = TypeNameConvertingAttributes.Default)
        {
            if (obj is null)
                return "null";

            return obj.GetType().GetName(attributes);
        }

        public static object? GetDefaultValue(this Type value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (value.IsClass)
                return null;

            var result = Activator.CreateInstance(value);

            TypeCache.TryCacheDefaultValue(value, result);

            return result;
        }

        /// <summary>
        /// Extends default method and now includes types <see cref="string"/>, <see cref="decimal"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBasicType(this Type? type)
        {
            if (type == null)
                return false;
            if (type == typeof(string)
                ||
                type == typeof(decimal)
                ||
                type == typeof(string)
                )
                return true;

            return type.IsPrimitive;
        }

        public static bool IsPrimitiveNumber(this Type value)
        {
            return value.IsPrimitive
                   &&
                   value == typeof(byte)
                   ||
                   value == typeof(sbyte)
                   ||
                   value == typeof(short)
                   ||
                   value == typeof(ushort)
                   ||
                   value == typeof(int)
                   ||
                   value == typeof(uint)
                   ||
                   value == typeof(long)
                   ||
                   value == typeof(ulong);
        }

        public static bool IsTypeBySemantics(this Type left, Type right)
        {
            CC.Guard.NullArgument(left, nameof(left));
            CC.Guard.NullArgument(right, nameof(right));

            MemberMatches matches = TypeHelper.GetMemberMatches(left, right);

            return matches.values.Count == matches.leftProcessedMemberCount;
        }

        public static bool IsNotTypeBySemantics(this Type left, Type right)
        {
            return !left.IsTypeBySemantics(right);
        }

        /// <summary>
        /// Correctly compares primitive number values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsType(this Type value, Type? other)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (value == other)
                return true;

            if (other.IsPrimitiveNumber() && value.IsPrimitiveNumber())
                return IsPrimitiveNumberValueAssignable(other, value);

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
                if (a.IsType(types[i]))
                    return true;
            }

            return false;
        }

        public static bool IsNotAnyType(this Type value, params Type?[] types)
        {
            return !value.IsAnyType(types);
        }

        public static string GetName(this Type? type,
            TypeNameConvertingAttributes attributes = TypeNameConvertingAttributes.Default)
        {
            if (type == null) return "null";

            if (attributes.IsFlagSetted(TypeNameConvertingAttributes.ShortName)
                &&
                type.IsBasicType()
                )
                return GetShortName(type);

            if (type.IsGenericType)
            {
                if (attributes.IsFlagSetted(TypeNameConvertingAttributes.IncludeGenericArguments))
                    return ConvertGenericArgumentsToString(type);
                else
                    return type.Name[..^2];
            }
            else return type.Name;
        }

        public static string GetFullName(this Type value,
            TypeNameConvertingAttributes nameAttributes = TypeNameConvertingAttributes.Default)
        {
            CC.Guard.NullArgument(value, nameof(value));

            if (value.Namespace.IsNotNullOrEmpty())
                return value.Namespace + '.' + value.GetName(nameAttributes);

            return value.GetName(nameAttributes);
        }

        public static string GetShortName(Type type)
        {
            CC.Guard.ArgumentObsolete(type, nameof(type), !type.IsGenericType);

            if (type == typeof(short))
                return "short";
            else if (type == typeof(ushort))
                return "ushort";
            else if (type == typeof(int))
                return "int";
            else if (type == typeof(uint))
                return "uint";
            else if (type == typeof(long))
                return "long";
            else if (type == typeof(ulong))
                return "ulong";
            else if (type == typeof(bool))
                return "bool";
            else if (type == typeof(float))
                return "float";
            else if (type.IsType<Array>())
                return $"{type.GetName(TypeNameConvertingAttributes.IncludeGenericArguments)}[]";

            return type.Name.ToLower();
        }

        private static string ConvertGenericArgumentsToString(Type type)
        {
            Type[] argumentTypes = type.GetGenericArguments();

            StringBuilder sb = new();

            sb.Append('<');
            sb.AppendJoin(", ", argumentTypes.Select(x => x.GetName()));
            sb.Append('>');

            return type.Name[..^2] + sb.ToString();
        }

        private static bool IsPrimitiveNumberValueAssignable(Type to, Type from)
        {
            if (to == typeof(int) || to == typeof(uint))
            {
                if (from == typeof(byte)
                    ||
                    from == typeof(sbyte)
                    ||
                    from == typeof(short)
                    ||
                    from == typeof(ushort)
                    )
                    return true;

                return false;
            }
            else if (to == typeof(long) || to == typeof(ulong))
            {
                if (from == typeof(byte)
                    ||
                    from == typeof(sbyte)
                    ||
                    from == typeof(short)
                    ||
                    from == typeof(ushort)
                    ||
                    from == typeof(int)
                    ||
                    from == typeof(uint)
                     )
                    return true;

                return false;
            }
            else if (to == typeof(short) || to == typeof(ushort))
            {
                if (from == typeof(byte)
                    ||
                    from == typeof(sbyte)
                    )
                    return true;

                return false;
            }

            return false;
        }
    }
}
