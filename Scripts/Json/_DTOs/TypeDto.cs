#nullable enable
using CCEnvs.Reflection;
using CCEnvs.Utils;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace CCEnvs.Json.DTO
{
    [JsonObject]
    [Serializable]
    public record TypeDto : IJsonDto, IJsonDtoConvertible<Type>
    {
        [JsonProperty]
        public string AssemblyName { get; set; } = string.Empty;

        [JsonProperty]
        public string Namespace { get; set; } = string.Empty;

        [JsonProperty]
        public string TypeName { get; set; } = string.Empty;

        [JsonProperty]
        public bool IgnoreCase { get; set; }

        public TypeDto()
        {
        }

        public TypeDto(Type type)
        {
            TypeName = type.GetName();
        }

        public Type ConvertToValue()
        {
            return TypeFinder.FindTypeInAppDomain(
                new TypeFinderParameters
                {
                    AssemblyName = AssemblyName,
                    Namepsace = Namespace,
                    TypeName = TypeName,
                },
                throwOnError: true) 
                ?? 
                throw new Diagnostics.DataAccessException($"Cannot find type: {this}.");
        }
    }
}
