using CCEnvs.Diagnostics;
using CCEnvs.Json.DTO;
using CCEnvs.Unity.GameSystems.Storages;
using Newtonsoft.Json;

#nullable enable
namespace CCEnvs.Unity.Json
{
    public record ItemStackDto : IJsonDto, ICCConvertible<ItemStack>
    {
        [JsonProperty]
        public int MaxItemCount { get; set; } = int.MaxValue;

        [JsonProperty]
        public IStorageItem? Item { get; set; }

        [JsonProperty]
        public int ItemCount { get; set; }

        public ItemStackDto()
        {
        }

        public ItemStackDto(ItemStack itemStack)
        {
            MaxItemCount = itemStack.MaxItemCount;
            Item = itemStack.Item;
            ItemCount = itemStack.ItemCount;
        }

        public ItemStack Convert()
        {
            if (Item.IsNull())
                return new ItemStack(MaxItemCount);

            return new ItemStack(Item, ItemCount, MaxItemCount);
        }
    }
}
