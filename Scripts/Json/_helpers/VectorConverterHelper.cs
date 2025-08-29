using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public static class VectorConverterHelper
    {
        public static Vector4 ReadVector(JsonReader reader)
        {
            var token = JToken.ReadFrom(reader);

            if (token is null)
                return default;

            try
            {
                var value = token.Value<float>();

                return new Vector4(value, value, value, value);
            }
            catch
            {
                //Empty
            }

            JToken? prop = token["all"];

            if (prop is not null)
            {
                var value = prop.Value<float>();

                return new Vector4(value, value, value, value);
            }

            var result = new Vector4();
            prop = token["x"];
            if (prop is not null)
            {
                var value = prop.Value<float>();

                result.x = value;
            }

            prop = token["y"];
            if (prop is not null)
            {
                var value = prop.Value<float>();

                result.x = value;
            }

            prop = token["z"];
            if (prop is not null)
            {
                var value = prop.Value<float>();

                result.x = value;
            }

            prop = token["w"];
            if (prop is not null)
            {
                var value = prop.Value<float>();

                result.x = value;
            }

            return result;
        }

        public static void WriteVector(JsonWriter writer, object vector)
        {
            var vector4 = vector switch
            {
                Vector2 typed => new Vector4(typed.x, typed.y),
                Vector2Int typed => new Vector4(typed.x, typed.y),
                Vector3 typed => new Vector4(typed.x, typed.y, typed.z),
                Vector3Int typed => new Vector4(typed.x, typed.y, typed.z),
                Vector4 typed => typed,
                _ => throw new System.InvalidOperationException(vector.GetType().GetName()),
            };

            if (vector4.Equals(new Vector4(vector4.x, vector4.x, vector4.x, vector4.x)))
            {
                writer.WriteValue(vector4.x);
                return;
            }

            writer.WriteStartObject();

            if (vector4.x.IsNotDefault())
            {
                writer.WritePropertyName("x");
                writer.WriteValue(vector4.x);
            }

            if (vector4.x.IsNotDefault())
            {
                writer.WritePropertyName("y");
                writer.WriteValue(vector4.y);
            }

            if (vector4.x.IsNotDefault())
            {
                writer.WritePropertyName("z");
                writer.WriteValue(vector4.y);
            }

            if (vector4.x.IsNotDefault())
            {
                writer.WritePropertyName("w");
                writer.WriteValue(vector4.y);
            }

            writer.WriteEndObject();
        }
    }
}
