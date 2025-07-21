using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable

namespace UTIRLib.Reflection
{
    public static class TypeHelper
    {
        /// <exception cref="TypeNotFoundException"></exception>
        public static Type GetTypeBySpecialName(string shortName, bool throwOnError = true)
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

        public static bool HasSpecialName(Type? type)
        {
            if (type == null) return false;

            return type.IsAny(typeof(byte),
                              typeof(sbyte),
                              typeof(short),
                              typeof(ushort),
                              typeof(int),
                              typeof(uint),
                              typeof(long),
                              typeof(ulong),
                              typeof(string),
                              typeof(bool),
                              typeof(object)
                              );
        }

        public static MemberInfo[] ForceGetMembers(Type type,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var toProccess = new List<Type>{ type };

            var predicate = new LoopPredicate(() => true);
            while (predicate.Invoke())
            {
                type = type.BaseType;

                if (type == typeof(object) || type == null)
                    break;

                toProccess.Add(type);
            }

            bindingFlags |= BindingFlags.DeclaredOnly;
            var members = new List<MemberInfo>();
            int toProccessCount = toProccess.Count;
            for (int i = 0; i < toProccessCount; i++)
                members.AddRange(toProccess[i].GetMembers(bindingFlags));

            return members.ToArray();
        }

        public static T[] ForceGetMembers<T>(Type type,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            return ForceGetMembers(type, bindingFlags).Where(x => x is T)
                                                       .Cast<T>()
                                                       .ToArray();
        }

        /// <summary>
        /// Gets all member from specified type and base types
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public unsafe static T[] GetAllMembers<T>(Type type, BindingFlags bindingFlags = BindingFlags.Default)
            where T : MemberInfo
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            MemberInfo[] members = type.GetMembers(bindingFlags);
            List<T> results = new();

            int membersCount = members.Length;
            for (int i = 0; i < membersCount; i++)
            {
                if (members[i] is T typed)
                    results.Add(typed);
            }

            return results.ToArray();
        }
    }
}