using CCEnvs.Json.Converters;
using CCEnvs.Json.Diagnsotics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;

#nullable enable
namespace CCEnvs.Json
{
    public static class JsonSettingsProvider
    {
        public static CCJsonConverterCollection Converters { get; private set; } = new()
        {
            new TypeJsonConverter()
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
