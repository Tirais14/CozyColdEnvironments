#nullable enable
using UTIRLib.UI.ItemSystem;

namespace UTIRLib.UI.StorageSystem
{
    public class ItemSlotUI : IItemSlotUI
    {
        public IItemStackUI ItemStack { get; private set; } = null!;

        public ItemSlotUI(IItemStackUI itemStack)
        {
            ItemStack = itemStack;
        }

    }
}
