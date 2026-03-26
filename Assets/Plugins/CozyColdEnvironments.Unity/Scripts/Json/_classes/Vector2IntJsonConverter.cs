using CCEnvs.Attributes;
using CCEnvs.Json;
using Newtonsoft.Json;
using System;
using System.Globalization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Json
{
    public class Vector2IntJsonConverter : JsonConverter<Vector2Int>
    {
        public override Vector2Int ReadJson(
            JsonReader reader,
            Type objectType,
            Vector2Int existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
            )
        {
            if (reader.TokenType == JsonToken.Null
                ||
                reader.Value is not string serialized)
            {
                return default;
            }

            var raw = serialized.Split("; ");

            for (int i = 0; i < raw.Length; i++)
                existingValue[i] = int.Parse(raw[i].Split(':')[1], serializer.Culture);

            return existingValue;
        }

        public override void WriteJson(
            JsonWriter writer,
            Vector2Int value,
            JsonSerializer serializer
            )
        {
            writer.WriteValue($"x:{value.x.ToString(serializer.Culture)}; y:{value.y.ToString(serializer.Culture)}");
        }

        [OnInstallExecutable]
        private static void OnInstall()
        {
            CC.SerializerSettings.AddConverters(new Vector2IntJsonConverter());
        }
    }
}
