using Newtonsoft.Json;
using System;
using UnityEngine;

#pragma warning disable S101
#pragma warning disable S1104
namespace CCEnvs.Json.DTO
{
    [JsonObject]
    [Serializable]
    public struct Vector2IntDto : IJsonDto
    {
        [JsonProperty(nameof(x))]
        public int x;
        [JsonProperty(nameof(y))]
        public int y;

        public static implicit operator Vector2Int(Vector2IntDto dto)
        {
            return new Vector2Int(dto.x, dto.y);
        }
    }
}
