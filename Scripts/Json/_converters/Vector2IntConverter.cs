using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CozyColdEnvironments.Json.Converters
{
    public class Vector2IntConverter : JsonConverter<Vector2Int>
    {
        public override Vector2Int ReadJson(JsonReader reader,
                                            Type objectType,
                                            Vector2Int existingValue,
                                            bool hasExistingValue,
                                            JsonSerializer serializer)
        {
            var vector = VectorConverterHelper.ReadVector(reader);

            return new Vector2Int((int)vector.x, (int)vector.y);
        }

        public override void WriteJson(JsonWriter writer,
                                       Vector2Int value,
                                       JsonSerializer serializer)
        {
            VectorConverterHelper.WriteVector(writer, value);
        }
    }
}
