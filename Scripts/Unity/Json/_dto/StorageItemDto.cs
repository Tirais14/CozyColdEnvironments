using CCEnvs.Json;
using CCEnvs.Json.DTO;
using CCEnvs.Unity.GameSystems.Storages;
using Newtonsoft.Json;
using System;

#nullable enable
#pragma warning disable S101
namespace CCEnvs.Unity.Json
{
    [JsonObject]
    [Serializable]
    public record StorageItemDto : ITypedJsonDTO, IJsonDtoConvertible<IStorageItem>
    {
        [JsonProperty]
        public Type ObjectType { get; set; } = null!;

        [JsonProperty]
        public int ID { get; set; } = -1;

        public StorageItemDto()
        {
        }

        public StorageItemDto(IStorageItem item)
        {
            ObjectType = item.GetType();
            ID = item.ID;
        }

        public IStorageItem ConvertToValue()
        {
            return DtoConverter.Convert<IStorageItem>(this)!;
        }
    }
}
