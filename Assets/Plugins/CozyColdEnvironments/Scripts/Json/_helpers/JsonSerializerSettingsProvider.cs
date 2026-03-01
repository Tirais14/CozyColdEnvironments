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

            settings.Error = (sender, args) => args.CurrentObject.PrintException(args.ErrorContext.Error);

            settings.TypeNameHandling = TypeNameHandling.None;
            settings.Formatting = Formatting.Indented;
            settings.MaxDepth = 64;
            settings.ObjectCreationHandling = ObjectCreationHandling.Auto;

            var ccContractResolver = new CCContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            };

            //var ignoreTypeMemberName = new KeyValuePair<Type, List<string>>[]
            //{
            //    KeyValuePair.Create(
            //        typeof(void),
            //        new List<string>(1)
            //        {
            //            "EqualityContract"
            //        })
            //};

            //ccContractResolver.IgnoredTypeMemberNames.AddRange(ignoreTypeMemberName);

            settings.ContractResolver = ccContractResolver;
            settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;

            return settings;
        }
    }
}
