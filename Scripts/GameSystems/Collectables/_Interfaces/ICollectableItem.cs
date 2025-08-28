using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
namespace UTIRLib.GameSystems.Collectables
{
    public interface ICollectableItem
    {
        IStorageItem Item { get; }

        IItemStack CollectItem();
    }
    public interface ICollectableItem<TItem> : ICollectableItem
        where TItem : IStorageItem
    {
        new TItem Item { get; }

        IStorageItem ICollectableItem.Item => Item;
    }
    public interface ICollectableItem<TItem, TItemStack> : ICollectableItem<TItem>
        where TItem : IStorageItem
        where TItemStack : IItemStack
    {
        new TItemStack CollectItem
            ();

        IItemStack ICollectableItem.CollectItem() => CollectItem();
    }
}
