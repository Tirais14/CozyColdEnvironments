using Newtonsoft.Json;
using System;

#nullable enable
#pragma warning disable S101
#pragma warning disable S1104
namespace UTIRLib.Json.Converters
{
    [Serializable]
    public struct VectorAllIntDTO
    {
        [JsonProperty(nameof(all))]
        public int all;
    }
}
