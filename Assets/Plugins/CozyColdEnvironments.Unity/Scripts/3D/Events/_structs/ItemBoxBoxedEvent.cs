#nullable enable
using Generator.Equals;

namespace CCEnvs.Unity.D3.Events
{
    [Equatable]
    public readonly partial struct ItemBoxBoxedEvent
    {
        [ReferenceEquality]
        public readonly ItemBox ItemBox;

        public readonly IBoxItem Item;

        public ItemBoxBoxedEvent(ItemBox itemBox, IBoxItem item)
        {
            CC.Guard.IsNotNull(itemBox, nameof(itemBox));
            CC.Guard.IsNotNull(item, nameof(item));

            ItemBox = itemBox;
            Item = item;
        }
    }
}
