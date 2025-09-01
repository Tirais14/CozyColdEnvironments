using CCEnvs.Json;
using CCEnvs.Json.DTO;
using CCEnvs.Unity.GameSystems.Storages;
using Newtonsoft.Json;
using System;

#nullable enable
namespace CCEnvs.Unity.Json
{
    [JsonObject]
    [Serializable]
    public record ItemStackDto : ITypedJsonDTO, IJsonDtoConvertible<IItemStack>
    {
        [JsonProperty("stackType")]
        public Type ObjectType { get; set; } = null!;

        [JsonProperty("maxItemCount")]
        public int MaxItemCount { get; set; } = int.MaxValue;

        [JsonProperty("item")]
        public IStorageItem? Item { get; set; }

        [JsonProperty("itemCount")]
        public int ItemCount { get; set; }

        public ItemStackDto()
        {
        }

        public ItemStackDto(IItemStack itemStack)
        {
            ObjectType = itemStack.GetType();
            MaxItemCount = itemStack.MaxItemCount;
            Item = itemStack.Item;
            ItemCount = itemStack.ItemCount;
        }

        public IItemStack ConvertToValue()
        {
            return DtoConverter.Convert<IItemStack>(this)!;
        }
    }
}
