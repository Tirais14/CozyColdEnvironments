using Newtonsoft.Json;
using System;
using UnityEngine;

namespace CCEnvs.Unity.Json.Converters
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 ReadJson(JsonReader reader,
                                         Type objectType,
                                         Vector2 existingValue,
                                         bool hasExistingValue,
                                         JsonSerializer serializer)
        {
            return VectorConverterHelper.ReadVector(reader);
        }

        public override void WriteJson(JsonWriter writer,
                                       Vector2 value,
                                       JsonSerializer serializer)
        {
            VectorConverterHelper.WriteVector(writer, value);
        }
    }
}
