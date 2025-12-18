using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;

#nullable enable
namespace CCEnvs.Json
{
    public static class JsonSerializerHelper
    {
        public static JsonSerializerSettings GetSerializerSettings(this JsonSerializer source)
        {
            Guard.IsNotNull(source, nameof(source));

            var settings = new JsonSerializerSettings
            {
                CheckAdditionalContent = source.CheckAdditionalContent,
                ConstructorHandling = source.ConstructorHandling,
                Context = source.Context,
                NullValueHandling = source.NullValueHandling,
                ContractResolver = source.ContractResolver,
                Culture = source.Culture,
                Converters = source.Converters,
                EqualityComparer = source.EqualityComparer,
                SerializationBinder = source.SerializationBinder,
                StringEscapeHandling = source.StringEscapeHandling,
                DateFormatString = source.DateFormatString,
                DateFormatHandling = source.DateFormatHandling,
                DateParseHandling = source.DateParseHandling,
                DateTimeZoneHandling = source.DateTimeZoneHandling,
                DefaultValueHandling = source.DefaultValueHandling,
                FloatFormatHandling = source.FloatFormatHandling,
                FloatParseHandling = source.FloatParseHandling,
                MaxDepth = source.MaxDepth,
                Formatting = source.Formatting,
                MetadataPropertyHandling = source.MetadataPropertyHandling,
                MissingMemberHandling = source.MissingMemberHandling,
                ObjectCreationHandling = source.ObjectCreationHandling,
                PreserveReferencesHandling = source.PreserveReferencesHandling,
                ReferenceLoopHandling = source.ReferenceLoopHandling,
                TraceWriter = source.TraceWriter,
                TypeNameAssemblyFormatHandling = source.TypeNameAssemblyFormatHandling,
                ReferenceResolverProvider = () => source.ReferenceResolver,
                TypeNameHandling = source.TypeNameHandling,
            };

            return settings;
        }
    }
}
