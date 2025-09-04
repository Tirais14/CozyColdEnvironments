#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Json.DTO;
using CCEnvs.Reflection.Data;
using Newtonsoft.Json;
using System;

namespace CCEnvs.Json.Converters
{
    public class CommonDtoJsonConverter<TDto, T> : JsonConverter<T>
        where TDto : IJsonDto
    {
        public override T? ReadJson(JsonReader reader,
                                    Type objectType,
                                    T? existingValue,
                                    bool hasExistingValue,
                                    JsonSerializer serializer)
        {
            var dto = serializer.Deserialize<TDto>(reader);

            return DtoConverter.Convert<T>(dto);
        }

        public override void WriteJson(JsonWriter writer,
                                       T? value,
                                       JsonSerializer serializer)
        {
            if (value.IsNull())
            {
                writer.WriteNull();
                return;
            }

            var dto = InstanceFactory.Create<TDto>(
                new Reflection.ConstructorBindings
                {
                    BindingFlags = BindingFlagsDefault.InstanceAll,
                    Arguments = new ExplicitArguments(new ExplicitArgument(value))
                },
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);

            serializer.Serialize(writer, dto);
        }
    }
}
