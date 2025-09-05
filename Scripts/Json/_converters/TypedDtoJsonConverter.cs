#nullable enable
using CCEnvs.Common;
using CCEnvs.Json.DTO;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace CCEnvs.Json.Converters
{
    public class TypedDtoJsonConverter<TDto, T> : JsonConverter<T>
        where TDto : ITypedJsonDto
    {
        public override T? ReadJson(JsonReader reader,
                                    Type objectType,
                                    T? existingValue,
                                    bool hasExistingValue,
                                    JsonSerializer serializer)
        {
            if (JsonSettingsProvider.IsDebugEnabled)
            {
                var token = JToken.Load(reader);

                CCDebug.PrintLog($"{this.GetTypeName()}: deserializing token.{Environment.NewLine}{token}");
            }

            var dto = serializer.Deserialize<TDto>(reader);

            return DtoConverter.Convert<T>(dto);
        }

        public override void WriteJson(JsonWriter writer,
                                       T? value,
                                       JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            InstanceFactory.Create(typeof(TDto),
                new ExplicitArguments(new ExplicitArgument(value)),
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);

            serializer.Serialize(writer, value);
        }
    }
}
