#nullable enable
using CCEnvs.Reflection;
using Newtonsoft.Json;
using System;

namespace CCEnvs.Json.DTO
{
    [JsonObject]
    [Serializable]
    public record TypeDto : IJsonDto, ITransformable<Type>
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

        public Type DoTransform()
        {
            return TypeSearch.FindTypeInAppDomain(
                new TypeSearchArguments
                {
                    AssemblyName = AssemblyName,
                    NamespaceName = Namespace,
                    TypeName = TypeName,
                },
                throwOnError: true);
        }
    }
}
