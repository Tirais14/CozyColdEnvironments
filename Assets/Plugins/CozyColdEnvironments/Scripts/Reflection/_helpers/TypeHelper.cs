using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable

namespace CCEnvs.Reflection
{
    public static class TypeHelper
    {
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
                            throw new TypeNotFoundException(shortName, "Type hasn't special short name.");
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
    }
}