using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Json.Converters;
using CCEnvs.Json.Diagnsotics;
using CCEnvs.Json.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs.Json
{
    public delegate object? JsonConverterFunc(object? value);

    public static class JsonSettingsProvider
    {
        public static CCJsonConverterCollection Converters { get; private set; } = new()
        {
            new PolymorphJsonConverter<TypeDto, Type>()
        };

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
            defaultSettings.Error = JsonSerializerDebug.OnError;

            return defaultSettings;
        }

        private static JsonSerializerSettings GetSettingsInternal()
        {
            return GetSettings();
        }
    }
}
