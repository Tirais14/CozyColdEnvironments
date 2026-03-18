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
            return objectType.IsDefined<MemberIDAttribute>(inherit: true);
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

            string? id;

            try
            {
                id = reader.ReadAsString();
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
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            if (value.GetType().GetCustomAttribute<MemberIDAttribute>().IsNull(out var memberIDAttribute))
                throw new JsonSerializationException($"Cannot find {nameof(MemberIDAttribute).Humanize()}");

            writer.WriteValue(memberIDAttribute.ID);
        }
    }
}
