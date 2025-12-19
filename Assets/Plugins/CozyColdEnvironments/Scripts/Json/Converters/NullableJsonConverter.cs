#nullable enable
using CCEnvs.Collections;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public class NullableJsonConverter : JsonConverter
    {
    //    private readonly MemoryCache cache = new(new MemoryCacheOptions
    //    {
    //        ExpirationScanFrequency = 1.Minutes()
    //    });

    //    public override bool CanConvert(Type objectType)
    //    {
    //        if (!cache.TryGetValue(objectType, out var result))
    //        {
    //            var entry = cache.CreateEntry(objectType);

    //            entry.Value = Regex.Match(objectType.FullName, @"^(?:\s*)System.Nullable(?:.*)").Success;
    //            entry.AbsoluteExpirationRelativeToNow = 10.Minutes();
    //        }

    //        return (bool)result!;
    //    }

    //    public override object? ReadJson(
    //        JsonReader reader,
    //        Type objectType,
    //        object? existingValue,
    //        JsonSerializer serializer)
    //    {
    //        var jObj = JObject.Load(reader);
            
    //        if (!jObj.TryGetValue("value", out var valueToken))
    //            throw new JsonSerializationException("Missing 'value' property");

    //        if (!jObj.TryGetValue("hasValue", out var hasValueToken))
    //            throw new JsonSerializationException("Missing 'hasValue' property");

    //        object value = serializer.Deserialize();
    //        Activator.CreateInstance(objectType, args: );
    //    }

    //    public override void WriteJson(
    //        JsonWriter writer,
    //        object? value,
    //        JsonSerializer serializer)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
