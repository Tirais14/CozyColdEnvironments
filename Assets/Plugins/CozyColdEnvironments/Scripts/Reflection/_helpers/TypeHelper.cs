using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection.Caching;
using CommunityToolkit.Diagnostics;

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

            return Loops.BreadthFirstSearch(type, x => x.BaseType);
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

        public static object ConvertNumber(object number, Type targetNumberType, bool @unchecked = true)
        {
            CC.Guard.IsNotNull(number, nameof(number));
            Guard.IsNotNull(targetNumberType);

            if (!targetNumberType.IsPrimitiveNumber())
                throw new ArgumentException($"Unexpected type: {targetNumberType}");

            switch (number)
            {
                case long value:
                    {
                        if (targetNumberType == TypeofCache<int>.Type)
                        {
                            if (@unchecked)
                                return (int)value;
                            else
                                return checked((int)value);
                        }
                        else if (targetNumberType == TypeofCache<uint>.Type)
                        {
                            if (@unchecked)
                                return (uint)value;
                            else
                                return checked((uint)value);
                        }
                        else if (targetNumberType == TypeofCache<short>.Type)
                        {
                            if (@unchecked)
                                return (short)value;
                            else
                                return checked((short)value);
                        }
                        else if (targetNumberType == TypeofCache<ushort>.Type)
                        {
                            if (@unchecked)
                                return (ushort)value;
                            else
                                return checked((ushort)value);
                        }
                        else if (targetNumberType == TypeofCache<byte>.Type)
                        {
                            if (@unchecked)
                                return (byte)value;
                            else
                                return checked((byte)value);
                        }
                        else if (targetNumberType == TypeofCache<sbyte>.Type)
                        {
                            if (@unchecked)
                                return (sbyte)value;
                            else
                                return checked((sbyte)value);
                        }
                        else if (targetNumberType == TypeofCache<ulong>.Type)
                        {
                            if (@unchecked)
                                return (ulong)value;
                            else
                                return checked((ulong)value);
                        }

                        return number;
                    }
                case ulong value:
                    {
                        if (targetNumberType == TypeofCache<int>.Type)
                        {
                            if (@unchecked)
                                return (int)value;
                            else
                                return checked((int)value);
                        }
                        else if (targetNumberType == TypeofCache<uint>.Type)
                        {
                            if (@unchecked)
                                return (uint)value;
                            else
                                return checked((uint)value);
                        }
                        else if (targetNumberType == TypeofCache<short>.Type)
                        {
                            if (@unchecked)
                                return (short)value;
                            else
                                return checked((short)value);
                        }
                        else if (targetNumberType == TypeofCache<ushort>.Type)
                        {
                            if (@unchecked)
                                return (ushort)value;
                            else
                                return checked((ushort)value);
                        }
                        else if (targetNumberType == TypeofCache<byte>.Type)
                        {
                            if (@unchecked)
                                return (byte)value;
                            else
                                return checked((byte)value);
                        }
                        else if (targetNumberType == TypeofCache<sbyte>.Type)
                        {
                            if (@unchecked)
                                return (sbyte)value;
                            else
                                return checked((sbyte)value);
                        }
                        else if (targetNumberType == TypeofCache<long>.Type)
                        {
                            if (@unchecked)
                                return (long)value;
                            else
                                return checked((long)value);
                        }

                        return number;
                    }
                case int value:
                    {
                        if (targetNumberType == TypeofCache<uint>.Type)
                        {
                            if (@unchecked)
                                return (uint)value;
                            else
                                return checked((uint)value);
                        }
                        else if (targetNumberType == TypeofCache<short>.Type)
                        {
                            if (@unchecked)
                                return (short)value;
                            else
                                return checked((short)value);
                        }
                        else if (targetNumberType == TypeofCache<ushort>.Type)
                        {
                            if (@unchecked)
                                return (ushort)value;
                            else
                                return checked((ushort)value);
                        }
                        else if (targetNumberType == TypeofCache<byte>.Type)
                        {
                            if (@unchecked)
                                return (byte)value;
                            else
                                return checked((byte)value);
                        }
                        else if (targetNumberType == TypeofCache<sbyte>.Type)
                        {
                            if (@unchecked)
                                return (sbyte)value;
                            else
                                return checked((sbyte)value);
                        }
                        else if (targetNumberType == TypeofCache<long>.Type)
                        {
                            if (@unchecked)
                                return (long)value;
                            else
                                return checked((long)value);
                        }
                        else if (targetNumberType == TypeofCache<ulong>.Type)
                        {
                            if (@unchecked)
                                return (ulong)value;
                            else
                                return checked((ulong)value);
                        }

                        return number;
                    }
                case uint value:
                    {
                        if (targetNumberType == TypeofCache<int>.Type)
                        {
                            if (@unchecked)
                                return (int)value;
                            else
                                return checked((int)value);
                        }
                        else if (targetNumberType == TypeofCache<short>.Type)
                        {
                            if (@unchecked)
                                return (short)value;
                            else
                                return checked((short)value);
                        }
                        else if (targetNumberType == TypeofCache<ushort>.Type)
                        {
                            if (@unchecked)
                                return (ushort)value;
                            else
                                return checked((ushort)value);
                        }
                        else if (targetNumberType == TypeofCache<byte>.Type)
                        {
                            if (@unchecked)
                                return (byte)value;
                            else
                                return checked((byte)value);
                        }
                        else if (targetNumberType == TypeofCache<sbyte>.Type)
                        {
                            if (@unchecked)
                                return (sbyte)value;
                            else
                                return checked((sbyte)value);
                        }
                        else if (targetNumberType == TypeofCache<long>.Type)
                        {
                            if (@unchecked)
                                return (long)value;
                            else
                                return checked((long)value);
                        }
                        else if (targetNumberType == TypeofCache<ulong>.Type)
                        {
                            if (@unchecked)
                                return (ulong)value;
                            else
                                return checked((ulong)value);
                        }

                        return number;
                    }
                case short value:
                    {
                        if (targetNumberType == TypeofCache<ushort>.Type)
                        {
                            if (@unchecked)
                                return (ushort)value;
                            else
                                return checked((ushort)value);
                        }
                        else if (targetNumberType == TypeofCache<byte>.Type)
                        {
                            if (@unchecked)
                                return (byte)value;
                            else
                                return checked((byte)value);
                        }
                        else if (targetNumberType == TypeofCache<sbyte>.Type)
                        {
                            if (@unchecked)
                                return (sbyte)value;
                            else
                                return checked((sbyte)value);
                        }
                        else if (targetNumberType == TypeofCache<int>.Type)
                        {
                            if (@unchecked)
                                return (int)value;
                            else
                                return checked((int)value);
                        }
                        else if (targetNumberType == TypeofCache<uint>.Type)
                        {
                            if (@unchecked)
                                return (uint)value;
                            else
                                return checked((uint)value);
                        }
                        else if (targetNumberType == TypeofCache<long>.Type)
                        {
                            if (@unchecked)
                                return (long)value;
                            else
                                return checked((long)value);
                        }
                        else if (targetNumberType == TypeofCache<ulong>.Type)
                        {
                            if (@unchecked)
                                return (ulong)value;
                            else
                                return checked((ulong)value);
                        }

                        return number;
                    }
                case ushort value:
                    {
                        if (targetNumberType == TypeofCache<short>.Type)
                        {
                            if (@unchecked)
                                return (short)value;
                            else
                                return checked((short)value);
                        }
                        else if (targetNumberType == TypeofCache<byte>.Type)
                        {
                            if (@unchecked)
                                return (byte)value;
                            else
                                return checked((byte)value);
                        }
                        else if (targetNumberType == TypeofCache<sbyte>.Type)
                        {
                            if (@unchecked)
                                return (sbyte)value;
                            else
                                return checked((sbyte)value);
                        }
                        else if (targetNumberType == TypeofCache<int>.Type)
                        {
                            if (@unchecked)
                                return (int)value;
                            else
                                return checked((int)value);
                        }
                        else if (targetNumberType == TypeofCache<uint>.Type)
                        {
                            if (@unchecked)
                                return (uint)value;
                            else
                                return checked((uint)value);
                        }
                        else if (targetNumberType == TypeofCache<long>.Type)
                        {
                            if (@unchecked)
                                return (long)value;
                            else
                                return checked((long)value);
                        }
                        else if (targetNumberType == TypeofCache<ulong>.Type)
                        {
                            if (@unchecked)
                                return (ulong)value;
                            else
                                return checked((ulong)value);
                        }

                        return number;
                    }
                case byte value:
                    {
                        if (targetNumberType == TypeofCache<sbyte>.Type)
                        {
                            if (@unchecked)
                                return (sbyte)value;
                            else
                                return checked((sbyte)value);
                        }
                        else if (targetNumberType == TypeofCache<short>.Type)
                        {
                            if (@unchecked)
                                return (short)value;
                            else
                                return checked((short)value);
                        }
                        else if (targetNumberType == TypeofCache<ushort>.Type)
                        {
                            if (@unchecked)
                                return (ushort)value;
                            else
                                return checked((ushort)value);
                        }
                        else if (targetNumberType == TypeofCache<int>.Type)
                        {
                            if (@unchecked)
                                return (int)value;
                            else
                                return checked((int)value);
                        }
                        else if (targetNumberType == TypeofCache<uint>.Type)
                        {
                            if (@unchecked)
                                return (ulong)value;
                            else
                                return checked((ulong)value);
                        }
                        else if (targetNumberType == TypeofCache<long>.Type)
                        {
                            if (@unchecked)
                                return (long)value;
                            else
                                return checked((long)value);
                        }
                        else if (targetNumberType == TypeofCache<ulong>.Type)
                        {
                            if (@unchecked)
                                return (ulong)value;
                            else
                                return checked((ulong)value);
                        }

                        return number;
                    }
                case sbyte value:
                    {
                        if (targetNumberType == TypeofCache<byte>.Type)
                        {
                            if (@unchecked)
                                return (byte)value;
                            else
                                return checked((byte)value);
                        }
                        else if (targetNumberType == TypeofCache<short>.Type)
                        {
                            if (@unchecked)
                                return (short)value;
                            else
                                return checked((short)value);
                        }
                        else if (targetNumberType == TypeofCache<ushort>.Type)
                        {
                            if (@unchecked)
                                return (ushort)value;
                            else
                                return checked((ushort)value);
                        }
                        else if (targetNumberType == TypeofCache<int>.Type)
                        {
                            if (@unchecked)
                                return (int)value;
                            else
                                return checked((int)value);
                        }
                        else if (targetNumberType == TypeofCache<uint>.Type)
                        {
                            if (@unchecked)
                                return (ulong)value;
                            else
                                return checked((ulong)value);
                        }
                        else if (targetNumberType == TypeofCache<long>.Type)
                        {
                            if (@unchecked)
                                return (long)value;
                            else
                                return checked((long)value);
                        }
                        else if (targetNumberType == TypeofCache<ulong>.Type)
                        {
                            if (@unchecked)
                                return (ulong)value;
                            else
                                return checked((ulong)value);
                        }

                        return number;
                    }
                default:
                    return number;
            }
        }
        public static string GetTypeReference(this Type source)
        {
            Guard.IsNotNull(source);

            return $"{source.FullName}, {source.Assembly.GetName().Name}";
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

            return TypeCache.Get(type).IsPrimitive;
        }

        public static bool IsPrimitiveNumber(this Type value)
        {
            return TypeCache.Get(value).IsPrimitive
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
        public static bool IsType(
            this Type source,
            Type? other,
            TypeMatchingSettings settings = TypeMatchingSettings.Default
            )
        {
            CC.Guard.IsNotNullSource(source);

            if (other is null)
                return false;

            if (source == other)
                return true;

            if (other.IsPrimitiveNumber() && source.IsPrimitiveNumber())
                return IsPrimitiveNumberValueAssignable(other, source);

            bool result = other.IsAssignableFrom(source);

            if (!result
                &&
                ((settings & TypeMatchingSettings.ByBaseGenericTypeDefinition) != 0
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
                    foreach (var iface in sourceDef.GetInterfaces())
                    {
                        if (!iface.IsGenericType)
                            continue;

                        if (otherDef.IsAssignableFrom(iface.GetGenericTypeDefinition()))
                            result = true;
                    }
                }
            }

            return result;
        }
        public static bool IsType<T>(this Type source, TypeMatchingSettings settings = TypeMatchingSettings.Default)
        {
            return source.IsType(typeof(T), settings: settings);
        }

        public static bool IsNotType(this Type source, Type? other, TypeMatchingSettings settings = TypeMatchingSettings.Default)
        {
            return !source.IsType(other, settings: settings);
        }
        public static bool IsNotType<T>(this Type source, TypeMatchingSettings settings = TypeMatchingSettings.Default)
        {
            return source.IsNotType(typeof(T), settings: settings);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsAnyType(this Type a, params Type?[] types)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            for (int i = 0; i < types.Length; i++)
                if (a.IsType(types[i]))
                    return true;

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

            if (attributes.HasFlagT(TypeNameConvertingAttributes.ShortName)
                &&
                type.IsBasicType()
                )
                return GetShortName(type);

            if (type.IsGenericType)
            {
                if (attributes.HasFlagT(TypeNameConvertingAttributes.IncludeGenericArguments))
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
                  .IncludeStatic()
                  .IncludeNonPublic()
                  .ByFullName()
                  .WithName("op_Implicit")
                  .Methods()
                  .Concat(source.Reflect()
                      .IncludeStatic()
                      .IncludeNonPublic()
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

        private static readonly Lazy<Dictionary<Type, object>> defaultValues = new(static () => new Dictionary<Type, object>());
        public static object? GetDefaultValue(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            if (!type.IsValueType)
                return null;

            if (defaultValues.IsValueCreated
                &&
                defaultValues.Value.TryGetValue(type, out var defaultValue))
            {
                return default;
            }

            defaultValue = Activator.CreateInstance(type);

            defaultValues.Value.Add(type, defaultValue);

            return defaultValue;
        }

        private static string ConvertGenericArgumentsToString(
            Type type,
            TypeNameConvertingAttributes typeNameConvertingAttributes = TypeNameConvertingAttributes.Default
            )
        {
            Guard.IsNotNull(type, nameof(type));

            var argTypes = type.GetGenericArguments();

            var argTypeNames = ArrayPool<string>.Shared.Get(argTypes.Length);

            try
            {
                for (int i = 0; i < argTypes.Length; i++)
                    argTypeNames[i] = argTypes[i].GetName(typeNameConvertingAttributes);

                StringBuilder sb = new();

                sb.Append('<');
                sb.AppendJoin(", ", argTypeNames);
                sb.Append('>');

                var typeNameSpan = type.Name.AsSpan()[..^2];

                sb.Append(typeNameSpan);

                return sb.ToString();
            }
            finally
            {
                argTypeNames.Dispose();
            }
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