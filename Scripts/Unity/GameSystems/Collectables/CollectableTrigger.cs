using CCEnvs.Unity.Components;
using CCEnvs.Unity.GameSystems.Storages;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public abstract class CollectableTrigger : CCBehaviour, ICollectableTrigger
    {
        public bool IsEnabled {
            get => didStart && enabled;
            set => enabled = value;
        }

        public abstract IItemStack Collect(IStorageItem? item = null);
    }
}
