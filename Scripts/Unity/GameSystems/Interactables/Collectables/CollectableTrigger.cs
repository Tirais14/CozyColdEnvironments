using CCEnvs.Unity.GameSystems.Storages;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public abstract class CollectableTrigger : MonoCC, ICollectableTrigger
    {
        public bool IsEnabled { get; set; }

        public abstract IItemStack InitiateCollecting(IStorageItem? item = null);
    }
}
