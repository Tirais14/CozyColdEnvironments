using Newtonsoft.Json;
using System;

#nullable enable
#pragma warning disable S101
#pragma warning disable S1104
namespace CCEnvs.Json.Converters
{
    [Serializable]
    public struct VectorAllDTO
    {
        [JsonProperty(nameof(all))]
        public float all;
    }
}
