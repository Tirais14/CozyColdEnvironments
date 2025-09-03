using CCEnvs.Json.DTO;
using CCEnvs.Unity.GameSystems.Storages;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using CCEnvs.Diagnostics;
using CCEnvs.Common;

#nullable enable
#pragma warning disable S101
namespace CCEnvs.Unity.Json
{
    [JsonObject]
    [Serializable]
    public record IStorageItemDto : ITypedJsonDTO
    {
        [JsonProperty]
        public Type ObjectType { get; set; } = null!;

        [JsonProperty("id")]
        public int ID { get; set; } = -1;

        public IStorageItemDto()
        {
        }

        public IStorageItemDto(IStorageItem item)
        {
            ObjectType = item.GetType();
            ID = item.ID;
        }

        [OnDeserialized]
        private void Validate(StreamingContext _)
        {
            if (ID < 1)
                CCDebug.PrintException(new DataAccessException(ID, nameof(ID)));
        }
    }
}
