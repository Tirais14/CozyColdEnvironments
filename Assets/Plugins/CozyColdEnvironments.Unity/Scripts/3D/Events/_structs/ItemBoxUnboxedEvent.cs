using Generator.Equals;

#nullable enable
namespace CCEnvs.Unity.D3.Events
{
    [Equatable]
    public readonly partial struct ItemBoxUnboxedEvent
    {
        [ReferenceEquality]
        public readonly ItemBox ItemBox;

        public readonly IBoxItem Item;

        public ItemBoxUnboxedEvent(ItemBox itemBox, IBoxItem item)
        {
            CC.Guard.IsNotNull(itemBox, nameof(itemBox));
            CC.Guard.IsNotNull(item, nameof(item));

            ItemBox = itemBox;
            Item = item;
        }
    }
}
