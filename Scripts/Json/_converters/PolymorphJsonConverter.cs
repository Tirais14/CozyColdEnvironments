#nullable enable
using CCEnvs.Common;
using CCEnvs.Diagnostics;
using CCEnvs.Json.Diagnsotics;
using CCEnvs.Json.DTO;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace CCEnvs.Json.Converters
{
    public class PolymorphJsonConverter<TIntermediate, T> : JsonConverter<T>
    {
        public override T? ReadJson(JsonReader reader,
                                    Type objectType,
                                    T? existingValue,
                                    bool hasExistingValue,
                                    JsonSerializer serializer)
        {
            if (JsonSerializerDebug.IsEnabled)
            {
                var token = JToken.Load(reader);

                CCDebug.PrintLog($"{this.GetTypeName()}: deserializing token.{Environment.NewLine}{token}");

                reader = token.CreateReader();
            }

            var dto = serializer.Deserialize<TIntermediate>(reader);

            return CCConvert.Convert<T>(dto);
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

            var dto = InstanceFactory.Create<TIntermediate>(
                new ExplicitArguments(new ExplicitArgument(value)),
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound
                |
                InstanceFactory.Parameters.NonPublic);

            serializer.Serialize(writer, dto);
        }
    }
}
