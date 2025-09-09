#nullable enable
using CCEnvs.Reflection;
using Newtonsoft.Json;
using System;

namespace CCEnvs.Json.DTO
{
    [JsonObject]
    [Serializable]
    public record TypeDto : IJsonDto, IConvertibleCC<Type>
    {
        [JsonProperty]
        public string? AssemblyName { get; set; } = string.Empty;

        [JsonProperty]
        public string? Namespace { get; set; } = string.Empty;

        [JsonProperty]
        public string TypeName { get; set; } = string.Empty;

        [JsonProperty]
        public bool IgnoreCase { get; set; }

        public TypeDto()
        {
        }

        public TypeDto(Type type)
        {
            AssemblyName = type.Assembly.GetName().Name;
            Namespace = type.Namespace;
            TypeName = type.Name;
        }

        public Type Convert()
        {
            return TypeFinder.FindTypeInAppDomain(
                new TypeFinderParameters
                {
                    AssemblyName = AssemblyName,
                    Namespace = Namespace,
                    TypeName = TypeName,
                },
                throwOnError: true);
        }
    }
}
