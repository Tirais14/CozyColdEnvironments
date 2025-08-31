using CCEnvs.Json.DTO;
using Newtonsoft.Json;
using System;

#nullable enable
namespace CCEnvs.Json.Converters
{
    [JsonObject]
    [Serializable]
    public struct VectorAllIntDto : IJsonDto
    {
        [JsonProperty(nameof(all))]
        public int all;
    }
}
