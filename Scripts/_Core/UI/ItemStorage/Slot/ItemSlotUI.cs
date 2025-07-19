#nullable enable
namespace UTIRLib.UI
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
