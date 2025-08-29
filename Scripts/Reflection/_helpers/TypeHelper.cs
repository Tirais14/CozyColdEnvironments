using System;
using System.Collections.Generic;
using CozyColdEnvironments.Diagnostics;

#nullable enable

namespace CozyColdEnvironments.Reflection
{
    public static class TypeHelper
    {
        public static Queue<Type> CollectBaseTypes(Type value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            return Collector.Collect(value, x => x.BaseType);
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
    }
}