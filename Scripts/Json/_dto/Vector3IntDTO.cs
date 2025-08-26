using Newtonsoft.Json;
using System;
using UnityEngine;

#pragma warning disable S101
#pragma warning disable S1104
namespace UTIRLib.Json.DTO
{
    [Serializable]
    public struct Vector3IntDTO
    {
        [JsonProperty(nameof(x))]
        public int x;
        [JsonProperty(nameof(y))]
        public int y;
        [JsonProperty(nameof(z))]
        public int z;

        public static implicit operator Vector3Int(Vector3IntDTO dto)
        {
            return new Vector3Int(dto.x, dto.y, dto.z);
        }
    }
}
