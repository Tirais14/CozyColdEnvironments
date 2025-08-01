#nullable enable
namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public interface IItemSlot
    {
        IItemStack ItemStack { get; }
    }
    public interface IItemSlot<T> : IItemSlot
        where T : IItemStack
    {
        new T ItemStack { get; }

        IItemStack IItemSlot.ItemStack => ItemStack;
    }
}
