using CCEnvs.Json.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public class TypeJsonConverter : JsonConverter<Type>
    {
        public override Type? ReadJson(JsonReader reader,
                                       Type objectType,
                                       Type? existingValue,
                                       bool hasExistingValue,
                                       JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            reader = token.CreateReader();

            TypeDto? dto;
            if (token.Type == JTokenType.String)
            {
                string? tokenString = token.Value<string>();

                if (tokenString is null)
                    throw new JsonSerializationException(nameof(tokenString));

                string[] splitted = tokenString.Split(',');

                string? assemblyName = null;
                if (splitted.Length > 1)
                    assemblyName = splitted[1];

                splitted = splitted[0].Split('.');

                if (tokenString is null)
                    throw new JsonSerializationException(nameof(splitted));

                string? namespaceValue = null;
                string typeName = splitted[^1];

                if (splitted.Length > 1)
                    namespaceValue = splitted[..^1].JoinStrings('.');

                dto = new TypeDto()
                {
                    AssemblyName = assemblyName?.Delete(' '),
                    Namespace = namespaceValue,
                    TypeName = typeName,
                };
            }
            else
                dto = serializer.Deserialize<TypeDto>(reader);

            return dto?.DoTransform();
        }

        public override void WriteJson(JsonWriter writer,
                                       Type? value,
                                       JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, new TypeDto(value));
        }
    }
}
