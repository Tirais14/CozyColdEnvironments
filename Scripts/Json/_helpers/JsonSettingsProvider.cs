using CCEnvs.Diagnostics;
using CCEnvs.Json.Converters;
using CCEnvs.Json.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace CCEnvs.Json
{
    public static class JsonSettingsProvider
    {
        public static JsonConverter[] Converters { get; private set; } = new JsonConverter[]
        {
            new CommonDtoJsonConverter<TypeDto, Type>()
        };
        private readonly static Dictionary<Type, Delegate> dtoConverters = new();

        /// <exception cref="CollectionArgumentException"></exception>
        public static void AddConverters(params JsonConverter[] converters)
        {
            if (converters.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(converters), converters);

            Converters = Converters.Concat(converters).ToArray();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static void ReplaceOrAddConverter(JsonConverter converter)
        {
            if (converter is null)
                throw new ArgumentNullException(nameof(converter));

            Type converterType = converter.GetType();
            for (int i = 0; i < Converters.Length; i++)
            {
                if (Converters[i].GetType() == converterType)
                    Converters[i] = converter;
            }

            AddConverters(converter);
        }


        /// <exception cref="ArgumentNullException"></exception>
        public static void ReplaceOrAddConverters(params JsonConverter[] converters)
        {
            if (converters is null)
                throw new ArgumentNullException(nameof(converters));

            for (int i = 0; i < converters.Length; i++)
                ReplaceOrAddConverter(converters[i]);
        }

        public static void AddOrReplaceDtoConverter(Type dtoType,
            Func<IJsonDto, object> func)
        {
            if (dtoConverters.ContainsKey(dtoType))
            {
                dtoConverters[dtoType] = func;
                return;
            }

            dtoConverters.Add(dtoType, func);
        }

        public static bool TryGetDtoConverter(Type dtoType,
            [NotNullWhen(true)] out Func<IJsonDto, object>? result)
        {
            if (dtoConverters.TryGetValue(dtoType, out Delegate methodUntyped))
            {
                result = (Func<IJsonDto, object>)methodUntyped;
                return true;
            }

            result = null;
            return false;
        }

        public static JsonSerializerSettings GetSettings()
        {
            JsonSerializerSettings? defaultSettings = null;
            if (JsonConvert.DefaultSettings != GetSettingsInternal)
                defaultSettings = JsonConvert.DefaultSettings?.Invoke();

            defaultSettings ??= new JsonSerializerSettings();

            defaultSettings.Converters = defaultSettings.Converters.Concat(Converters)
                                                                   .Distinct()
                                                                   .ToArray();

            defaultSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(true, true, true)
            };
            defaultSettings.Formatting = Formatting.Indented;

            return defaultSettings;
        }

        private static JsonSerializerSettings GetSettingsInternal()
        {
            return GetSettings();
        }
    }
}
