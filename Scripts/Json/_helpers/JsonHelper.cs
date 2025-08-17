using Newtonsoft.Json;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UTIRLib.Json.Convertes;

#nullable enable
namespace UTIRLib.Json
{
    public static class JsonHelper
    {
        public static JsonConverter[] Converters { get; private set; } = new JsonConverter[]
        {
            new AssetReferenceDTOConverter()
        };

        public static void AddConverters(params JsonConverter[] converters)
        {
            if (converters is null)
                throw new System.ArgumentNullException(nameof(converters));
            if (converters.IsEmpty())
                return;

            Converters = Converters.Concat(converters).ToArray();
        }
    }
}
