using Newtonsoft.Json;
using System;
using UnityEngine;

#pragma warning disable S101
#pragma warning disable S1104
namespace CCEnvs.Json.DTO
{
    [JsonObject]
    [Serializable]
    public struct Vector2Dto : IJsonDto, IJsonDtoConvertible<Vector2>
    {
        [JsonProperty]
        public float x;
        [JsonProperty]
        public float y;

        public static implicit operator Vector2(Vector2Dto dto)
        {
            return new Vector2(dto.x, dto.y);
        }

        public readonly Vector2 ConvertToValue() => new(x, y);
    }
}
