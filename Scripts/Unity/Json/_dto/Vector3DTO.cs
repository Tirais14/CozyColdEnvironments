using Newtonsoft.Json;
using System;
using UnityEngine;

#pragma warning disable S101
#pragma warning disable S1104
namespace CCEnvs.Json.DTO
{
    [JsonObject]
    [Serializable]
    public struct Vector3Dto : IJsonDTO
    {
        [JsonProperty(nameof(x))]
        public float x;
        [JsonProperty(nameof(y))]
        public float y;
        [JsonProperty(nameof(z))]
        public float z;

        public static implicit operator Vector3(Vector3Dto dto)
        {
            return new Vector3(dto.x, dto.y, dto.z);
        }
    }
}
