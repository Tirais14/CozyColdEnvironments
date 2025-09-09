using System;
using System.Linq;
using System.Collections.Generic;
using CCEnvs.Diagnostics;
using System.Runtime.InteropServices;

#nullable enable

namespace CCEnvs.Reflection
{
    public static class TypeHelper
    {
        /// <summary>
        /// Finds type with the largest number of base types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Type GetElderType(IEnumerable<Type>? types, Type? restriction = null)
        {
            CC.Validate.CollectionArgument(nameof(types), types);

            IEnumerable<Type> ordered = from type in types
                                        orderby type.GetBaseTypeCount() descending
                                        select type;

            if (restriction is not null)
                return ordered.FirstOrDefault(x => x.IsType(restriction))
                       ??
                       throw new LogicException("Incorrect input types. Not found any matches by setted restriction.");

            return ordered.First();
        }

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