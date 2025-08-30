using CCEnvs.Diagnostics;
using CCEnvs.Unity.Json.Converters;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;

#nullable enable
namespace CCEnvs.Unity.Json
{
    public static class JsonSerializerSettingsProvider
    {
        public static JsonConverter[] Converters { get; private set; } = new JsonConverter[]
        {
            new Vector2Converter(),
            new Vector2IntConverter(),
            new Vector3Converter(),
            new Vector3IntConverter(),
        };

        public static void AddConverters(params JsonConverter[] converters)
        {
            if (converters.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(converters), converters);

            Converters = Converters.Concat(converters).ToArray();
        }

        public static JsonSerializerSettings GetSettings(object? context = null, 
            StreamingContextStates contextStates = StreamingContextStates.File)
        {
            JsonSerializerSettings defaultSettings = JsonConvert.DefaultSettings?.Invoke()
                                                     ??
                                                     new JsonSerializerSettings();

            defaultSettings.Context = new StreamingContext(contextStates, context);
            defaultSettings.Converters = defaultSettings.Converters.Concat(Converters).ToArray();

            return defaultSettings;
        }
    }
}
