using CCEnvs.Common;
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
    public delegate object? CCJsonConverterFunc(object? value);

    public static class JsonSettingsProvider
    {
        public static List<JsonConverter> Converters { get; private set; } = new()
        {
            new CommonDtoJsonConverter<TypeDto, Type>()
        };
        private readonly static Dictionary<Type, CCJsonConverterFunc> dtoConverters = new();

        public static void AddOrReplaceDtoConverter(Type dtoType,
            CCJsonConverterFunc func)
        {
            if (dtoConverters.ContainsKey(dtoType))
            {
                dtoConverters[dtoType] = func;
                return;
            }

            dtoConverters.Add(dtoType, func);
        }
        public static void AddOrReplaceDtoConverter<T>(CCJsonConverterFunc func)
        {
            AddOrReplaceDtoConverter(typeof(T), func);
        }

        public static bool TryGetDtoConverter(Type dtoType,
            [NotNullWhen(true)] out CCJsonConverterFunc? result)
        {
            if (dtoConverters.TryGetValue(dtoType, out CCJsonConverterFunc methodUntyped))
            {
                result = methodUntyped;
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

            defaultSettings.Error = (sender, e) =>
            {
                CCDebug.PrintError(e.ErrorContext.Path, sender);
            };

            return defaultSettings;
        }

        private static JsonSerializerSettings GetSettingsInternal()
        {
            return GetSettings();
        }
    }
}
