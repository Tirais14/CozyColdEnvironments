using Newtonsoft.Json;
using System;
using UnityEngine;

#pragma warning disable S101
#pragma warning disable S1104
namespace CCEnvs.Json.DTO
{
    [JsonObject]
    [Serializable]
    public struct Vector3IntDto : IJsonDto, ICCConvertible<Vector3Int>
    {
        [JsonProperty]
        public int x;
        [JsonProperty]
        public int y;
        [JsonProperty]
        public int z;

        public static implicit operator Vector3Int(Vector3IntDto dto)
        {
            return new Vector3Int(dto.x, dto.y, dto.z);
        }

        public readonly Vector3Int Convert() => new(x, y, z);
    }
}
