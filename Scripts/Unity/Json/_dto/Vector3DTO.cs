using Newtonsoft.Json;
using System;
using UnityEngine;

#pragma warning disable S101
#pragma warning disable S1104
namespace CCEnvs.Json.DTO
{
    [JsonObject]
    [Serializable]
    public struct Vector3Dto : IJsonDto, IJsonDtoConvertible<Vector3>
    {
        [JsonProperty]
        public float x;
        [JsonProperty]
        public float y;
        [JsonProperty]
        public float z;

        public static implicit operator Vector3(Vector3Dto dto)
        {
            return new Vector3(dto.x, dto.y, dto.z);
        }

        public readonly Vector3 ConvertToValue() => new(x, y, z);
    }
}
