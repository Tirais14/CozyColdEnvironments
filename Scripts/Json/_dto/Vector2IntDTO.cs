using Newtonsoft.Json;
using System;
using UnityEngine;

#pragma warning disable S101
#pragma warning disable S1104
namespace CozyColdEnvironments.Json.DTO
{
    [Serializable]
    public struct Vector2IntDTO
    {
        [JsonProperty(nameof(x))]
        public int x;
        [JsonProperty(nameof(y))]
        public int y;

        public static implicit operator Vector2Int(Vector2IntDTO dto)
        {
            return new Vector2Int(dto.x, dto.y);
        }
    }
}
