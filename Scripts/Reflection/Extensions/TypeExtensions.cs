using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class TypeExtensions
    {
        public static Queue<Type> CollectBaseTypes(this Type value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            return LoopHelper.Collect(value.BaseType, x => x.BaseType);
        }

        /// <summary>
        /// Extends default method and now includes types <see cref="string"/>, <see cref="decimal"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPrimitiveType(this Type? type)
        {
            if (type == null)
                return false;

            return type.IsPrimitive || type.IsAnyType(typeof(decimal), typeof(string));
        }

        /// <summary>
        /// Extended version
        /// </summary>
        /// <param name="type"></param>
        /// <param name="constructorParameters"></param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(this Type type,
            ConstructorParameters constructorParameters,
            bool throwIfNotFound = false)
        {
            if (type.GetConstructor(constructorParameters.BindingFlags,
                                    constructorParameters.Binder,
                                    constructorParameters.CallingConventions,
                                    (Type[])constructorParameters.Signature,
                                    constructorParameters.ParameterModifiers
                                    )
                is ConstructorInfo found
                                    )
                return found;

            ConstructorInfo[] constructors = type.GetConstructors(
                constructorParameters.BindingFlags);

            found = constructors.SingleOrDefault(x =>
            {
                ParameterInfo[] parameters = x.GetParameters();

                if (parameters.Length != constructorParameters.Signature.Count)
                    return false;

                if (constructorParameters.ParameterModifiers.IsNotNullOrEmpty()
                    &&
                    !constructorParameters.ParameterModifiers[0].Equals(parameters.GetParameterModifier())
                    )
                    return false;

                Type[] parametersTypes = parameters.Select(x => x.ParameterType).ToArray();

                return constructorParameters.Signature == parametersTypes;
            });

            if (throwIfNotFound && found is null)
                throw new MemberNotFoundException(type,
                                                  MemberType.Constructor,
                                                  constructorParameters);

            return found;
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
                type.IsPrimitiveType()
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
            BindingFlags bindingFlags = BindingFlagsDefault.InstancePublic)
        {
            return TypeHelper.ForceGetMembers(value, bindingFlags);
        }

        public static FieldInfo[] ForceGetFields(this Type value,
            BindingFlags bindingFlags = BindingFlagsDefault.InstancePublic)
        {
            return TypeHelper.ForceGetMembers<FieldInfo>(value, bindingFlags);
        }

        public static PropertyInfo[] ForceGetProperties(this Type value,
            BindingFlags bindingFlags = BindingFlagsDefault.InstancePublic)
        {
            return TypeHelper.ForceGetMembers<PropertyInfo>(value, bindingFlags);
        }

        public static MethodInfo[] ForceGetMethods(this Type value,
            BindingFlags bindingFlags = BindingFlagsDefault.InstancePublic)
        {
            return TypeHelper.ForceGetMembers<MethodInfo>(value, bindingFlags);
        }

        public static ConstructorInfo[] ForceGetConstructors(this Type value,
            BindingFlags bindingFlags = BindingFlagsDefault.InstancePublic)
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
            else if (type.IsType<float>())
                return "float";
            else if (type.IsType<double>())
                return "double";
            else if (type.IsType<Array>())
                return $"{type.GetName(TypeNameAttributes.Default | ~TypeNameAttributes.ShortName)}[]";

            throw new Exception($"Invalid type {type.Name}.");
        }
    }
}
