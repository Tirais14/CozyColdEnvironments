using CCEnvs.Json.DTO;
using Newtonsoft.Json;
using System;

#nullable enable
#pragma warning disable S101
#pragma warning disable S1104
namespace CCEnvs.Json.Converters
{
    [JsonObject]
    [Serializable]
    public struct VectorAllDto : IJsonDto
    {
        [JsonProperty(nameof(all))]
        public float all;
    }
}
