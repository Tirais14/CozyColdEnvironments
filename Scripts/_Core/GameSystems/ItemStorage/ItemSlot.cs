#nullable enable
namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public class ItemSlot : IItemSlot
    {
        public IItemStack ItemStack { get; private set; }

        public ItemSlot(IItemStack itemStack)
        {
            ItemStack = itemStack;
        }
    }
    public class ItemSlot<T> : IItemSlot<T>
        where T : IItemStack
    {
        public T ItemStack { get; private set; }

        public ItemSlot(T itemStack)
        {
            ItemStack = itemStack;
        }
    }
}
