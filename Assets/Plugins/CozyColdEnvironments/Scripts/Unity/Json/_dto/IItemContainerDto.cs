using CCEnvs.Diagnostics;
using CCEnvs.Json.DTO;
using CCEnvs.Reflection;
using CCEnvs.Unity.GameSystems.Storages;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Runtime.Serialization;

#nullable enable
#pragma warning disable S101
#pragma warning disable IDE0044
namespace CCEnvs.Unity.Json
{
    [JsonObject]
    [Serializable]
    public record IItemContainerDto : IJsonDto, ITypeProvider
    {
        [JsonProperty]
        public Type ObjectType { get; set; } = typeof(ItemStack);

        [JsonProperty]
        public int MaxItemCount { get; set; } = int.MaxValue;

        [JsonProperty]
        public int ItemCount { get; set; }

        [JsonProperty]
        public IStorageItem Item { get; set; } = null!;

        public IItemContainerDto()
        {
        }

        public IItemContainerDto(IItemContainer itemContainer)
        {
            ObjectType = itemContainer.GetType();
            MaxItemCount = itemContainer.MaxItemCount;
            ItemCount = itemContainer.ItemCount;
            Item = itemContainer.Item;
        }

        [OnDeserialized]
        private void Validate(StreamingContext _)
        {
            if (ObjectType.IsNotType<IItemStack>())
                CCDebug.PrintException(new IncorrectDataException(ObjectType));
        }
    }
    public record IItemContainerDto<T> : IItemContainerDto
        where T : IItemStack
    {
        public IItemContainerDto()
        {
            ObjectType = typeof(T);
        }

        public IItemContainerDto(T itemStack)
            :
            base(itemStack)
        {
        }
    }
}
