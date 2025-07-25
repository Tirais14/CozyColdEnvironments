#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public class ItemSlotUI : IItemSlotUI
    {
        public IItemStackUI ItemStack { get; private set; }

        public ItemSlotUI(IItemStackUI itemStack)
        {
            ItemStack = itemStack;
        }
    }
}
