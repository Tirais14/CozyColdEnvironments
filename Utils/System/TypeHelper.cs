using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable

namespace UTIRLib.Utils
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