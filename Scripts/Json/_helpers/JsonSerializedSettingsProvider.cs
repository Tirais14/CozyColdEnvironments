using Newtonsoft.Json;
using System.Linq;

#nullable enable
namespace UTIRLib.Json
{
    public static class JsonSerializedSettingsProvider
    {
        public static JsonConverter[] Converters { get; } = new JsonConverter[]
        {
        };
    }
}
