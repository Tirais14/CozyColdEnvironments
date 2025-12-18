using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;

#nullable enable
namespace CCEnvs.Json
{
    public static class JsonSerializerSettingsProvider
    {
        public static JsonSerializerSettings GetDefault(params JsonConverter[] converters)
        {
            JsonSerializerSettings settings;

            if (JsonConvert.DefaultSettings is not null)
                settings = new JsonSerializerSettings(JsonConvert.DefaultSettings());
            else
                settings = new JsonSerializerSettings();

            if (settings.Converters.IsReadOnly)
                settings.Converters = settings.Converters.Concat(converters).ToList();
            else
            {
                foreach (var conv in converters)
                    settings.Converters.Add(conv);
            }

            settings.NullValueHandling = NullValueHandling.Include;
            settings.TypeNameHandling = TypeNameHandling.Objects;
            settings.Error = (sender, args) => args.CurrentObject.PrintException(args.ErrorContext.Error);
            settings.Formatting = Formatting.Indented;
            settings.MaxDepth = 64;
            settings.ObjectCreationHandling = ObjectCreationHandling.Auto;
            settings.ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            return settings;
        }
    }
}
