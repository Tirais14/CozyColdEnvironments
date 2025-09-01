#nullable enable
using CCEnvs.Reflection.ObjectModel;
using CCEnvs.Returnables;
using CCEnvs.Unity.GameSystems.Storages;
using System.Linq;

namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface ICollectableTrigger : IInteractableTrigger<IItemStack>
    {
        IItemStack InitiateCollecting(IStorageItem? item = null);

        MethodResult<IItemStack> IInteractableTrigger<IItemStack>.InitiateInteraction(ExplicitArguments args)
        {
            IItemStack collected = InitiateCollecting(args.arguments.FirstOrDefault() as IStorageItem);

            if (!collected.HasItem)
                return MethodResult<IItemStack>.Failed;

            return new MethodResult<IItemStack>(isValidResults: true, collected);
        }

        MethodResult IInteractableTrigger.InitiateInteraction(ExplicitArguments args) => InitiateInteraction(args);
    }
    public interface ICollectableTrigger<TItem> : ICollectableTrigger
        where TItem : IStorageItem
    {
        new TItem Item { get; }

        IStorageItem ICollectableTrigger.Item => Item;
    }
    public interface ICollectableTrigger<TItem, TItemStack> : ICollectableTrigger<TItem>
        where TItem : IStorageItem
        where TItemStack : IItemStack
    {
        new TItemStack InitiateCollecting(IStorageItem? item = null);

        IItemStack ICollectableTrigger.InitiateCollecting(IStorageItem? item) => InitiateCollecting(item);
    }
}
