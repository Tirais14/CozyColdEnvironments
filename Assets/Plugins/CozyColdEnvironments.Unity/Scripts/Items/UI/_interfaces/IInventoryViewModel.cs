using CCEnvs.UnityX.Items;
using CCEnvs.UnityX.UI;
using ObservableCollections;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Storages.UI
{
    public interface IInventoryViewModel
        :
        IViewModel
    {
        IReadOnlyObservableDictionary<int, IItemContainer> Containers { get; }
        IReadOnlyObservableDictionary<IItemContainer, GameObject> ContainerViews { get; }

        void AddContainer(IItemContainer cnt);

        void RemoveContainer(int id);
    }
}
