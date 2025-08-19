using Newtonsoft.Json;
using System.Linq;

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
    }
}
