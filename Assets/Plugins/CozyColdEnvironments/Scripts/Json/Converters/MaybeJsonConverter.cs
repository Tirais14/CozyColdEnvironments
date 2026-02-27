using System;
using System.Linq;
using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public class MaybeJsonConverter<T> : JsonConverter<Maybe<T>>
    {
        public override bool CanWrite => false;

        public override Maybe<T> ReadJson(
            JsonReader reader,
            Type objectType,
            Maybe<T> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
            )
        {
            var filteredConvs = serializer.Converters.Where(typeof(MaybeJsonConverter<T>), (conv, thisType) => conv.GetType() != thisType)
                .EnumerableToArrayPooled();

            serializer.Converters.Clear();
            serializer.Converters.AddRange(filteredConvs);

            var jObj = JObject.Load(reader);

            try
            {
                return serializer.Deserialize<Maybe<T>>(jObj.CreateReader());
            }
            catch (Exception)
            {
                return serializer.Deserialize<T>(jObj.CreateReader());
            }
        }

        public override void WriteJson(JsonWriter writer, Maybe<T> value, JsonSerializer serializer)
        {
            //var filteredConvs = serializer.Converters.Where(typeof(MaybeJsonConverter<T>), (conv, thisType) => conv.GetType() != thisType)
            //    .EnumerableToArrayPooled();

            //serializer.Converters.Clear();
            //serializer.Converters.AddRange(filteredConvs);

            //serializer.Serialize(writer, value);
        }
    }
}
