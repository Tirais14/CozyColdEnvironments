using CCEnvs.Json.DTO;
using Newtonsoft.Json;
using System;

#nullable enable
namespace CCEnvs.Json.Converters
{
    [JsonObject]
    [Serializable]
    public struct VectorIntAllDto : IJsonDto
    {
        [JsonProperty]
        public int all;

        public readonly int ConvertToValue() => all;
    }
}
