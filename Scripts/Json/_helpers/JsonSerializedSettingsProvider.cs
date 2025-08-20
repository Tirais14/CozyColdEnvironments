using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UTIRLib.Json.Converters;
using Object = UnityEngine.Object;

#nullable enable
namespace UTIRLib.Json
{
    public static class JsonSerializedSettingsProvider
    {
        public static JsonConverter[] Converters { get; private set; } = new JsonConverter[]
        {
        };

        public static void AddConverters(params JsonConverter[] converters)
        {
            if (converters.IsNullOrEmpty())
                throw new Diagnostics.CollectionArgumentException(nameof(converters), converters);

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
