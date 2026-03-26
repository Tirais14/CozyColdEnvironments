using CCEnvs.Attributes;
using CCEnvs.Json;
using Newtonsoft.Json;
using System;
using System.Globalization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Json
{
    public class Vector3IntJsonConverter : JsonConverter<Vector3Int>
    {
        public override Vector3Int ReadJson(
            JsonReader reader,
            Type objectType,
            Vector3Int existingValue,
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
            Vector3Int value,
            JsonSerializer serializer
            )
        {
            writer.WriteValue($"x:{value.x.ToString(serializer.Culture)}; y:{value.y.ToString(serializer.Culture)}; z:{value.z.ToString(serializer.Culture)}");
        }

        [OnInstallExecutable]
        private static void OnInstall()
        {
            CC.SerializerSettings.AddConverters(new Vector3IntJsonConverter());
        }
    }
}
