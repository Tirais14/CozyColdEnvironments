using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using ObservableCollections;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public interface IInventoryViewModel
        :
        IViewModel
    {
        IReadOnlyObservableDictionary<int, IItemContainer> Containers { get; }
        IReadOnlyObservableDictionary<IItemContainer, GameObject> ContainerViews { get; }
    }
}
