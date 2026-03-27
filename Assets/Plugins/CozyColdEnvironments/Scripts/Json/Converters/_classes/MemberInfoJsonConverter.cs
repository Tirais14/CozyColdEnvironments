using CCEnvs.Reflection;
using Humanizer;
using Newtonsoft.Json;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public class MemberInfoJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsType<MemberInfo>();
        }

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
            )
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException($"Unexpected readr token. Token: {reader.TokenType}");

            string? id;

            try
            {
                id = (string?)reader.Value;
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
                return null;
            }

            if (id is null)
            {
                this.PrintException(new JsonSerializationException($"Cannot find id"));
                return null;
            }

            if (!MemberID.TryResolveMember(id, out var member))
            {
                this.PrintException(new JsonSerializationException($"Cannot find member. ID: {id}"));
                return null;
            }

            return member;
        }

        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer serializer
            )
        {
            if (value is not MemberInfo memberInfo)
            {
                writer.WriteNull();
                return;
            }

            if (memberInfo.GetCustomAttribute<MemberIDAttribute>().IsNull(out var memberIDAttribute))
                throw new JsonSerializationException($"Cannot find {nameof(MemberIDAttribute).Humanize()}");

            writer.WriteValue(memberIDAttribute.ID);
        }
    }
}
