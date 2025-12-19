using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

#nullable enable

namespace CCEnvs.Reflection
{
    public static class TypeHelper
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

        /// <summary>
        /// Also supports interfaces
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Queue<Type> CollectBaseTypes(this Type type)
        {
            CC.Guard.IsNotNull(type, nameof(type));

            //if (type.IsInterface)
            //    return GetInterfaceInheritancePath(type);

            return Do.Collect(type, x => x.BaseType);
        }

        /// <exception cref="TypeNotFoundException"></exception>
        public static Type GetPirmitiveType(string shortName, bool throwOnError = true)
        {
            switch (shortName)
            {
                case "byte":
                    return typeof(byte);
                case "sbyte":
                    return typeof(sbyte);
                case "short":
                    return typeof(short);
                case "ushort":
                    return typeof(ushort);
                case "int":
                    return typeof(int);
                case "uint":
                    return typeof(uint);
                case "long":
                    return typeof(long);
                case "ulong":
                    return typeof(ulong);
                case "string":
                    return typeof(string);
                case "bool":
                    return typeof(bool);
                case "object":
                    return typeof(object);
                default:
                    {
                        if (throwOnError)
                            throw CC.ThrowHelper.TypeNotFoundException(shortName);
                        return null!;
                    }
            }
        }

        public static string GetTypeReference(this Type source)
        {
            Guard.IsNotNull(source);

            return $"{source.FullName}, {source.Assembly.GetName().Name}";
        }

        public static Maybe<object> GetMemberValue(MemberInfo member, object? target)
        {
            Guard.IsNotNull(member);

            return member switch
            {
                FieldInfo field => field.GetValue(target),
                PropertyInfo prop => prop.GetValue(target),
                _ => Maybe<object>.None
            };
        }

        public static Type[] GetPrimitiveTypes()
        {
            return basicTypes.ToArray();
        }

        public static int GetParentsCount(this Type value, bool trimCache = false)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            if (baseTypeCountCache.TryGetValue(value, out int count))
                return count;

            Type[] baseTypes = TypeHelper.CollectBaseTypes(value).ToArray();

            for (int i = 0; i < baseTypes.Length; i++)
                baseTypeCountCache.TryAdd(baseTypes[i], baseTypes.Length - 1 - i);

            if (trimCache)
                baseTypeCountCache.TrimExcess();

            return count;
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

        /// <summary>
        /// Correctly compares primitive number values
        /// </summary>
        /// <param name="source"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsType(this Type source, Type? other, TypeMatchingSettings settings = TypeMatchingSettings.Default)
        {
            Guard.IsNotNull(source, nameof(source));
            if (other is null)
                return false;

            if (source == other)
                return true;

            if (other.IsPrimitiveNumber() && source.IsPrimitiveNumber())
                return IsPrimitiveNumberValueAssignable(other, source);

            bool result = other.IsAssignableFrom(source);

            if (!result
                &&
                (settings.IsFlagSetted(TypeMatchingSettings.ByBaseGenericTypeDefinition)
                ||
                source.IsGenericTypeDefinition)
                &&
                source.IsGenericType
                &&
                other.IsGenericType)
            {
                Type sourceDef = source.GetGenericTypeDefinition();
                Type otherDef = other.GetGenericTypeDefinition();

                result = otherDef.IsAssignableFrom(sourceDef);

                if (!result)
                {
                    result = sourceDef.GetInterfaces()
                        .Where(iface => iface.IsGenericType)
                        .Select(iface => iface.GetGenericTypeDefinition())
                        .Any(iface => otherDef.IsAssignableFrom(iface));
                }
            }

            return result;
        }
        public static bool IsType<T>(this Type value, TypeMatchingSettings settings = TypeMatchingSettings.Default)
        {
            return value.IsType(typeof(T));
        }

        public static bool IsNotType(this Type value, Type other, TypeMatchingSettings settings = TypeMatchingSettings.Default)
        {
            return !value.IsType(other);
        }
        public static bool IsNotType<T>(this Type value, TypeMatchingSettings settings = TypeMatchingSettings.Default)
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
            CC.Guard.IsNotNull(value, nameof(value));

            if (value.Namespace.IsNotNullOrEmpty())
                return value.Namespace + '.' + value.GetName(nameAttributes);

            return value.GetName(nameAttributes);
        }

        public static string GetShortName(Type type)
        {
            Guard.IsTrue(!type.IsGenericType, nameof(type));

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

        public static MethodInfo[] GetOverloadedCastOperators(this Type source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            var t = source.Reflect()
                  .Static()
                  .NonPublic()
                  .ByFullName()
                  .WithName("op_Implicit")
                  .Methods()
                  .Concat(source.Reflect()
                      .Static()
                      .NonPublic()
                      .ByFullName()
                      .WithName("op_Explicit")
                      .Methods())
                  .ToArray();

            return t;
        }

        public static Maybe<MethodInfo> GetOverloadedCastOperator(this Type source,
            Type castType)
        {
            CC.Guard.IsNotNull(source, nameof(source));
            CC.Guard.IsNotNull(castType, nameof(castType));

            return source.GetOverloadedCastOperators()
                .FirstOrDefault(x => x.ReturnType.IsType(castType));
        }

        public static bool TrySwitchType(this Type source,
            params (Type onType, Action<Type> action)[] conditions)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            var convertedConditions = conditions.Select<(Type onType, Action<Type> action), (Predicate<Type?>, Action<Type>)>(condition => ((inputType) => inputType is not null && inputType.IsType(condition.onType), condition.action)).ToArray();
            return ObjectExtensions.TrySwitch(source, convertedConditions);
        }
        public static bool TrySwitchType<TResult>(this Type source,
            out TResult result,
            params (Type onType, Func<Type, TResult> func)[] conditions)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            var convertedConditions = conditions.Select<(Type onType, Func<Type, TResult> func), (Predicate<Type?>, Func<Type, TResult>)>(condition => ((inputType) => inputType is not null && inputType.IsType(condition.onType), condition.func)).ToArray();
            return ObjectExtensions.TrySwitch(source, out result, convertedConditions);
        }

        public static Maybe<TResult> SwitchType<TResult>(this Type source,
            params (Type onType, Func<Type, TResult> func)[] conditions)
        {
            TrySwitchType(source, out TResult result, conditions);

            return result;
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