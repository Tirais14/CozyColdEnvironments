using System;
using Newtonsoft.Json;

#nullable enable
namespace UTIRLib.FileSystem.Json.Convertes
{
    [Serializable]
    [JsonObject]
    internal struct SpriteData
    {
        public string SpriteName { get; set; }
        public string SpritePath { get; set; }
    }
}