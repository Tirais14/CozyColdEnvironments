using CozyColdEnvironments.GameSystems.Interactables;
using CozyColdEnvironments.GameSystems.ItemStorageSystem;

#nullable enable
namespace CozyColdEnvironments.GameSystems.Collectables
{
    public interface ICollectableItem : IInteractable<IItemStack>
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
        new TItemStack CollectItem();

        IItemStack ICollectableItem.CollectItem() => CollectItem();
    }
}
