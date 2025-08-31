#nullable enable
using CCEnvs.Reflection.ObjectModel;
using CCEnvs.Returnables;
using CCEnvs.Unity.GameSystems.Interactables;
using CCEnvs.Unity.GameSystems.Storages;

namespace CCEnvs.Unity.GameSystems.Collectables
{
    public interface ICollectableUnit : IInteractable<IItemStack>
    {
        IStorageItem Item { get; }
        int ItemCount { get; }

        IItemStack CollectUnit();

        MethodResult<IItemStack> IInteractable<IItemStack>.Interact(ExplicitArguments args)
        {
            IItemStack collected = CollectUnit();

            if (!collected.HasItem)
                return MethodResult<IItemStack>.Failed;

            return new MethodResult<IItemStack>(isValidResults: true, collected);
        }

        MethodResult IInteractable.Interact(ExplicitArguments args) => Interact(args);
    }
    public interface ICollectableUnit<TItem> : ICollectableUnit
        where TItem : IStorageItem
    {
        new TItem Item { get; }

        IStorageItem ICollectableUnit.Item => Item;
    }
    public interface ICollectableUnit<TItem, TItemStack> : ICollectableUnit<TItem>
        where TItem : IStorageItem
        where TItemStack : IItemStack
    {
        new TItemStack CollectUnit();

        IItemStack ICollectableUnit.CollectUnit() => CollectUnit();
    }
}
