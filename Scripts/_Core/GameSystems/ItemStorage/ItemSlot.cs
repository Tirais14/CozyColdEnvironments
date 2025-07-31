#nullable enable
namespace UTIRLib.GameSystems.Storage
{
    public class ItemSlot<T> : IItemSlot<T>
        where T : IItemStack
    {
        public T ItemStack { get; private set; }

        public ItemSlot(T itemStack)
        {
            ItemStack = itemStack;
        }
    }
    public class ItemSlot : ItemSlot<IItemStack>
    {
        public ItemSlot(IItemStack itemStack) : base(itemStack)
        {
        }
    }
}
