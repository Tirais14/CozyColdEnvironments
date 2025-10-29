#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CCEnvs.Json
{
    public static class JsonConverterHelper
    {
        /// <summary>
        /// Only for generic converters for now
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static Type GetConversationType(JsonConverter converter)
        {
            CC.Guard.IsNotNull(converter, nameof(converter));

            Type type = converter.GetType();
            if (!JsonHelper.IsDefaultJsonType(type))
                type = GetDefaultJsonConverterType(converter);

            if (!type.IsGenericType)
                throw new NotSupportedException("Cannot resolve converter type for non generic version.");

            return type.GetGenericArguments()[0];
        }

        public static Type GetDefaultJsonConverterType(JsonConverter converter)
        {
            CC.Guard.IsNotNull(converter, nameof(converter));
            Type type = converter.GetType();
            if (type.Namespace.IsNotNullOrEmpty()
                &&
                type.Namespace.ContainsOrdinal(GJson.Namespace)
                )
                return type;

            Queue<Type> baseTypes = TypeHelper.CollectBaseTypes(type);

            return (from baseType in baseTypes
                    where baseType.Namespace.IsNotNullOrEmpty()
                    where baseType.Namespace.ContainsOrdinal(GJson.Namespace)
                    select baseType)
                    .First();
        }
    }
}
