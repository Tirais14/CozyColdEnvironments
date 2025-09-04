#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCEnvs.Json
{
    public static class JsonConverterHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static Type ResolveConvertType(JsonConverter converter,
                                              bool throwOnError = true)
        {
            if (converter is null)
                throw new ArgumentNullException(nameof(converter));

            Type type = converter.GetType();
            HashSet<Type> baseTypes = TypeHelper.CollectBaseTypes(type).Distinct().ToHashSet();
            Type[] genericTypes = (from x in baseTypes
                                   where x.IsType<JsonConverter>()
                                   where x.IsGenericType
                                   where x.Namespace.Contains(nameof(Newtonsoft.Json), StringComparison.Ordinal)
                                   select x)
                                   .ToArray();

            if (genericTypes.IsNullOrEmpty())
                throw new LogicException($"Cannot resolve convert type for non-generic {nameof(JsonConverter)}.");

            Type[] genericArguments = (from x in genericTypes
                                       select x.GetGenericArguments() into types
                                       from t in types
                                       select t)
                                       .Distinct()
                                       .ToArray();

            Type genericConverterType = typeof(JsonConverter<>);
            Type[] genericConverters = (from x in genericArguments
                                        select genericConverterType.MakeGenericType(x) into x
                                        select x)
                                       .ToArray();

            for (int i = 0; i < genericConverters.Length; i++)
            {
                if (baseTypes.Contains(genericConverters[i]))
                    return genericConverters[i].GetGenericArguments().Single();
            }

            if (genericArguments is null && throwOnError)
                throw new Diagnostics.DataAccessException(null, $"Cannot resolve type for non generic {nameof(JsonConverter)}.");

            return null;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static JsonConverter[] FindOfType(IEnumerable<JsonConverter> converters,
                                                 Type convertType)
        {
            if (converters is null)
                throw new ArgumentNullException(nameof(converters));
            if (convertType is null)
                throw new ArgumentNullException(nameof(convertType));

            var results = (from x in converters
                           let t = ResolveConvertType(x, throwOnError: false)
                           where t is not null && t.IsType(convertType)
                           select x).ToArray();

            return results;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static JsonConverter[] RemoveByType(IEnumerable<JsonConverter> converters,
                                                   Type convertType)
        {
            if (converters is null)
                throw new ArgumentNullException(nameof(converters));
            if (convertType is null)
                throw new ArgumentNullException(nameof(convertType));

            return converters.Except(FindOfType(converters, convertType)).ToArray();
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static JsonConverter[] RemoveByType(IEnumerable<JsonConverter> converters,
                                                   JsonConverter converter)
        {
            if (converters is null)
                throw new ArgumentNullException(nameof(converters));
            if (converter is null)
                throw new ArgumentNullException(nameof(converter));

            Type seekType = ResolveConvertType(converter);

            return RemoveByType(converters, seekType);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static JsonConverter[] ReplaceByType(IEnumerable<JsonConverter> converters,
                                                    JsonConverter converter)
        {
            if (converters is null)
                throw new ArgumentNullException(nameof(converters));
            if (converter is null)
                throw new ArgumentNullException(nameof(converter));

            return RemoveByType(converters, converter).Concat(CC.Create.Array(converter))
                                                      .ToArray();
        }
    }
}
