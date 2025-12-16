using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Json
{
    public static class JsonSerilizerOptionsProvider
    {
        public static JsonSerializerOptions GetDefault(params JsonConverter[] converters)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IncludeFields = true
            };

            foreach (var conv in converters)
                options.Converters.Add(conv);

            return options;
        }

        //public static JsonSerializerOptions GetDefaultPolymorph(params JsonConverter[] converters)
        //{
        //    return GetDefault(converters.Append(new PolymorphJsonConverter<object>()));
        //}
    }
}
