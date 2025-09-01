using CCEnvs.Unity.GameSystems.Storages;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public abstract class CollectableTrigger : MonoCC, ICollectableTrigger
    {
        public bool IsEnabled {
            get => didStart && enabled;
            set => enabled = value;
        }

        public abstract IItemStack Collect(IStorageItem? item = null);
    }
}
