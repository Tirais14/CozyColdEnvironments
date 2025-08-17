using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UTIRLib.Json.DTO;

#nullable enable
#pragma warning disable S101
namespace UTIRLib.Json.Convertes
{
    public sealed class AssetReferenceDTOConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(AssetReferenceDTO<>);

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null) { writer.WriteNull(); return; }

            var t = value.GetType();

            // 1) Берём Guid из DTO
            var guid = (string?)t.GetProperty("Guid")!.GetValue(value);
            var sub = (string?)t.GetProperty("SubObjectName")!.GetValue(value);

            // 2) Если Guid пуст — fallback к Reference.AssetGUID
            if (string.IsNullOrEmpty(guid))
            {
                var reference = t.GetProperty("Reference", BindingFlags.Instance | BindingFlags.Public)!.GetValue(value) as AssetReference;
                if (reference != null)
                {
                    guid = reference.AssetGUID;
                    if (string.IsNullOrWhiteSpace(sub))
                        sub = TryGetSub(reference);
                }
            }

            // Если вообще нечего писать — null, а не пустая строка
            if (string.IsNullOrEmpty(guid))
            {
                writer.WriteNull();
                return;
            }

            if (string.IsNullOrWhiteSpace(sub))
            {
                writer.WriteValue(guid);
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("guid"); writer.WriteValue(guid);
            writer.WritePropertyName("sub"); writer.WriteValue(sub);
            writer.WriteEndObject();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            string? guid = null;
            string? sub = null;

            if (reader.TokenType == JsonToken.String)
            {
                guid = (string?)reader.Value;
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                var jo = JObject.Load(reader);
                guid = jo["guid"]?.ToString();
                sub = jo["sub"]?.ToString();
            }
            else
            {
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} for {objectType}.");
            }

            var dto = Activator.CreateInstance(objectType)!;

            // Устанавливаем DTO-поля
            objectType.GetProperty("Guid")!.SetValue(dto, guid ?? string.Empty);
            objectType.GetProperty("SubObjectName")!.SetValue(dto, string.IsNullOrWhiteSpace(sub) ? null : sub);

            // Воссоздаём и записываем Reference, чтобы инспектор сразу показал выбранный адресабл
            if (!string.IsNullOrWhiteSpace(guid))
            {
                var trefType = objectType.GetGenericArguments()[0];
                var reference = Activator.CreateInstance(trefType, guid) as AssetReference;
                if (reference != null && !string.IsNullOrWhiteSpace(sub))
                    TrySetSub(reference, sub!);

                objectType.GetProperty("Reference")!.SetValue(dto, reference);
            }

            return dto;
        }

        private static string? TryGetSub(AssetReference r)
        {
            var prop = r.GetType().GetProperty("SubObjectName", BindingFlags.Instance | BindingFlags.Public);
            if (prop?.CanRead == true)
            {
                var val = prop.GetValue(r) as string;
                if (!string.IsNullOrWhiteSpace(val)) return val;
            }
            var field = r.GetType().GetField("m_SubObjectName", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                var val = field.GetValue(r) as string;
                if (!string.IsNullOrWhiteSpace(val)) return val;
            }
            return null;
        }

        private static void TrySetSub(AssetReference r, string value)
        {
            var prop = r.GetType().GetProperty("SubObjectName", BindingFlags.Instance | BindingFlags.Public);
            if (prop?.CanWrite == true) { prop.SetValue(r, value); return; }
            var field = r.GetType().GetField("m_SubObjectName", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null) field.SetValue(r, value);
        }
    }
}
