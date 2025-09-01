#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Json.DTO;
using CCEnvs.Reflection.ObjectModel;
using Newtonsoft.Json;
using System;

namespace CCEnvs.Json.Converters
{
    public class CommonDtoJsonConverter<TDto, T> : JsonConverter<T>
        where TDto : IJsonDtoConvertible
    {
        public override T? ReadJson(JsonReader reader,
                                    Type objectType,
                                    T? existingValue,
                                    bool hasExistingValue,
                                    JsonSerializer serializer)
        {
            var dto = serializer.Deserialize<TDto>(reader);

            if (dto.IsDefault())
                return default;

            if (dto is IJsonDtoConvertible<T> convertible)
                return convertible.ConvertToValue();

            return (T)dto.ConvertToValue();
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

            InstanceFactory.Create(typeof(TDto),
                new Reflection.ConstructorBindings
                {
                    BindingFlags = BindingFlagsDefault.InstanceAll,
                    Arguments = new ExplicitArguments(TypeValuePair.Create(value))
                },
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);
        }
    }
}
