#nullable enable
namespace UTIRLib.GameSystems.Storage
{
    public class ItemSlot : IItemSlot
    {
        public IItemStack ItemStack { get; private set; }

        public ItemSlot(IItemStack itemStack)
        {
            ItemStack = itemStack;
        }
    }
}
